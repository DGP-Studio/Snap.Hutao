// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Service.Navigation;

namespace Snap.Hutao.ViewModel.Abstraction;

[ConstructorGenerated]
internal abstract partial class ViewModelSlim : ObservableObject
{
    public bool IsInitialized { get; set => SetProperty(ref field, value); }

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
        INavigationService navigationService = ServiceProvider.GetRequiredService<INavigationService>();
        navigationService.Navigate<TPage>(INavigationAwaiter.Default, true);
    }
}