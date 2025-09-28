// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.UI.Input;
using System.Collections;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using WinRT;
using VirtualKey = Windows.System.VirtualKey;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

[DependencyProperty<Style>("AutoSuggestBoxStyle")]
[DependencyProperty<Style>("AutoSuggestBoxTextBoxStyle")]
[DependencyProperty<IReadOnlyDictionary<string, SearchToken>>("AvailableTokens")]
[DependencyProperty<ICommand>("FilterCommand")]
[DependencyProperty<object>("FilterCommandParameter")]
[DependencyProperty<int>("MaximumTokens", DefaultValue = -1, PropertyChangedCallbackName = nameof(OnMaximumTokensChanged), NotNull = true)]
[DependencyProperty<string>("PlaceholderText")]
[DependencyProperty<IconSource>("QueryIcon")]
[DependencyProperty<object>("SuggestedItemsSource")]
[DependencyProperty<DataTemplate>("SuggestedItemTemplate")]
[DependencyProperty<DataTemplateSelector>("SuggestedItemTemplateSelector")]
[DependencyProperty<Style>("SuggestedItemContainerStyle")]
[DependencyProperty<bool>("TabNavigateBackOnArrow", DefaultValue = false, NotNull = true)]
[DependencyProperty<string>("Text", PropertyChangedCallbackName = nameof(OnTextPropertyChanged))]
[DependencyProperty<string>("TextMemberPath")]
[DependencyProperty<DataTemplate>("TokenItemTemplate")]
[DependencyProperty<DataTemplateSelector>("TokenItemTemplateSelector")]
[DependencyProperty<string>("TokenDelimiter", DefaultValue = " ")]
[DependencyProperty<double>("TokenSpacing", NotNull = true)]
[TemplatePart(Name = NormalState, Type = typeof(VisualState))]
[TemplatePart(Name = PointerOverState, Type = typeof(VisualState))]
[TemplatePart(Name = FocusedState, Type = typeof(VisualState))]
[TemplatePart(Name = UnfocusedState, Type = typeof(VisualState))]
[TemplatePart(Name = MaxReachedState, Type = typeof(VisualState))]
[TemplatePart(Name = MaxUnreachedState, Type = typeof(VisualState))]
internal sealed partial class AutoSuggestTokenBox : ListViewBase
{
    public const string NormalState = "Normal";
    public const string PointerOverState = "PointerOver";
    public const string FocusedState = "Focused";
    public const string UnfocusedState = "Unfocused";
    public const string MaxReachedState = "MaxReached";
    public const string MaxUnreachedState = "MaxUnreached";

    private InterspersedObservableCollection innerItemsSource;
    private ITokenStringContainer currentTextEdit; // Don't update this directly outside of initialization, use UpdateCurrentTextEdit Method
    private ITokenStringContainer lastTextEdit;

    public AutoSuggestTokenBox()
    {
        innerItemsSource = [];
        currentTextEdit = lastTextEdit = new PreTokenStringContainer(true);
        innerItemsSource.Insert(innerItemsSource.Count, currentTextEdit);
        ItemsSource = innerItemsSource;

        DefaultStyleKey = typeof(AutoSuggestTokenBox);

        RegisterPropertyChangedCallback(ItemsSourceProperty, OnItemsSourceChanged);
        PreviewKeyDown += OnPreviewKeyDown;
        PreviewKeyUp += OnPreviewKeyUp;
        CharacterReceived += OnCharacterReceived;
        ItemClick += OnItemClick;
    }

    private enum MoveDirection
    {
        Next,
        Previous,
    }

    public static bool IsControlPressed
    {
        get => InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
    }

    public static bool IsShiftPressed
    {
        get => InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
    }

    public bool IsClearingForClick { get; set; }

    public bool PauseTokenClearOnFocus { get; set; }

    public string SelectedTokenText
    {
        get => PrepareSelectionForClipboard();
    }

    public IEnumerable<SearchToken> Tokens
    {
        get => ItemsSource.As<IList>().OfType<SearchToken>();
    }

