// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("Speed", typeof(string))]
[DependencyProperty("RemainingTime", typeof(string))]
[DependencyProperty("Value", typeof(int), 0, nameof(OnChunksChanged))]
[DependencyProperty("Maximum", typeof(int), -1, nameof(OnChunksChanged))]
[DependencyProperty("Description", typeof(string))]
[DependencyProperty("IconGlyph", typeof(string))]
internal sealed partial class SophonProgressBar : UserControl, INotifyPropertyChanged
{
    public SophonProgressBar()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public double ProgressValue { get => Maximum <= 0 ? 0D : 1D * Value / Maximum; }

    public string ProgressPercentFormatted { get => $"{ProgressValue:P2}"; }

    public string ProgressFormatted { get => Maximum > -1 ? $"{Value} / {Maximum}" : SH.UIXamlViewSpecializedSophonProgressDefault; }

    public bool IsIndeterminate { get => Maximum is -1; }

    private static void OnChunksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SophonProgressBar sender = (SophonProgressBar)d;
        sender.OnPropertyChanged(nameof(ProgressValue));
        sender.OnPropertyChanged(nameof(ProgressPercentFormatted));
        sender.OnPropertyChanged(nameof(ProgressFormatted));
        sender.OnPropertyChanged(nameof(IsIndeterminate));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }
}