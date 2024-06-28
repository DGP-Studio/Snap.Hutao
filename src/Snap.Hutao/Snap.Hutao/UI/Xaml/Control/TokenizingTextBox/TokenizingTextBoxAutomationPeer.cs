// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.Control.TokenizingTextBox;

internal class TokenizingTextBoxAutomationPeer : ListViewBaseAutomationPeer, IValueProvider
{
    public TokenizingTextBoxAutomationPeer(TokenizingTextBox owner)
        : base(owner)
    {
    }

    public bool IsReadOnly => !OwningTokenizingTextBox.IsEnabled;

    public string Value => OwningTokenizingTextBox.Text;

    private TokenizingTextBox OwningTokenizingTextBox
    {
        get => (TokenizingTextBox)Owner;
    }

    public void SetValue(string value)
    {
        if (IsReadOnly)
        {
            throw new ElementNotEnabledException($"Could not set the value of the {nameof(TokenizingTextBox)} ");
        }

        OwningTokenizingTextBox.Text = value;
    }

    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    protected override string GetNameCore()
    {
        string name = OwningTokenizingTextBox.Name;
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        name = AutomationProperties.GetName(OwningTokenizingTextBox);
        return !string.IsNullOrWhiteSpace(name) ? name : base.GetNameCore();
    }

    protected override object GetPatternCore(PatternInterface patternInterface)
    {
        return patternInterface switch
        {
            PatternInterface.Value => this,
            _ => base.GetPatternCore(patternInterface),
        };
    }

    protected override IList<AutomationPeer> GetChildrenCore()
    {
        TokenizingTextBox owner = OwningTokenizingTextBox;

        ItemCollection items = owner.Items;
        if (items.Count <= 0)
        {
            return default!;
        }

        List<AutomationPeer> peers = new(items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            if (owner.ContainerFromIndex(i) is TokenizingTextBoxItem element)
            {
                peers.Add(FromElement(element) ?? CreatePeerForElement(element));
            }
        }

        return peers;
    }
}