    public void AddToken(object data, bool atEnd = false)
    {
        if (MaximumTokens >= 0 && IsTokenLimitReached())
        {
            return;
        }

        if (data is string str)
        {
            TokenItemAddingEventArgs args = new(str);
            OnTokenItemAdding(this, args);

            if (args.Cancel)
            {
                return;
            }

            if (args.Item is not null)
            {
                data = args.Item;
            }
        }

        // Custom: Avoid add same token
        if (innerItemsSource.Contains(data))
        {
            return;
        }

        // If we've been typing in the last box, just add this to the end of our collection
        if (atEnd || currentTextEdit == lastTextEdit)
        {
            innerItemsSource.InsertAt(innerItemsSource.Count - 1, data);
        }
        else
        {
            // Otherwise, we'll insert before our current box
            ITokenStringContainer edit = currentTextEdit;
            int index = innerItemsSource.IndexOf(edit);

            // Insert our new data item at the location of our textbox
            innerItemsSource.InsertAt(index, data);

            // Remove our textbox
            innerItemsSource.Remove(edit);
        }

        AutoSuggestTokenBoxItem last = ContainerFromItem(lastTextEdit).As<AutoSuggestTokenBoxItem>();
        ArgumentNullException.ThrowIfNull(last.AutoSuggestTextBox);
        last.AutoSuggestTextBox.Focus(FocusState.Keyboard);

        OnTokenItemAdded(this, data);
        UpdatePlaceholderVisibility();
    }

    public void Clear()
    {
        while (innerItemsSource.Count > 1)
        {
            if (ContainerFromItem(innerItemsSource[0]) is AutoSuggestTokenBoxItem container)
            {
                if (!RemoveToken(container, innerItemsSource[0]))
                {
                    break;
                }
            }
        }

        Text = string.Empty;
    }

    public void DeselectAllTokensAndText(AutoSuggestTokenBoxItem? ignoreItem = default)
    {
        this.DeselectAll();
        ClearAllTextSelections(ignoreItem);
    }

    public bool IsTokenLimitReached()
    {
        return innerItemsSource.ItemsSource.Count >= MaximumTokens;
    }

    public void RefreshTokenCounter()
    {
        if (ContainerFromIndex(Items.Count - 1) is not AutoSuggestTokenBoxItem { AutoSuggestBox: { } autoSuggestBox, AutoSuggestTextBox: { } autoSuggestTextBox })
        {
            return;
        }

        if (autoSuggestBox.FindDescendant("TextTokensCounter") is not Microsoft.UI.Xaml.Controls.TextBlock maxTokensCounter)
        {
            return;
        }

        int currentTokens = innerItemsSource.ItemsSource.Count;
        int maxTokens = MaximumTokens;

        maxTokensCounter.Text = $"{currentTokens}/{maxTokens}";
        maxTokensCounter.Visibility = Visibility.Visible;

        string targetState = IsTokenLimitReached()
            ? MaxReachedState
            : MaxUnreachedState;

        VisualStateManager.GoToState(autoSuggestTextBox, targetState, true);
    }

