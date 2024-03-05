// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.SuggestBox;

namespace Snap.Hutao.View.Control;

[DependencyProperty("FilterCommand", typeof(ICommand))]
[DependencyProperty("AvailableTokens", typeof(IReadOnlyDictionary<string, SearchToken>))]
internal sealed partial class AutoSuggestTokenBox : TokenizingTextBox
{
    public AutoSuggestTokenBox()
    {
        TextChanged += OnFilterSuggestionRequested;
        QuerySubmitted += OnQuerySubmitted;
        TokenItemAdding += OnTokenItemAdding;
        TokenItemAdded += OnTokenItemModified;
        TokenItemRemoved += OnTokenItemModified;
    }

    private void OnFilterSuggestionRequested(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = AvailableTokens.Values.Where(q => q.Value.Contains(Text, StringComparison.OrdinalIgnoreCase));
        }
    }

    private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is not null)
        {
            return;
        }

        if (FilterCommand.CanExecute(null))
        {
            FilterCommand.Execute(null);
        }
    }

    private void OnTokenItemAdding(TokenizingTextBox sender, TokenItemAddingEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.TokenText))
        {
            return;
        }

        args.Item = AvailableTokens.GetValueOrDefault(args.TokenText) ?? new SearchToken(args.TokenText, SearchTokenKind.Others);
    }

    private void OnTokenItemModified(TokenizingTextBox sender, object args)
    {
        if (FilterCommand.CanExecute(null))
        {
            FilterCommand.Execute(null);
        }
    }
}