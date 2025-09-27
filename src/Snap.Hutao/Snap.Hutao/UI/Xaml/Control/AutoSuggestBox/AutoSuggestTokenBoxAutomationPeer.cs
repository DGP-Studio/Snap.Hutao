// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control.AutoSuggestBox;

internal sealed partial class AutoSuggestTokenBoxAutomationPeer : ListViewBaseAutomationPeer, IValueProvider
{
    public AutoSuggestTokenBoxAutomationPeer(AutoSuggestTokenBox owner)
        : base(owner)
    {
    }

    public bool IsReadOnly { get => !OwningAutoSuggestTokenBox.IsEnabled; }

    public string? Value { get => OwningAutoSuggestTokenBox.Text; }

    private AutoSuggestTokenBox OwningAutoSuggestTokenBox { get => Owner.As<AutoSuggestTokenBox>(); }

    public void SetValue(string value)
    {
        if (IsReadOnly)
        {
            throw new ElementNotEnabledException($"Could not set the value of the {nameof(AutoSuggestTokenBox)} ");
        }

        OwningAutoSuggestTokenBox.Text = value;
    }

    protected override string GetClassNameCore()
    {
        return Owner.GetType().Name;
    }

    protected override string GetNameCore()
    {
        string name = OwningAutoSuggestTokenBox.Name;
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        name = AutomationProperties.GetName(OwningAutoSuggestTokenBox);
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
        AutoSuggestTokenBox owner = OwningAutoSuggestTokenBox;

        ItemCollection items = owner.Items;
        if (items.Count <= 0)
        {
            return default!;
        }

        List<AutomationPeer> peers = new(items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            if (owner.ContainerFromIndex(i) is AutoSuggestTokenBoxItem element)
            {
                peers.Add(FromElement(element) ?? CreatePeerForElement(element));
            }
        }

        return peers;
    }
}