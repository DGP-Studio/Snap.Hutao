// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Snap.Hutao.Control.Extension;
using System.Collections;

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
        TokenItemAdded += OnTokenItemCollectionChanged;
        TokenItemRemoved += OnTokenItemCollectionChanged;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (this.FindDescendant("SuggestionsPopup") is Popup { Child: Border { Child: ListView listView } border })
        {
            IAppResourceProvider appResourceProvider = this.ServiceProvider().GetRequiredService<IAppResourceProvider>();

            listView.Background = null;
            listView.Margin = appResourceProvider.GetResource<Thickness>("AutoSuggestListPadding");

            border.Background = appResourceProvider.GetResource<Microsoft.UI.Xaml.Media.Brush>("AutoSuggestBoxSuggestionsListBackground");
            CornerRadius overlayCornerRadius = appResourceProvider.GetResource<CornerRadius>("OverlayCornerRadius");
            CornerRadiusFilterConverter cornerRadiusFilterConverter = new() { Filter = CornerRadiusFilterKind.Bottom };
            border.CornerRadius = (CornerRadius)cornerRadiusFilterConverter.Convert(overlayCornerRadius, typeof(CornerRadius), default, default);
        }
    }

    private void OnFilterSuggestionRequested(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (string.IsNullOrWhiteSpace(Text))
        {
            sender.ItemsSource = AvailableTokens
                .OrderBy(kvp => kvp.Value.Kind)
                .Select(kvp => kvp.Value);
        }

        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            sender.ItemsSource = AvailableTokens
                .Where(kvp => kvp.Value.Value.Contains(Text, StringComparison.OrdinalIgnoreCase))
                .OrderBy(kvp => kvp.Value.Kind)
                .Select(kvp => kvp.Value)
                .DefaultIfEmpty(SearchToken.NotFound);
        }
    }

    private void OnQuerySubmitted(Microsoft.UI.Xaml.Controls.AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion is not null)
        {
            return;
        }

        CommandInvocation.TryExecute(FilterCommand, FilterCommandParameter);
    }

    private void OnTokenItemAdding(TokenizingTextBox sender, TokenItemAddingEventArgs args)
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

    private void OnTokenItemCollectionChanged(TokenizingTextBox sender, object args)
    {
        if (args is SearchToken { Kind: SearchTokenKind.None } token)
        {
            ((IList)sender.ItemsSource).Remove(token);
        }

        FilterCommand.TryExecute(FilterCommandParameter);
    }
}