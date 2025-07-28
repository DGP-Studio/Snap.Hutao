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
internal sealed class ComboBoxDesktopAcrylicWorkaroundBehavior : BehaviorBase<ComboBox>
{
    private readonly Lock syncRoot = new();
    private Popup? popup;
    private ContentExternalBackdropLink? backdropLink;
    private DesktopAcrylicController? desktopAcrylicController;
    private SystemBackdropConfiguration? systemBackdropConfiguration;
    private Grid? visualGrid;
    private bool connected;
    private bool initialized;

    protected override bool Initialize()
    {
        lock (syncRoot)
        {
            if (initialized)
            {
                return true;
            }

            initialized = true;
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
    }

    protected override bool Uninitialize()
    {
        lock (syncRoot)
        {
            if (!initialized)
            {
                return true;
            }

            initialized = false;

            if (popup is not null && connected)
            {
                connected = false;
                popup.Opened -= OnPopupOpened;
                popup.ActualThemeChanged -= OnPopupActualThemeChanged;

                if (visualGrid is not null)
                {
                    ElementCompositionPreview.SetElementChildVisual(visualGrid, null);
                    visualGrid = null;
                }

                DisposableMarshal.DisposeAndClear(ref desktopAcrylicController);
                DisposableMarshal.DisposeAndClear(ref backdropLink);
            }

            return base.Uninitialize();
        }
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
        size = new(size.X - 2, size.Y - 2);
        CornerRadius cornerRadius = border.CornerRadius;
        Compositor compositor = ElementCompositionPreview.GetElementVisual(border).Compositor;

        if (!Interlocked.Exchange(ref connected, true))
        {
            UIElement originalChild = border.Child;
            Grid newRootGrid = new();
            border.Child = newRootGrid;
            visualGrid = new();
            newRootGrid.Children.Add(visualGrid);
            newRootGrid.Children.Add(originalChild);

            backdropLink = ContentExternalBackdropLink.Create(compositor);
            backdropLink.ExternalBackdropBorderMode = CompositionBorderMode.Soft;

            // Modify PlacementVisual
            Visual placementVisual = backdropLink.PlacementVisual;
            placementVisual.Size = size;
            placementVisual.Clip = compositor.CreateRectangleClip(size, cornerRadius);
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
            placementVisual.Clip = compositor.CreateRectangleClip(size, cornerRadius);
        }
    }

    private void OnPopupActualThemeChanged(FrameworkElement sender, object args)
    {
        if (systemBackdropConfiguration is null)
        {
            return;
        }

        systemBackdropConfiguration.Theme = ThemeHelper.ElementToSystemBackdrop(sender.ActualTheme);
    }
}