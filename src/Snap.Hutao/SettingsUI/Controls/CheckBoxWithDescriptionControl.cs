// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;

namespace SettingsUI.Controls;

public class CheckBoxWithDescriptionControl : CheckBox
{
    private readonly CheckBoxWithDescriptionControl _checkBoxSubTextControl;

    public CheckBoxWithDescriptionControl()
    {
        _checkBoxSubTextControl = this;
        Loaded += CheckBoxSubTextControl_Loaded;
    }

    protected override void OnApplyTemplate()
    {
        Update();
        base.OnApplyTemplate();
    }

    private void Update()
    {
        if (!string.IsNullOrEmpty(Header))
        {
            AutomationProperties.SetName(this, Header);
        }
    }

    private void CheckBoxSubTextControl_Loaded(object sender, RoutedEventArgs e)
    {
        StackPanel panel = new() { Orientation = Orientation.Vertical };

        // Add text box only if the description is not empty. Required for additional plugin options.
        if (!string.IsNullOrWhiteSpace(Description))
        {
            panel.Children.Add(new TextBlock() { Margin = new Thickness(0, 10, 0, 0), Text = Header });
            //Style = (Style)Application.Current.Resources["SecondaryIsEnabledTextBlockStyle"]
            panel.Children.Add(new IsEnabledTextBlock() { Text = Description });
        }
        else
        {
            panel.Children.Add(new TextBlock() { Margin = new Thickness(0, 0, 0, 0), Text = Header });
        }

        _checkBoxSubTextControl.Content = panel;
    }

    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        "Header",
        typeof(string),
        typeof(CheckBoxWithDescriptionControl),
        new PropertyMetadata(default(string)));

    public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
        "Description",
        typeof(object),
        typeof(CheckBoxWithDescriptionControl),
        new PropertyMetadata(default(string)));

    [Localizable(true)]
    public string Header
    {
        get => (string)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    [Localizable(true)]
    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }
}
