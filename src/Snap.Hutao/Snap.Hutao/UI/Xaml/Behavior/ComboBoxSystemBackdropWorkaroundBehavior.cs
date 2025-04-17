// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Hosting;
using Snap.Hutao.Core;
using Snap.Hutao.UI.Xaml.Control.Theme;
using System.Numerics;
using WinRT;

namespace Snap.Hutao.UI.Xaml.Behavior;

[SuppressMessage("", "CA1001")]
internal sealed class ComboBoxSystemBackdropWorkaroundBehavior : BehaviorBase<ComboBox>
{
    private Popup? popup;
    private ContentExternalBackdropLink? backdropLink;
    private DesktopAcrylicController? desktopAcrylicController;
    private SystemBackdropConfiguration? systemBackdropConfiguration;
    private bool connected;

    protected override bool Initialize()
    {
        ComboBox comboBox = AssociatedObject;
        if (comboBox.FindDescendant("Popup") is not Popup popup)
        {
            return false;
        }

        popup.Opened += OnPopupOpened;
        popup.ActualThemeChanged += OnPopupActualThemeChanged;

        if (!comboBox.IsEditable)
        {
            comboBox.IsDropDownOpen = true;
        }

        return true;
    }

    protected override bool Uninitialize()
    {
        if (popup is not null)
        {
            popup.Opened -= OnPopupOpened;
            popup.ActualThemeChanged -= OnPopupActualThemeChanged;
        }

        DisposableMarshal.DisposeAndClear(ref backdropLink);
        DisposableMarshal.DisposeAndClear(ref desktopAcrylicController);

        return base.Uninitialize();
    }

    private void OnPopupOpened(object? sender, object e)
    {
        if (sender is not Popup popup)
        {
            return;
        }

        this.popup = popup;

        if (popup.FindName("PopupBorder") is not Border border)
        {
            return;
        }

        Vector2 size = border.ActualSize;
        Compositor compositor = ElementCompositionPreview.GetElementVisual(border).Compositor;
        Vector2 cornerRadius = new(8, 8);

        if (!connected)
        {
            connected = true;

            UIElement child = border.Child;
            Grid rootGrid = new();
            border.Child = rootGrid;
            Grid visualGrid = new();
            rootGrid.Children.Add(visualGrid);
            rootGrid.Children.Add(child);

            backdropLink = ContentExternalBackdropLink.Create(compositor);
            backdropLink.ExternalBackdropBorderMode = CompositionBorderMode.Soft;

            // Modify PlacementVisual
            Visual placementVisual = backdropLink.PlacementVisual;
            placementVisual.Size = size;
            placementVisual.Clip = compositor.CreateRectangleClip(0, 0, size.X, size.Y, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
            placementVisual.BorderMode = CompositionBorderMode.Soft;

            ElementCompositionPreview.SetElementChildVisual(visualGrid, placementVisual);

            systemBackdropConfiguration = new()
            {
                IsInputActive = true,
                Theme = ThemeHelper.ElementToSystemBackdrop(popup.ActualTheme),
            };

            desktopAcrylicController = new();
            desktopAcrylicController.SetSystemBackdropConfiguration(systemBackdropConfiguration);
            desktopAcrylicController.AddSystemBackdropTarget(backdropLink.As<ICompositionSupportsSystemBackdrop>());

            popup.IsOpen = false;
        }
        else if (backdropLink is not null && systemBackdropConfiguration is not null)
        {
            // Update PlacementVisual
            Visual placementVisual = backdropLink.PlacementVisual;
            placementVisual.Size = size;
            placementVisual.Clip = compositor.CreateRectangleClip(0, 0, size.X, size.Y, cornerRadius, cornerRadius, cornerRadius, cornerRadius);
        }
    }

    private void OnPopupActualThemeChanged(FrameworkElement sender, object args)
    {
        if (systemBackdropConfiguration is not null)
        {
            systemBackdropConfiguration.Theme = ThemeHelper.ElementToSystemBackdrop(sender.ActualTheme);
        }
    }
}