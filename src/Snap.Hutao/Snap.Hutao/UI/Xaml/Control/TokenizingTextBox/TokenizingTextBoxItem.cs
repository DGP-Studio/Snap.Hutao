// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.System;

namespace Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;

[DependencyProperty("ClearButtonStyle", typeof(Style))]
[DependencyProperty("Owner", typeof(TokenizingTextBox))]
[TemplatePart(Name = TokenRemoveButton, Type = typeof(ButtonBase))] //// Token case
[TemplatePart(Name = TextAutoSuggestBox, Type = typeof(Microsoft.UI.Xaml.Controls.AutoSuggestBox))] //// String case
[TemplatePart(Name = TextTokensCounter, Type = typeof(Microsoft.UI.Xaml.Controls.TextBlock))]
internal partial class TokenizingTextBoxItem : ListViewItem
{
    private const string TokenRemoveButton = nameof(TokenRemoveButton);
    private const string TextAutoSuggestBox = nameof(TextAutoSuggestBox);
    private const string TextTokensCounter = nameof(TextTokensCounter);
    private const string QueryButton = nameof(QueryButton);

    private Microsoft.UI.Xaml.Controls.AutoSuggestBox? autoSuggestBox;
    private TextBox? autoSuggestTextBox;
    private bool isSelectedFocusOnFirstCharacter;
    private bool isSelectedFocusOnLastCharacter;

    public TokenizingTextBoxItem()
    {
        DefaultStyleKey = typeof(TokenizingTextBoxItem);

        KeyDown += OnKeyDown;
    }

    public event TypedEventHandler<TokenizingTextBoxItem, RoutedEventArgs>? AutoSuggestTextBoxLoaded;

    public event TypedEventHandler<TokenizingTextBoxItem, RoutedEventArgs>? ClearAllAction;

    public Microsoft.UI.Xaml.Controls.AutoSuggestBox? AutoSuggestBox
    {
        get => autoSuggestBox;
    }

    public TextBox? AutoSuggestTextBox
    {
        get => autoSuggestTextBox;
    }

    public bool UseCharacterAsUser { get; set; }

    private bool IsAllSelected
    {
        get => autoSuggestTextBox?.SelectedText == autoSuggestTextBox?.Text && !string.IsNullOrEmpty(autoSuggestTextBox?.Text);
    }

    private bool IsCaretAtStart
    {
        get => autoSuggestTextBox?.SelectionStart is 0;
    }

    private bool IsCaretAtEnd
    {
        get => autoSuggestTextBox?.SelectionStart == autoSuggestTextBox?.Text.Length || autoSuggestTextBox?.SelectionStart + autoSuggestTextBox?.SelectionLength == autoSuggestTextBox?.Text.Length;
    }

