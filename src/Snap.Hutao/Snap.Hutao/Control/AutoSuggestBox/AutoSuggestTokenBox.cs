// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Control.Extension;

namespace Snap.Hutao.Control.AutoSuggestBox;

[DependencyProperty("FilterCommand", typeof(ICommand))]
[DependencyProperty("FilterCommandParameter", typeof(object))]
[DependencyProperty("AvailableTokens", typeof(IReadOnlyDictionary<string, SearchToken>))]
internal sealed partial class AutoSuggestTokenBox : TokenizingTextBox
{
    public AutoSuggestTokenBox()
    {
        DefaultStyleKey = typeof(TokenizingTextBox);
        TextChanged += OnFilterSuggestionRequested;
        QuerySubmitted += OnQuerySubmitted;
        TokenItemAdding += OnTokenItemAdding;
        TokenItemAdded += OnTokenItemModified;
        TokenItemRemoved += OnTokenItemModified;
    }

    private void OnFilterSuggestionRequested(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = AvailableTokens.Values.Where(q => q.Value.Contains(Text, StringComparison.OrdinalIgnoreCase));

            // TODO: CornerRadius
            // Popup? popup = this.FindDescendant("SuggestionsPopup") as Popup;
        }
    }

    private void OnQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is not null)
        {
            return;
        }

        CommandExtension.TryExecute(FilterCommand, FilterCommandParameter);
    }

    private void OnTokenItemAdding(TokenizingTextBox sender, TokenItemAddingEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.TokenText))
        {
            return;
        }

        args.Item = AvailableTokens.GetValueOrDefault(args.TokenText) ?? new SearchToken(SearchTokenKind.None, args.TokenText);
    }

    private void OnTokenItemModified(TokenizingTextBox sender, object args)
    {
        CommandExtension.TryExecute(FilterCommand, FilterCommandParameter);
    }
}