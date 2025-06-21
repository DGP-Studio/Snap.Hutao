// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Abstraction;

[ConstructorGenerated]
internal abstract partial class ViewModelSlim : ObservableObject
{
    [ObservableProperty]
    public partial bool IsInitialized { get; set; }

    protected partial IServiceProvider ServiceProvider { get; }

    [Command("LoadCommand")]
    protected virtual Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}

[ConstructorGenerated(CallBaseConstructor = true)]
internal abstract partial class ViewModelSlim<TPage> : ViewModelSlim
    where TPage : Page
{
    [Command("NavigateCommand")]
    protected virtual void Navigate()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI($"Navigate to {TypeNameHelper.GetTypeDisplayName(typeof(TPage), fullName: false)}", "ViewModelSlim.Command"));

        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(new NavigationExtraData(new DrillInNavigationTransitionInfo()), true);
    }
}