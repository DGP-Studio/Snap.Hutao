// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Property;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Abstraction;

internal abstract partial class ViewModelSlim : ObservableObject
{
    [GeneratedConstructor]
    public partial ViewModelSlim(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial bool IsInitialized { get; set; }

    public IProperty<bool> IsViewUnloaded { get => field ??= Property.Create(false); }

    protected partial IServiceProvider ServiceProvider { get; }

    [Command("LoadCommand")]
    protected virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}

internal abstract partial class ViewModelSlim<TPage> : ViewModelSlim
    where TPage : Page
{
    [GeneratedConstructor(CallBaseConstructor = true)]
    public partial ViewModelSlim(IServiceProvider serviceProvider);

    [Command("NavigateCommand")]
    protected virtual void Navigate()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI($"Navigate to {TypeNameHelper.GetTypeDisplayName(typeof(TPage), fullName: false)}", "ViewModelSlim.Command"));

        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(new NavigationExtraData(new DrillInNavigationTransitionInfo()), true);
    }
}