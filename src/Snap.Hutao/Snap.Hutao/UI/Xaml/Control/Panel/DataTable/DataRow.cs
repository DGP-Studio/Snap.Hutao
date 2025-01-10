// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Snap.Hutao.UI.Xaml.Control.Panel.DataTable;

internal sealed partial class DataRow : Microsoft.UI.Xaml.Controls.Panel
{
    // TODO: Create our own helper class here for the Header as well vs. straight-Grid.
    // TODO: WeakReference?
    private Microsoft.UI.Xaml.Controls.Panel? parentPanel;
    private DataTable? parentTable;

    private bool isTreeView;
    private double treePadding;

    public DataRow()
    {
        Unloaded += OnDataRowUnloaded;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // We should probably only have to do this once ever?
        parentPanel ??= InitializeParentHeaderConnection();

        double maxHeight = 0;

        if (Children.Count > 0)
        {
            // If we don't have a grid, just measure first child to get row height and take available space
            if (parentPanel is null)
            {
                Children[0].Measure(availableSize);
                return new(availableSize.Width, Children[0].DesiredSize.Height);
            }

            // Handle DataTable Parent
            if (parentTable is not null && parentTable.Children.Count == Children.Count)
            {
                // TODO: Need to check visibility
                // Measure all children since we need to determine the row's height at minimum
                for (int i = 0; i < Children.Count; i++)
                {
                    switch (parentTable.Children[i])
                    {
                        case DataColumn { CurrentWidth.GridUnitType: GridUnitType.Auto } col:
                            {
                                Children[i].Measure(availableSize);

                                // For TreeView in the first column, we want the header to expand to encompass
                                // the maximum indentation of the tree.
                                double padding = 0;
                                //// TODO: We only want/need to do this once? We may want to do if we're not an Auto column too...?
                                if (i is 0 && isTreeView)
                                {
                                    // Get our containing grid from TreeViewItem, start with our indented padding
                                    if (this.FindAscendant("MultiSelectGrid") is Grid parentContainer)
                                    {
                                        treePadding = parentContainer.Padding.Left;

                                        // We assume our 'DataRow' is in the last child slot of the Grid, need to know how large the other columns are.
                                        for (int j = 0; j < parentContainer.Children.Count - 1; j++)
                                        {
                                            // TODO: We may need to get the actual size here later in Arrange?
                                            treePadding += parentContainer.Children[j].DesiredSize.Width;
                                        }
                                    }

                                    padding = treePadding;
                                }

                                // TODO: Do we want this to ever shrink back?
                                double prev = col.MaxChildDesiredWidth;
                                col.MaxChildDesiredWidth = Math.Max(col.MaxChildDesiredWidth, Children[i].DesiredSize.Width + padding);
                                if (col.MaxChildDesiredWidth != prev)
                                {
                                    // If our measure has changed, then we have to invalidate the arrangement of the DataTable
                                    parentTable.ColumnResized();
                                }

                                break;
                            }

                        case DataColumn { CurrentWidth.GridUnitType: GridUnitType.Pixel } pixel:
                            Children[i].Measure(new(pixel.DesiredWidth.Value, availableSize.Height));
                            break;
                        default:
                            Children[i].Measure(availableSize);
                            break;
                    }

                    maxHeight = Math.Max(maxHeight, Children[i].DesiredSize.Height);
                }
            }

            // Fallback for Grid Hybrid scenario...
            else if (parentPanel is Grid grid
                     && parentPanel.Children.Count == Children.Count
                     && grid.ColumnDefinitions.Count == Children.Count)
            {
                // TODO: Need to check visibility
                // Measure all children since we need to determine the row's height at minimum
                for (int i = 0; i < Children.Count; i++)
                {
                    if (grid.ColumnDefinitions[i].Width.GridUnitType is GridUnitType.Pixel)
                    {
                        Children[i].Measure(new(grid.ColumnDefinitions[i].Width.Value, availableSize.Height));
                    }
                    else
                    {
                        Children[i].Measure(availableSize);
                    }

                    maxHeight = Math.Max(maxHeight, Children[i].DesiredSize.Height);
                }
            }
        }

        // Otherwise, return our parent's size as the desired size.
        return new(parentPanel?.DesiredSize.Width ?? availableSize.Width, maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int column = 0;
        double x = 0;

        // Try and grab Column Spacing from DataTable, if not a parent Grid, if not 0.
        double spacing = parentTable?.ColumnSpacing ?? (parentPanel as Grid)?.ColumnSpacing ?? 0;

        double width = 0;

        if (parentPanel is not null)
        {
            int i = 0;
            foreach (UIElement child in Children.Where(static e => e.Visibility == Visibility.Visible))
            {
                if (parentPanel is Grid grid && column < grid.ColumnDefinitions.Count)
                {
                    width = grid.ColumnDefinitions[column++].ActualWidth;
                }

                // TODO: Need to check Column visibility here as well...
                else if (parentPanel is DataTable table &&
                         column < table.Children.Count)
                {
                    // TODO: This is messy...
                    width = (table.Children[column++] as DataColumn)?.ActualWidth ?? 0;
                }

                // Note: For Auto, since we measured our children and bubbled that up to the DataTable layout, then the DataColumn size we grab above should account for the largest of our children.
                if (i == 0)
                {
                    child.Arrange(new(x, 0, width, finalSize.Height));
                }
                else
                {
                    // If we're in a tree, remove the indentation from the layout of columns beyond the first.
                    child.Arrange(new(x - treePadding, 0, width, finalSize.Height));
                }

                x += width + spacing;
                i++;
            }

            return new(Math.Max(0, x - spacing), finalSize.Height);
        }

        return finalSize;
    }

    private void OnDataRowUnloaded(object sender, RoutedEventArgs e)
    {
        // Remove our references on unloaded
        parentTable?.Rows.Remove(this);
        parentTable = null;
        parentPanel = null;
    }

    private Microsoft.UI.Xaml.Controls.Panel? InitializeParentHeaderConnection()
    {
        // TODO: Think about this expression instead...
        //       Drawback: Can't have Grid between table and header
        //       Positive: don't have to restart climbing the Visual Tree if we don't find ItemsPresenter...
        ////var parent = this.FindAscendant<FrameworkElement>(static (element) => element is ItemsPresenter or Grid);

        // TODO: Investigate what a scenario with an ItemsRepeater would look like (with a StackLayout, but using DataRow as the item's panel inside)
        Microsoft.UI.Xaml.Controls.Panel? panel = null;

        // 1a. Get parent ItemsPresenter to find header
        if (this.FindAscendant<ItemsPresenter>() is { } itemsPresenter)
        {
            // 2. Quickly check if the header is just what we're looking for.
            if (itemsPresenter.Header is Grid or DataTable)
            {
                panel = itemsPresenter.Header as Microsoft.UI.Xaml.Controls.Panel;
            }
            else
            {
                // 3. Otherwise, try and find the inner thing we want.
                panel = itemsPresenter.FindDescendant<Microsoft.UI.Xaml.Controls.Panel>(static element => element is Grid or DataTable);
            }

            // Check if we're in a TreeView
            isTreeView = itemsPresenter.FindAscendant<TreeView>() is not null;
        }

        // 1b. If we can't find the ItemsPresenter, then we reach up outside to find the next thing we could use as a parent
        panel ??= this.FindAscendant<Microsoft.UI.Xaml.Controls.Panel>(static element => element is Grid or DataTable);

        // Cache actual datatable reference
        if (panel is DataTable table)
        {
            parentTable = table;
            parentTable.Rows.Add(this); // Add us to the row list.
        }

        return panel;
    }
}