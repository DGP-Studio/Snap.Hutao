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

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

[DependencyProperty<Style>("ClearButtonStyle")]
[DependencyProperty<AutoSuggestTokenBox>("Owner", NotNull = true)]
[TemplatePart(Name = TokenRemoveButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = TextAutoSuggestBox, Type = typeof(Microsoft.UI.Xaml.Controls.AutoSuggestBox))]
[TemplatePart(Name = TextTokensCounter, Type = typeof(Microsoft.UI.Xaml.Controls.TextBlock))]
internal partial class AutoSuggestTokenBoxItem : ListViewItem
{
    private const string TokenRemoveButton = nameof(TokenRemoveButton);
    private const string TextAutoSuggestBox = nameof(TextAutoSuggestBox);
    private const string TextTokensCounter = nameof(TextTokensCounter);
    private const string QueryButton = nameof(QueryButton);

    private bool isSelectedFocusOnFirstCharacter;
    private bool isSelectedFocusOnLastCharacter;

    public AutoSuggestTokenBoxItem()
    {
        DefaultStyleKey = typeof(AutoSuggestTokenBoxItem);

        KeyDown += OnKeyDown;
    }

    public event TypedEventHandler<AutoSuggestTokenBoxItem, RoutedEventArgs>? AutoSuggestTextBoxLoaded;

    public event TypedEventHandler<AutoSuggestTokenBoxItem, RoutedEventArgs>? ClearAllAction;

    public Microsoft.UI.Xaml.Controls.AutoSuggestBox? AutoSuggestBox { get; private set; }

    public TextBox? AutoSuggestTextBox { get; private set; }

    public bool UseCharacterAsUser { get; set; }

    private bool IsAllSelected
    {
        get => AutoSuggestTextBox?.SelectedText == AutoSuggestTextBox?.Text && !string.IsNullOrEmpty(AutoSuggestTextBox?.Text);
    }

    private bool IsCaretAtStart
    {
        get => AutoSuggestTextBox?.SelectionStart is 0;
    }

    private bool IsCaretAtEnd
    {
        get => AutoSuggestTextBox?.SelectionStart == AutoSuggestTextBox?.Text.Length || AutoSuggestTextBox?.SelectionStart + AutoSuggestTextBox?.SelectionLength == AutoSuggestTextBox?.Text.Length;
    }

    public void UpdateText(string text)
    {
        if (AutoSuggestBox is not null)
        {
            AutoSuggestBox.Text = text;
            return;
        }

        AutoSuggestTextBoxLoaded += WaitForLoad;

        void WaitForLoad(object s, RoutedEventArgs eargs)
        {
            AutoSuggestTextBox?.Text = text;

            AutoSuggestTextBoxLoaded -= WaitForLoad;
        }
    }

