// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.UI.Input;
using Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;
using System.Collections;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

[DependencyProperty("FilterCommand", typeof(ICommand))]
[DependencyProperty("FilterCommandParameter", typeof(object))]
[DependencyProperty("AvailableTokens", typeof(IReadOnlyDictionary<string, SearchToken>))]
internal sealed partial class AutoSuggestTokenBox : TokenizingTextBox.TokenizingTextBox
{
    public AutoSuggestTokenBox()
    {
        DefaultStyleKey = typeof(TokenizingTextBox.TokenizingTextBox);
    }

    public IEnumerable<SearchToken> Tokens
    {
        get => ((IList)ItemsSource).OfType<SearchToken>();
    }

    public override void OnTextChanged(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (IsTokenLimitReached())
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(Text))
        {
            sender.ItemsSource = AvailableTokens
                .ExceptBy(Tokens, kvp => kvp.Value)
                .OrderBy(kvp => kvp.Value.Kind)
                .ThenBy(kvp => kvp.Value.Order)
                .Select(kvp => kvp.Value);
        }

        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = AvailableTokens
                .ExceptBy(Tokens, kvp => kvp.Value)
                .Where(kvp => kvp.Value.Value.Contains(Text, StringComparison.OrdinalIgnoreCase))
                .OrderBy(kvp => kvp.Value.Kind)
                .ThenBy(kvp => kvp.Value.Order)
                .Select(kvp => kvp.Value)
                .DefaultIfEmpty(SearchToken.NotFound);
        }
    }

    public override void OnQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is not null)
        {
            return;
        }

        CommandInvocation.TryExecute(FilterCommand, FilterCommandParameter);
    }

    public override void OnTokenItemAdding(TokenizingTextBox.TokenizingTextBox sender, TokenItemAddingEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.TokenText))
        {
            return;
        }

        if (AvailableTokens.GetValueOrDefault(args.TokenText) is { } token)
        {
            args.Item = token;
        }
        else
        {
            args.Cancel = true;
        }
    }

    public override void OnTokenItemAdded(TokenizingTextBox.TokenizingTextBox sender, object args)
    {
        if (args is SearchToken { Kind: SearchTokenKind.None } token)
        {
            ((IList)ItemsSource).Remove(token);
        }

        base.OnTokenItemAdded(sender, args);

        FilterCommand.TryExecute(FilterCommandParameter);
    }

    public override void OnTokenItemRemoved(TokenizingTextBox.TokenizingTextBox sender, object args)
    {
        base.OnTokenItemRemoved(sender, args);
        FilterCommand.TryExecute(FilterCommandParameter);
    }
}