    public void OnTextChanged(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (IsTokenLimitReached())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Text))
        {
            sender.ItemsSource = AvailableTokens?
                .ExceptBy(Tokens, kvp => kvp.Value)
                .OrderBy(kvp => kvp.Value.Kind)
                .ThenBy(kvp => kvp.Value.Order)
                .Select(kvp => kvp.Value);
        }

        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = AvailableTokens?
                .ExceptBy(Tokens, kvp => kvp.Value)
                .Where(kvp => kvp.Value.Value.Contains(Text ?? string.Empty, StringComparison.OrdinalIgnoreCase))
                .OrderBy(kvp => kvp.Value.Kind)
                .ThenBy(kvp => kvp.Value.Order)
                .Select(kvp => kvp.Value)
                .DefaultIfEmpty(SearchToken.NotFound);
        }
    }

    public void OnQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is not null)
        {
            return;
        }

        FilterCommand.TryExecute(FilterCommandParameter);
    }

    public void OnSuggestionsChosen(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
    }

    public void OnTokenItemAdding(AutoSuggestTokenBox sender, TokenItemAddingEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.TokenText))
        {
            return;
        }

        if (AvailableTokens?.GetValueOrDefault(args.TokenText) is { } token)
        {
            args.Item = token;
        }
        else
        {
            args.Cancel = true;
        }
    }

    public void OnTokenItemAdded(AutoSuggestTokenBox sender, object args)
    {
        if (args is SearchToken { Kind: SearchTokenKind.None } token)
        {
            ItemsSource.As<IList>().Remove(token);
        }

        RefreshTokenCounter();

        FilterCommand.TryExecute(FilterCommandParameter);
    }

    public void OnTokenItemRemoved(AutoSuggestTokenBox sender, object args)
    {
        RefreshTokenCounter();
        FilterCommand.TryExecute(FilterCommandParameter);
    }

    public void OnTokenItemRemoving(AutoSuggestTokenBox sender, TokenItemRemovingEventArgs args)
    {
    }

    public void RemoveAllSelectedTokens()
    {
        while (SelectedItems.Count > 0)
        {
            if (ContainerFromItem(SelectedItems[0]) is AutoSuggestTokenBoxItem container)
            {
                if (IndexFromContainer(container) != Items.Count - 1)
                {
                    // if its a text box, remove any selected text, and if its then empty remove the container, unless its focused
                    if (SelectedItems[0] is ITokenStringContainer)
                    {
                        ArgumentNullException.ThrowIfNull(container.AutoSuggestTextBox);
                        TextBox astb = container.AutoSuggestTextBox;

                        // grab any selected text
                        string tempStr = astb.SelectionStart is 0
                            ? string.Empty
                            : astb.Text[..astb.SelectionStart];
                        tempStr += astb.SelectionStart + astb.SelectionLength < astb.Text.Length
                                ? astb.Text[(astb.SelectionStart + astb.SelectionLength)..]
                                : string.Empty;

                        if (tempStr.Length is 0)
                        {
                            RemoveToken(container);
                        }
                        else
                        {
                            astb.Text = tempStr;
                        }
                    }
                    else
                    {
                        RemoveToken(container);
                    }
                }
                else
                {
                    if (SelectedItems.Count is 1)
                    {
                        // Only remains the default textbox.
                        break;
                    }
                }
            }
        }
    }

    [Command("SelectAllTokensAndTextCommand")]
    public void SelectAllTokensAndText()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Select all tokens and text", "AutoSuggestTokenBox.Command"));
        DispatcherQueue.TryEnqueue(SelectAllTokensAndTextCore);

        void SelectAllTokensAndTextCore()
        {
            this.SelectAllSafe();

            // need to synchronize the select all and the focus behavior on the text box
            // because there is no way to identify that the focus has been set from this point
            // to avoid instantly clearing the selection of tokens
            PauseTokenClearOnFocus = true;

            foreach (object? item in Items)
            {
                if (item is not ITokenStringContainer)
                {
                    continue;
                }

                // grab any selected text
                if (ContainerFromItem(item) is AutoSuggestTokenBoxItem pretoken)
                {
                    ArgumentNullException.ThrowIfNull(pretoken.AutoSuggestTextBox);
                    pretoken.AutoSuggestTextBox.SelectionStart = 0;
                    pretoken.AutoSuggestTextBox.SelectionLength = pretoken.AutoSuggestTextBox.Text.Length;
                }
            }

            if (ContainerFromIndex(Items.Count - 1) is AutoSuggestTokenBoxItem container)
            {
                container.Focus(FocusState.Programmatic);
            }
        }
    }

    public bool SelectNextItem(AutoSuggestTokenBoxItem item)
    {
        return SelectNewItem(item, 1, i => i < Items.Count - 1);
    }

    public bool SelectPreviousItem(AutoSuggestTokenBoxItem item)
    {
        return SelectNewItem(item, -1, i => i > 0);
    }

    /// <inheritdoc/>
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new AutoSuggestTokenBoxItem();
    }

    /// <inheritdoc/>
    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is AutoSuggestTokenBoxItem;
    }

    /// <inheritdoc/>
    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new AutoSuggestTokenBoxAutomationPeer(this);
    }

    /// <inheritdoc/>
    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);

        if (element is AutoSuggestTokenBoxItem tokenItem)
        {
            tokenItem.Owner = this;

            tokenItem.ContentTemplateSelector = TokenItemTemplateSelector;
            tokenItem.ContentTemplate = TokenItemTemplate;

            tokenItem.ClearAllAction -= OnItemClearAllAction;
            tokenItem.ClearAllAction += OnItemClearAllAction;

            tokenItem.GotFocus -= OnItemGotFocus;
            tokenItem.GotFocus += OnItemGotFocus;

            tokenItem.LostFocus -= OnItemLostFocus;
            tokenItem.LostFocus += OnItemLostFocus;
        }
    }

    private static void OnMaximumTokensChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AutoSuggestTokenBox { MaximumTokens: >= 0 } ttb && e.NewValue is int newMaxTokens)
        {
            int tokenCount = ttb.innerItemsSource.ItemsSource.Count;
            if (tokenCount > 0 && tokenCount > newMaxTokens)
            {
                int tokensToRemove = tokenCount - Math.Max(newMaxTokens, 0);

                // Start at the end, remove any extra tokens.
                for (int i = tokenCount; i > tokenCount - tokensToRemove; --i)
                {
                    if (ttb.innerItemsSource.ItemsSource[i - 1] is { } token)
                    {
                        // Force remove the items. No warning and no option to cancel.
                        ttb.innerItemsSource.Remove(token);
                        ttb.OnTokenItemRemoved(ttb, token);
                    }
                }
            }
        }
    }

    private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AutoSuggestTokenBox { currentTextEdit: { } } ttb)
        {
            if (e.NewValue is string newValue)
            {
                ttb.currentTextEdit.Text = newValue;

                // Notify inner container of text change, see issue #4749
                if (ttb.ContainerFromItem(ttb.currentTextEdit) is AutoSuggestTokenBoxItem item)
                {
                    item.UpdateText(newValue);
                }
            }
        }
    }

    private void ClearAllTextSelections(AutoSuggestTokenBoxItem? ignoreItem)
    {
        // Clear any selection in the text box
        foreach (object? item in Items)
        {
            if (item is not ITokenStringContainer)
            {
                continue;
            }

            if (ContainerFromItem(item) is AutoSuggestTokenBoxItem container)
            {
                if (container != ignoreItem)
                {
                    ArgumentNullException.ThrowIfNull(container.AutoSuggestTextBox);
                    container.AutoSuggestTextBox.SelectionLength = 0;
                }
            }
        }
    }

    private void CopySelectedToClipboard()
    {
        DataPackage dataPackage = new()
        {
            RequestedOperation = DataPackageOperation.Copy,
        };

        string tokenString = PrepareSelectionForClipboard();

        if (!string.IsNullOrEmpty(tokenString))
        {
            dataPackage.SetText(tokenString);
            Clipboard.SetContent(dataPackage);
        }
    }

    private void FocusPrimaryAutoSuggestBox()
    {
        if (Items.Count > 0)
        {
            if (ContainerFromIndex(Items.Count - 1) is AutoSuggestTokenBoxItem container)
            {
                container.Focus(FocusState.Programmatic);
            }
        }
    }

    private AutoSuggestTokenBoxItem? GetCurrentContainerItem()
    {
        return XamlRoot is not null
            ? FocusManager.GetFocusedElement(XamlRoot) as AutoSuggestTokenBoxItem
            : FocusManager.GetFocusedElement() as AutoSuggestTokenBoxItem;
    }

    private object GetFocusedElement()
    {
        return XamlRoot is not null
            ? FocusManager.GetFocusedElement(XamlRoot)
            : FocusManager.GetFocusedElement();
    }

    private bool MoveFocusAndSelection(MoveDirection direction)
    {
        bool ret = false;

        if (GetCurrentContainerItem() is { } currentContainerItem)
        {
            object? currentItem = ItemFromContainer(currentContainerItem);
            int previousIndex = Items.IndexOf(currentItem);
            int index = previousIndex;

            switch (direction)
            {
                case MoveDirection.Previous:
                    if (previousIndex > 0)
                    {
                        index -= 1;
                        break;
                    }

                    if (TabNavigateBackOnArrow)
                    {
                        FocusManager.TryMoveFocus(FocusNavigationDirection.Previous, new()
                        {
                            SearchRoot = XamlRoot.Content,
                        });
                    }

                    ret = true;

                    break;
                case MoveDirection.Next:
                    if (previousIndex < Items.Count - 1)
                    {
                        index += 1;
                    }

                    break;
            }

            // Only do stuff if the index is actually changing
            if (index != previousIndex)
            {
                if (ContainerFromIndex(index) is AutoSuggestTokenBoxItem newItem)
                {
                    // Check for the new item being a text control.
                    // this must happen before focus is set to avoid seeing the caret
                    // jump in come cases
                    if (Items[index] is ITokenStringContainer && !IsShiftPressed)
                    {
                        ArgumentNullException.ThrowIfNull(newItem.AutoSuggestTextBox);
                        newItem.AutoSuggestTextBox.SelectionLength = 0;
                        newItem.AutoSuggestTextBox.SelectionStart = direction switch
                        {
                            MoveDirection.Previous => newItem.AutoSuggestTextBox.Text.Length,
                            MoveDirection.Next => 0,
                            _ => throw new NotSupportedException(),
                        };
                    }

                    newItem.Focus(FocusState.Keyboard);

                    // if no control keys are selected then the selection also becomes just this item
                    if (IsShiftPressed)
                    {
                        // What we do here depends on where the selection started
                        // if the previous item is between the start and new position then we add the new item to the selected range
                        // if the new item is between the start and the previous position then we remove the previous position
                        int newDistance = Math.Abs(SelectedIndex - index);
                        int oldDistance = Math.Abs(SelectedIndex - previousIndex);

                        if (newDistance > oldDistance)
                        {
                            SelectedItems.Add(Items[index]);
                        }
                        else
                        {
                            SelectedItems.Remove(Items[previousIndex]);
                        }
                    }
                    else if (!IsControlPressed)
                    {
                        SelectedIndex = index;

                        // This looks like a bug in the underlying ListViewBase control.
                        // Might need to be reviewed if the base behavior is fixed
                        // When two consecutive items are selected and the navigation moves between them,
                        // the first time that happens the old focused item is not unselected
                        if (SelectedItems.Count > 1)
                        {
                            SelectedItems.Clear();
                            SelectedIndex = index;
                        }
                    }

                    ret = true;
                }
            }
        }

        return ret;
    }

    private void OnCharacterReceived(UIElement sender, CharacterReceivedRoutedEventArgs args)
    {
        if (ContainerFromItem(currentTextEdit) is AutoSuggestTokenBoxItem container && !(GetFocusedElement().Equals(container.AutoSuggestTextBox) || char.IsControl(args.Character)))
        {
            if (SelectedItems.Count > 0)
            {
                int index = innerItemsSource.IndexOf(SelectedItems.First());

                RemoveAllSelectedTokens();

                void RemoveOldItems()
                {
                    // If we're before the last textbox and it's empty, redirect focus to that one instead
                    if (index == innerItemsSource.Count - 1 && string.IsNullOrWhiteSpace(lastTextEdit.Text))
                    {
                        if (ContainerFromItem(lastTextEdit) is AutoSuggestTokenBoxItem lastContainer)
                        {
                            lastContainer.UseCharacterAsUser = true;

                            lastTextEdit.Text = string.Empty + args.Character;
                            UpdateCurrentTextEdit(lastTextEdit);

                            ArgumentNullException.ThrowIfNull(lastContainer.AutoSuggestTextBox);
                            lastContainer.AutoSuggestTextBox.SelectionStart = 1;
                            lastContainer.AutoSuggestTextBox.Focus(FocusState.Keyboard);
                        }
                    }
                    else
                    {
                        // Otherwise, create a new textbox for this text.
                        UpdateCurrentTextEdit(new PreTokenStringContainer((string.Empty + args.Character).Trim()));
                        innerItemsSource.Insert(index, currentTextEdit);

                        void Containerization()
                        {
                            if (ContainerFromIndex(index) is AutoSuggestTokenBoxItem newContainer)
                            {
                                newContainer.UseCharacterAsUser = true;

                                void WaitForLoad(object s, RoutedEventArgs eargs)
                                {
                                    if (newContainer.AutoSuggestTextBox is not null)
                                    {
                                        newContainer.AutoSuggestTextBox.SelectionStart = 1;
                                        newContainer.AutoSuggestTextBox.Focus(FocusState.Keyboard);
                                    }

                                    newContainer.AutoSuggestTextBoxLoaded -= WaitForLoad;
                                }

                                newContainer.AutoSuggestTextBoxLoaded += WaitForLoad;
                            }
                        }

                        // Need to wait for containerization
                        DispatcherQueue.TryEnqueue(Containerization);
                    }
                }

                // Wait for removal of old items
                DispatcherQueue.TryEnqueue(RemoveOldItems);
            }
            else
            {
                // If no items are selected, send input to the last active string container.
                // This code is only fires during an edgecase where an item is in the process of being deleted and the user inputs a character before the focus has been redirected to a string container.
                if (innerItemsSource[^1] is ITokenStringContainer textEdit)
                {
                    // Should be our last text box
                    if (ContainerFromIndex(Items.Count - 1) is AutoSuggestTokenBoxItem last)
                    {
                        ArgumentNullException.ThrowIfNull(last.AutoSuggestTextBox);
                        string text = last.AutoSuggestTextBox.Text;
                        int selectionStart = last.AutoSuggestTextBox.SelectionStart;
                        int position = Math.Min(selectionStart, text.Length);
                        textEdit.Text = text[..position] + args.Character + text[position..];

                        last.AutoSuggestTextBox.SelectionStart = position + 1;
                        last.AutoSuggestTextBox.Focus(FocusState.Keyboard);
                    }
                }
            }
        }
    }

    private void OnItemClearAllAction(AutoSuggestTokenBoxItem sender, RoutedEventArgs args)
    {
        int newSelectedIndex = SelectedRanges.Count > 0
            ? SelectedRanges[0].FirstIndex - 1
            : -1;

        RemoveAllSelectedTokens();

        SelectedIndex = newSelectedIndex;

        if (newSelectedIndex is -1)
        {
            newSelectedIndex = Items.Count - 1;
        }

        // focus the item prior to the first selected item
        if (ContainerFromIndex(newSelectedIndex) is AutoSuggestTokenBoxItem container)
        {
            container.Focus(FocusState.Keyboard);
        }
    }

    private void OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (!IsControlPressed)
        {
            IsClearingForClick = true;
            ClearAllTextSelections(default);
        }
    }

    private void OnItemGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is AutoSuggestTokenBoxItem { AutoSuggestBox: { } autoSuggestBox, Content: ITokenStringContainer text })
        {
            UpdateCurrentTextEdit(text);

            // Custom: Show all items when focus on the textbox
            OnTextChanged(autoSuggestBox, new());
        }
    }

    private void OnItemLostFocus(object sender, RoutedEventArgs e)
    {
        // Keep track of our currently focused textbox
        if (sender is AutoSuggestTokenBoxItem { Content: ITokenStringContainer text } && string.IsNullOrWhiteSpace(text.Text) && text != lastTextEdit)
        {
            // We're leaving an inner textbox that's blank, so we'll remove it
            innerItemsSource.Remove(text);

            UpdateCurrentTextEdit(lastTextEdit);

            UpdatePlaceholderVisibility();
        }
    }

    private void OnItemsSourceChanged(DependencyObject sender, DependencyProperty dp)
    {
        // If we're given a different ItemsSource, we need to wrap that collection in our helper class.
        if (ItemsSource is { } and not InterspersedObservableCollection)
        {
            innerItemsSource = new(ItemsSource);

            if (MaximumTokens >= 0 && IsTokenLimitReached())
            {
                // Reduce down to below the max as necessary.
                int endCount = MaximumTokens > 0 ? MaximumTokens : 0;
                for (int i = innerItemsSource.ItemsSource.Count - 1; i >= endCount; --i)
                {
                    innerItemsSource.Remove(innerItemsSource[i]);
                }
            }

            // Add our text box at the end of items and set its default value to our initial text, fix for #4749
            currentTextEdit = lastTextEdit = new PreTokenStringContainer(true) { Text = Text };
            innerItemsSource.Insert(innerItemsSource.Count, currentTextEdit);
            ItemsSource = innerItemsSource;
        }
    }

    private void OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.C:
                if (IsControlPressed)
                {
                    CopySelectedToClipboard();
                    e.Handled = true;
                }

                break;

            case VirtualKey.X:
                if (IsControlPressed)
                {
                    CopySelectedToClipboard();

                    // now clear all selected tokens and text, or all if none are selected
                    RemoveAllSelectedTokens();
                }

                break;

            // For moving between tokens
            case VirtualKey.Left:
                e.Handled = MoveFocusAndSelection(MoveDirection.Previous);
                return;

            case VirtualKey.Right:
                e.Handled = MoveFocusAndSelection(MoveDirection.Next);
                return;

            case VirtualKey.A:
                // modify the select-all behavior to ensure the text in the edit box gets selected.
                if (IsControlPressed)
                {
                    SelectAllTokensAndText();
                    e.Handled = true;
                }

                break;
        }
    }

    private void OnPreviewKeyUp(object sender, KeyRoutedEventArgs e)
    {
        switch (e.Key)
        {
            case VirtualKey.Escape:
                // Clear any selection and place the focus back into the text box
                DeselectAllTokensAndText();
                FocusPrimaryAutoSuggestBox();
                break;
        }
    }

    private string PrepareSelectionForClipboard()
    {
        string tokenString = string.Empty;
        bool addSeparator = false;

        // Copy all items if none selected (and no text selected)
        foreach (object? item in SelectedItems.Count > 0 ? SelectedItems : Items)
        {
            if (addSeparator)
            {
                tokenString += TokenDelimiter;
            }
            else
            {
                addSeparator = true;
            }

            if (item is ITokenStringContainer)
            {
                // grab any selected text
                if (ContainerFromItem(item) is AutoSuggestTokenBoxItem { AutoSuggestTextBox: { } textBox })
                {
                    tokenString += textBox.Text.Substring(textBox.SelectionStart, textBox.SelectionLength);
                }
            }
            else
            {
                tokenString += item.ToString();
            }
        }

        return tokenString;
    }

    [Command("RemoveItemCommand")]
    private void RemoveToken(AutoSuggestTokenBoxItem? item)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove token", "AutoSuggestTokenBox.Command"));
        ArgumentNullException.ThrowIfNull(item);
        RemoveToken(item, default);
    }

    private bool RemoveToken(AutoSuggestTokenBoxItem item, object? data)
    {
        data ??= ItemFromContainer(item);

        TokenItemRemovingEventArgs tirea = new(data, item);
        OnTokenItemRemoving(this, tirea);

        if (tirea.Cancel)
        {
            return false;
        }

        innerItemsSource.Remove(data);

        OnTokenItemRemoved(this, data);
        UpdatePlaceholderVisibility();

        return true;
    }

    private bool SelectNewItem(AutoSuggestTokenBoxItem item, int increment, Func<int, bool> testFunc)
    {
        // find the item in the list
        int currentIndex = IndexFromContainer(item);

        // Select previous token item (if there is one).
        if (testFunc(currentIndex))
        {
            if (ContainerFromItem(Items[currentIndex + increment]) is ListViewItem newItem)
            {
                newItem.Focus(FocusState.Keyboard);
                SelectedItems.Add(Items[currentIndex + increment]);
                return true;
            }
        }

        return false;
    }

    private void UpdateCurrentTextEdit(ITokenStringContainer edit)
    {
        currentTextEdit = edit;

        Text = edit.Text; // Update our text property.
    }

    private void UpdatePlaceholderVisibility()
    {
        if (ContainerFromItem(lastTextEdit)?.FindDescendant("PlaceholderTextContentPresenter") is not { } placeholder)
        {
            return;
        }

        // Fix layout issue
        placeholder.Visibility = Visibility.Collapsed;

        CompositionTargetHelper.ExecuteAfterCompositionRenderingAsync(() =>
        {
            int currentTokens = innerItemsSource.ItemsSource.Count;
            int maxTokens = MaximumTokens;
            placeholder.Visibility = currentTokens >= maxTokens
                ? Visibility.Collapsed
                : Visibility.Visible;
        }).SafeForget();
    }
}