    /// <inheritdoc/>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (GetTemplateChild(TextAutoSuggestBox) is Microsoft.UI.Xaml.Controls.AutoSuggestBox suggestbox)
        {
            OnAutoSuggestBoxApplyTemplate(suggestbox);
        }
    }

    private void OnAutoSuggestBoxApplyTemplate(Microsoft.UI.Xaml.Controls.AutoSuggestBox asb)
    {
        // Revoke previous events
        if (AutoSuggestBox is not null)
        {
            AutoSuggestBox.Loaded -= OnAutoSuggestBoxLoaded;

            AutoSuggestBox.QuerySubmitted -= OnAutoSuggestBoxQuerySubmitted;
            AutoSuggestBox.SuggestionChosen -= OnAutoSuggestBoxSuggestionChosen;
            AutoSuggestBox.TextChanged -= OnAutoSuggestBoxTextChanged;
            AutoSuggestBox.PointerEntered -= OnAutoSuggestBoxPointerEntered;
            AutoSuggestBox.PointerExited -= OnAutoSuggestBoxPointerExited;
            AutoSuggestBox.PointerCanceled -= OnAutoSuggestBoxPointerExited;
            AutoSuggestBox.PointerCaptureLost -= OnAutoSuggestBoxPointerExited;
            AutoSuggestBox.GotFocus -= OnAutoSuggestBoxGotFocus;
            AutoSuggestBox.LostFocus -= OnAutoSuggestBoxLostFocus;

            // Remove any previous QueryIcon
            AutoSuggestBox.QueryIcon = default;
        }

        AutoSuggestBox = asb;

        if (AutoSuggestBox is not null)
        {
            AutoSuggestBox.Loaded += OnAutoSuggestBoxLoaded;

            AutoSuggestBox.QuerySubmitted += OnAutoSuggestBoxQuerySubmitted;
            AutoSuggestBox.SuggestionChosen += OnAutoSuggestBoxSuggestionChosen;
            AutoSuggestBox.TextChanged += OnAutoSuggestBoxTextChanged;
            AutoSuggestBox.PointerEntered += OnAutoSuggestBoxPointerEntered;
            AutoSuggestBox.PointerExited += OnAutoSuggestBoxPointerExited;
            AutoSuggestBox.PointerCanceled += OnAutoSuggestBoxPointerExited;
            AutoSuggestBox.PointerCaptureLost += OnAutoSuggestBoxPointerExited;
            AutoSuggestBox.GotFocus += OnAutoSuggestBoxGotFocus;
            AutoSuggestBox.LostFocus += OnAutoSuggestBoxLostFocus;

            // Setup a binding to the QueryIcon of the Parent if we're the last box.
            if (Content is ITokenStringContainer str)
            {
                AutoSuggestBox.Text = str.Text;

                if (str.IsLast)
                {
                    if (Owner.QueryIcon is FontIconSource fis && fis.ReadLocalValue(FontIconSource.FontSizeProperty) == DependencyProperty.UnsetValue)
                    {
                        fis.FontSize = 16;

                        if (Owner.TryFindResource("AutoSuggestTokenBoxIconFontSize", out object? fontSizeObj) && fontSizeObj is double fontSize)
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
                    AutoSuggestBox.QueryIcon = iconSourceElement;
                }
            }
        }
    }

    private void OnAutoSuggestBoxGotFocus(object sender, RoutedEventArgs e)
    {
        // Verify if the usual behavior of clearing token selection is required
        if (!Owner.PauseTokenClearOnFocus && !AutoSuggestTokenBox.IsShiftPressed)
        {
            Owner.DeselectAll();
        }

        Owner.PauseTokenClearOnFocus = false;
        VisualStateManager.GoToState(Owner, AutoSuggestTokenBox.FocusedState, true);
    }

    private void OnAutoSuggestBoxLoaded(object sender, RoutedEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(AutoSuggestBox);
        if (AutoSuggestBox.FindDescendant(QueryButton) is Button queryButton)
        {
            queryButton.Visibility = Owner.QueryIcon is not null ? Visibility.Visible : Visibility.Collapsed;
        }

        if (AutoSuggestTextBox is not null)
        {
            AutoSuggestTextBox.PreviewKeyDown -= OnAutoSuggestTextBoxPreviewKeyDown;
            AutoSuggestTextBox.TextChanging -= OnAutoSuggestTextBoxTextChanging;
            AutoSuggestTextBox.SelectionChanged -= OnAutoSuggestTextBoxSelectionChanged;
            AutoSuggestTextBox.SelectionChanging -= OnAutoSuggestTextBoxSelectionChanging;
        }

        AutoSuggestTextBox = AutoSuggestBox.FindDescendant<TextBox>();

        if (AutoSuggestTextBox is not null)
        {
            AutoSuggestTextBox.PreviewKeyDown += OnAutoSuggestTextBoxPreviewKeyDown;
            AutoSuggestTextBox.TextChanging += OnAutoSuggestTextBoxTextChanging;
            AutoSuggestTextBox.SelectionChanged += OnAutoSuggestTextBoxSelectionChanged;
            AutoSuggestTextBox.SelectionChanging += OnAutoSuggestTextBoxSelectionChanging;

            AutoSuggestTextBoxLoaded?.Invoke(this, e);
        }

        Owner.RefreshTokenCounter();
    }

    private void OnAutoSuggestBoxLostFocus(object sender, RoutedEventArgs e)
    {
        VisualStateManager.GoToState(Owner, AutoSuggestTokenBox.UnfocusedState, true);
    }

    private void OnAutoSuggestBoxPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(Owner, AutoSuggestTokenBox.PointerOverState, true);
    }

    private void OnAutoSuggestBoxPointerExited(object sender, PointerRoutedEventArgs e)
    {
        VisualStateManager.GoToState(Owner, AutoSuggestTokenBox.NormalState, true);
    }

    private void OnAutoSuggestBoxQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
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
        }

        void AddToken(object data)
        {
            Owner.AddToken(data);
            sender.Text = Owner.Text = string.Empty;
            sender.Focus(FocusState.Programmatic);
        }
    }

    private void OnAutoSuggestBoxSuggestionChosen(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
    {
        Owner.OnSuggestionsChosen(sender, args);
    }

    private void OnAutoSuggestBoxTextChanged(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
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

        if (IsCaretAtStart && e.Key is VirtualKey.Back or VirtualKey.Left)
        {
            // if the back key is pressed and there is any selection in the text box then the text box can handle it
            if ((e.Key is VirtualKey.Left && isSelectedFocusOnFirstCharacter) || autoSuggestTextBox.SelectionLength is 0)
            {
                if (Owner.SelectPreviousItem(this))
                {
                    if (!AutoSuggestTokenBox.IsShiftPressed)
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
                    if (!AutoSuggestTokenBox.IsShiftPressed)
                    {
                        // Clear any text box selection
                        autoSuggestTextBox.SelectionLength = 0;
                    }

                    e.Handled = true;
                }
            }
        }
        else if (e.Key is VirtualKey.A && AutoSuggestTokenBox.IsControlPressed)
        {
            // Need to provide this shortcut from the textbox only, as ListViewBase will do it for us on token.
            Owner.SelectAllTokensAndText();
        }
    }

    private void OnAutoSuggestTextBoxSelectionChanged(object sender, RoutedEventArgs args)
    {
        if (!(IsAllSelected || AutoSuggestTokenBox.IsShiftPressed || Owner.IsClearingForClick))
        {
            Owner.DeselectAllTokensAndText(this);
        }

        Owner.IsClearingForClick = false;
    }

    private void OnAutoSuggestTextBoxSelectionChanging(TextBox sender, TextBoxSelectionChangingEventArgs args)
    {
        isSelectedFocusOnFirstCharacter = args is { SelectionLength: > 0, SelectionStart: 0 } && sender.SelectionStart > 0;
        isSelectedFocusOnLastCharacter = args.SelectionStart + args.SelectionLength == sender.Text.Length
            && sender.SelectionStart + sender.SelectionLength != sender.Text.Length;
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