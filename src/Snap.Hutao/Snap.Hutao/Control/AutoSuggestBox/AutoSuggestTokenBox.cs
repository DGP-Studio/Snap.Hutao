// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
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
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (this.FindDescendant("SuggestionsPopup") is Popup { Child: Border { Child: ListView listView } border })
        {
            IAppResourceProvider appResourceProvider = Ioc.Default.GetRequiredService<IAppResourceProvider>();
            listView.Background = null;
            listView.Margin = appResourceProvider.GetResource<Thickness>("AutoSuggestListPadding");

            border.Background = appResourceProvider.GetResource<Microsoft.UI.Xaml.Media.Brush>("AutoSuggestBoxSuggestionsListBackground");
            border.CornerRadius = new(0, 0, 8, 8);
        }

        if (this.FindDescendant("PART_AutoSuggestBox") is Microsoft.UI.Xaml.Controls.AutoSuggestBox autoSuggestBox)
        {
            autoSuggestBox.GotFocus += OnSuggestBoxFocusGot;
            autoSuggestBox.LosingFocus += OnSuggestBoxFocusLosing;
        }
    }

    private void OnSuggestBoxFocusGot(object sender, RoutedEventArgs e)
    {
        if (sender is Microsoft.UI.Xaml.Controls.AutoSuggestBox autoSuggestBox)
        {
            autoSuggestBox.ItemsSource = AvailableTokens.Values;
        }
    }

    private void OnSuggestBoxFocusLosing(object sender, RoutedEventArgs e)
    {
        if (sender is Microsoft.UI.Xaml.Controls.AutoSuggestBox autoSuggestBox)
        {
            autoSuggestBox.ItemsSource = null;
        }
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

        args.Item = AvailableTokens.GetValueOrDefault(args.TokenText) ?? new SearchToken(SearchTokenKind.None, args.TokenText);
    }

    private void OnTokenItemModified(TokenizingTextBox sender, object args)
    {
        CommandInvocation.TryExecute(FilterCommand, FilterCommandParameter);
    }
}