    public void UpdateText(string text)
    {
        if (autoSuggestBox is not null)
        {
            autoSuggestBox.Text = text;
            return;
        }

        void WaitForLoad(object s, RoutedEventArgs eargs)
        {
            if (autoSuggestTextBox is not null)
            {
                autoSuggestTextBox.Text = text;
            }

            AutoSuggestTextBoxLoaded -= WaitForLoad;
        }

        AutoSuggestTextBoxLoaded += WaitForLoad;
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(TextAutoSuggestBox) is Microsoft.UI.Xaml.Controls.AutoSuggestBox suggestbox)
        {
            OnASBApplyTemplate(suggestbox);
        }
    }

    private void OnASBApplyTemplate(Microsoft.UI.Xaml.Controls.AutoSuggestBox asb)
    {
        if (autoSuggestBox is not null)
        {
            autoSuggestBox.Loaded -= OnASBLoaded;

            autoSuggestBox.QuerySubmitted -= OnASBQuerySubmitted;
            autoSuggestBox.SuggestionChosen -= OnASBSuggestionChosen;
            autoSuggestBox.TextChanged -= OnASBTextChanged;
            autoSuggestBox.PointerEntered -= OnASBPointerEntered;
            autoSuggestBox.PointerExited -= OnASBPointerExited;
            autoSuggestBox.PointerCanceled -= OnASBPointerExited;
            autoSuggestBox.PointerCaptureLost -= OnASBPointerExited;
            autoSuggestBox.GotFocus -= OnASBGotFocus;
            autoSuggestBox.LostFocus -= OnASBLostFocus;

            // Remove any previous QueryIcon
            autoSuggestBox.QueryIcon = default;
        }

        autoSuggestBox = asb;

        if (autoSuggestBox is not null)
        {
            autoSuggestBox.Loaded += OnASBLoaded;

            autoSuggestBox.QuerySubmitted += OnASBQuerySubmitted;
            autoSuggestBox.SuggestionChosen += OnASBSuggestionChosen;
            autoSuggestBox.TextChanged += OnASBTextChanged;
            autoSuggestBox.PointerEntered += OnASBPointerEntered;
            autoSuggestBox.PointerExited += OnASBPointerExited;
            autoSuggestBox.PointerCanceled += OnASBPointerExited;
            autoSuggestBox.PointerCaptureLost += OnASBPointerExited;
            autoSuggestBox.GotFocus += OnASBGotFocus;
            autoSuggestBox.LostFocus += OnASBLostFocus;

            // Setup a binding to the QueryIcon of the Parent if we're the last box.
            if (Content is ITokenStringContainer str)
            {
                autoSuggestBox.Text = str.Text;

                if (str.IsLast)
                {
                    if (Owner.QueryIcon is FontIconSource fis && fis.ReadLocalValue(FontIconSource.FontSizeProperty) == DependencyProperty.UnsetValue)
                    {
                        fis.FontSize = 16;

                        if (Owner.TryFindResource("TokenizingTextBoxIconFontSize", out object? fontSizeObj) && fontSizeObj is double fontSize)
                        {
                            fis.FontSize = fontSize;
                        }
                    }

                    Binding iconBinding = new()
                    {
                        Source = Owner,
                        Path = new(nameof(Owner.QueryIcon)),
                        RelativeSource = new()
                        {
                            Mode = RelativeSourceMode.TemplatedParent,
                        },
                    };

                    IconSourceElement iconSourceElement = new();
                    iconSourceElement.SetBinding(IconSourceElement.IconSourceProperty, iconBinding);
                    autoSuggestBox.QueryIcon = iconSourceElement;
                }
            }
        }
    }

    private void OnASBGotFocus(object sender, RoutedEventArgs e)
    {
        // Verify if the usual behavior of clearing token selection is required
        if (!Owner.PauseTokenClearOnFocus && !TokenizingTextBox.IsShiftPressed)
        {
            Owner.DeselectAll();
        }

        Owner.PauseTokenClearOnFocus = false;

        VisualStateManager.GoToState(Owner, TokenizingTextBox.FocusedState, true);
    }

    private void OnASBLoaded(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(autoSuggestBox);
        if (autoSuggestBox.FindDescendant(QueryButton) is Button queryButton)
        {
            queryButton.Visibility = Owner.QueryIcon is not null ? Visibility.Visible : Visibility.Collapsed;
        }

        if (autoSuggestTextBox is not null)
        {
            autoSuggestTextBox.PreviewKeyDown -= OnAutoSuggestTextBoxPreviewKeyDown;
            autoSuggestTextBox.TextChanging -= OnAutoSuggestTextBoxTextChanging;
            autoSuggestTextBox.SelectionChanged -= OnAutoSuggestTextBoxSelectionChanged;
            autoSuggestTextBox.SelectionChanging -= OnAutoSuggestTextBoxSelectionChanging;
        }

        autoSuggestTextBox = autoSuggestBox.FindDescendant<TextBox>();

        if (autoSuggestTextBox is not null)
        {
            autoSuggestTextBox.PreviewKeyDown += OnAutoSuggestTextBoxPreviewKeyDown;
            autoSuggestTextBox.TextChanging += OnAutoSuggestTextBoxTextChanging;
            autoSuggestTextBox.SelectionChanged += OnAutoSuggestTextBoxSelectionChanged;
            autoSuggestTextBox.SelectionChanging += OnAutoSuggestTextBoxSelectionChanging;

            AutoSuggestTextBoxLoaded?.Invoke(this, e);
        }

        Owner.RefreshTokenCounter();
    }

    private void OnASBLostFocus(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(Owner, TokenizingTextBox.UnfocusedState, true);
    }

    private void OnASBPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(Owner, TokenizingTextBox.PointerOverState, true);
    }

    private void OnASBPointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(Owner, TokenizingTextBox.NormalState, true);
    }

    private void OnASBQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        Owner.OnQuerySubmitted(sender, args);

        if (args.ChosenSuggestion is not null)
        {
            AddToken(args.ChosenSuggestion);
            return;
        }

        if (!string.IsNullOrWhiteSpace(args.QueryText))
        {
            AddToken(args.QueryText);
            return;
        }

        void AddToken(object data)
        {
            Owner.AddToken(data);
            sender.Text = Owner.Text = string.Empty;
            sender.Focus(FocusState.Programmatic);
        }
    }

    private void OnASBSuggestionChosen(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        Owner.OnSuggestionsChosen(sender, args);
    }

    private void OnASBTextChanged(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (sender.Text is null)
        {
            return;
        }

        if (!EqualityComparer<string>.Default.Equals(sender.Text, Owner.Text))
        {
            Owner.Text = sender.Text;
        }

        // Override our programmatic manipulation as we're redirecting input for the user
        if (UseCharacterAsUser)
        {
            UseCharacterAsUser = false;

            args.Reason = AutoSuggestionBoxTextChangeReason.UserInput;
        }

        Owner.OnTextChanged(sender, args);

        string t = sender.Text?.Trim() ?? string.Empty;

        // Look for Token Delimiters to create new tokens when text changes.
        if (!string.IsNullOrEmpty(Owner.TokenDelimiter) && t.Contains(Owner.TokenDelimiter, StringComparison.OrdinalIgnoreCase))
        {
            bool lastDelimited = t[^1] == Owner.TokenDelimiter[0];

            ReadOnlySpan<string> tokens = t.Split(Owner.TokenDelimiter);
            int numberToProcess = lastDelimited ? tokens.Length : tokens.Length - 1;
            for (int position = 0; position < numberToProcess; position++)
            {
                string token = tokens[position].Trim();
                if (token.Length > 0)
                {
                    Owner.AddToken(token);
                }
            }

            sender.Text = lastDelimited ? string.Empty : tokens[^1].Trim();
        }
    }

    private void OnAutoSuggestTextBoxPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (sender is not TextBox autoSuggestTextBox)
        {
            return;
        }

        if (IsCaretAtStart && (e.Key is VirtualKey.Back or VirtualKey.Left))
        {
            // if the back key is pressed and there is any selection in the text box then the text box can handle it
            if ((e.Key is VirtualKey.Left && isSelectedFocusOnFirstCharacter) || autoSuggestTextBox.SelectionLength is 0)
            {
                if (Owner.SelectPreviousItem(this))
                {
                    if (!TokenizingTextBox.IsShiftPressed)
                    {
                        // Clear any text box selection
                        autoSuggestTextBox.SelectionLength = 0;
                    }

                    e.Handled = true;
                }
            }
        }
        else if (IsCaretAtEnd && e.Key is VirtualKey.Right)
        {
            // if the back key is pressed and there is any selection in the text box then the text box can handle it
            if (isSelectedFocusOnLastCharacter || autoSuggestTextBox.SelectionLength is 0)
            {
                if (Owner.SelectNextItem(this))
                {
                    if (!TokenizingTextBox.IsShiftPressed)
                    {
                        // Clear any text box selection
                        autoSuggestTextBox.SelectionLength = 0;
                    }

                    e.Handled = true;
                }
            }
        }
        else if (e.Key is VirtualKey.A && TokenizingTextBox.IsControlPressed)
        {
            // Need to provide this shortcut from the textbox only, as ListViewBase will do it for us on token.
            Owner.SelectAllTokensAndText();
        }
    }

    private void OnAutoSuggestTextBoxSelectionChanged(object sender, RoutedEventArgs args)
    {
        if (!(IsAllSelected || TokenizingTextBox.IsShiftPressed || Owner.IsClearingForClick))
        {
            Owner.DeselectAllTokensAndText(this);
        }

        Owner.IsClearingForClick = false;
    }

    private void OnAutoSuggestTextBoxSelectionChanging(TextBox sender, TextBoxSelectionChangingEventArgs args)
    {
        isSelectedFocusOnFirstCharacter = args.SelectionLength > 0 && args.SelectionStart is 0 && sender.SelectionStart > 0;
        isSelectedFocusOnLastCharacter = (args.SelectionStart + args.SelectionLength == sender.Text.Length) &&
            (sender.SelectionStart + sender.SelectionLength != sender.Text.Length);
    }

    private void OnAutoSuggestTextBoxTextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
    {
        if (Owner.SelectedItems.Count > 1)
        {
            Owner.RemoveAllSelectedTokens();
        }
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (Content is ITokenStringContainer)
        {
            return;
        }

        switch (e.Key)
        {
            case VirtualKey.Back:
            case VirtualKey.Delete:
                {
                    ClearAllAction?.Invoke(this, e);
                    break;
                }
        }
    }
}
