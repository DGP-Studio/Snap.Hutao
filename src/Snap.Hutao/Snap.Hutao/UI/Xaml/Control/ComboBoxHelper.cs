// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Snap.Hutao.Win32.Foundation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty("SystemBackdropWorkaround", typeof(bool), false, nameof(OnSystemBackdropWorkaroundChanged), IsAttached = true, AttachedType = typeof(Microsoft.UI.Xaml.Controls.ComboBox))]
public sealed partial class ComboBoxHelper
{
    private static readonly ConditionalWeakTable<Popup, DesktopAcrylicController> PopupDesktopAcrylicControllerTable = [];

    private static void OnSystemBackdropWorkaroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        bool isEnabled = (bool)args.NewValue;
        if (!isEnabled)
        {
            return;
        }

        if (sender is not ComboBox comboBox)
        {
            return;
        }

        comboBox.Loaded += OnComboBoxLoaded;
    }

    private static void OnComboBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not ComboBox comboBox)
        {
            return;
        }

        comboBox.Loaded -= OnComboBoxLoaded;

        if (comboBox.FindDescendant("Popup") is not Popup popup)
        {
            return;
        }

        popup.Opened += OnPopupOpened;
        if (!comboBox.IsEditable)
        {
            comboBox.IsDropDownOpen = true;
        }
    }

    private static void OnPopupOpened(object? sender, object e)
    {
        if (sender is not Popup popup)
        {
            return;
        }

        popup.Opened -= OnPopupOpened;

        if (popup.FindName("PopupBorder") is not Border border)
        {
            return;
        }

        Vector2 size = border.ActualSize;
        UIElement child = border.Child;

        Grid rootGrid = new()
        {
            Width = size.X,
            Height = size.Y,
        };

        border.Child = rootGrid;

        Grid visualGrid = new();
        rootGrid.Children.Add(visualGrid);
        rootGrid.Children.Add(child);

        Compositor compositor = ElementCompositionPreview.GetElementVisual(border).Compositor;
        ContentExternalBackdropLink link = ContentExternalBackdropLink.Create(compositor);

        link.ExternalBackdropBorderMode = CompositionBorderMode.Soft;

        // Modify PlacementVisual
        Visual placementVisual = link.PlacementVisual;
        placementVisual.Size = size;
        Vector2 cornerRadius = new(8, 8);
        placementVisual.Clip = compositor.CreateRectangleClip(0, 0, size.X, size.Y, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
        placementVisual.BorderMode = CompositionBorderMode.Soft;

        ElementCompositionPreview.GetElementVisual(visualGrid).As<ContainerVisual>().Children.InsertAtTop(placementVisual);

        DesktopAcrylicController controller = new();
        controller.AddSystemBackdropTarget(link.As<ICompositionSupportsSystemBackdrop>());
        controller.SetSystemBackdropConfiguration(new()
        {
            IsInputActive = true,
        });

        PopupDesktopAcrylicControllerTable.Add(popup, controller);
        GC.KeepAlive(link);
    }
}