// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control.SuggestBox;

namespace Snap.Hutao.View.Control;

[DependencyProperty("FilterCommand", typeof(ICommand))]
[DependencyProperty("ITokenizableItemsSource", typeof(IEnumerable<ITokenizable>))]
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
        if (SuggestedItemsSource is not IEnumerable<string> availableQueries)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Text))
        {
            return;
        }

        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = availableQueries.Where(q => q.Contains(Text, StringComparison.OrdinalIgnoreCase));
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

        if (ITokenizableItemsSource is null)
        {
            return;
        }

        if (ITokenizableItemsSource.SingleOrDefault(i => i.Name == args.TokenText) is { } item)
        {
            args.Item = item.Tokenize();
            return;
        }

        args.Item = new SearchToken(args.TokenText);
    }

    private void OnTokenItemModified(TokenizingTextBox sender, object args)
    {
        if (FilterCommand.CanExecute(null))
        {
            FilterCommand.Execute(null);
        }
    }
}