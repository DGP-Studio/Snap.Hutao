// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snap.Hutao.UI.Xaml.View.Specialized;

[DependencyProperty("Speed", typeof(string))]
[DependencyProperty("RemainingTime", typeof(string))]
[DependencyProperty("FinishedChunks", typeof(int), 0, nameof(OnChunksChanged))]
[DependencyProperty("TotalChunks", typeof(int), -1, nameof(OnChunksChanged))]
[DependencyProperty("ProgressBarIconGlyph", typeof(string))]
[INotifyPropertyChanged]
internal sealed partial class SophonProgressBar : UserControl
{
    public SophonProgressBar()
    {
        InitializeComponent();
    }

    public double Progress { get => TotalChunks <= 0 ? 0D : 1D * FinishedChunks / TotalChunks; }

    public string ProgressPercent { get => $"{Progress:P2}"; }

    public string ProgressString { get => TotalChunks > -1 ? $"{FinishedChunks} / {TotalChunks}" : "正在加载清单文件"; }

    public bool IsIndeterminate { get => TotalChunks is -1; }

    private static void OnChunksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SophonProgressBar sender = (SophonProgressBar)d;
        sender.OnPropertyChanged(nameof(Progress));
        sender.OnPropertyChanged(nameof(ProgressPercent));
        sender.OnPropertyChanged(nameof(ProgressString));
        sender.OnPropertyChanged(nameof(IsIndeterminate));
    }
}
