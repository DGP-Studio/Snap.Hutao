// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Abstraction;

[ConstructorGenerated]
internal abstract partial class ViewModelSlim : ObservableObject
{
    private readonly IServiceProvider serviceProvider;
    private bool isInitialized;

    public bool IsInitialized { get => isInitialized; set => SetProperty(ref isInitialized, value); }

    protected IServiceProvider ServiceProvider { get => serviceProvider; }

    [Command("OpenUICommand")]
    protected virtual Task OpenUIAsync()
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
        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(INavigationAwaiter.Default, true);
    }
}