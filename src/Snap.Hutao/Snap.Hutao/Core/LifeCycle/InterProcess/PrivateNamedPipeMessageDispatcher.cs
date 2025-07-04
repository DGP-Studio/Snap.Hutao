// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
[ConstructorGenerated]
internal sealed partial class PrivateNamedPipeMessageDispatcher
{
    private readonly IServiceProvider serviceProvider;

    public void RedirectedActivation(HutaoActivationArguments? args)
    {
        if (args is null)
        {
            return;
        }

        serviceProvider.GetRequiredService<IAppActivation>().RedirectedActivate(args);
    }

    public void ExitApplication()
    {
        try
        {
            // Cannot access a disposed object. Object name: 'IServiceProvider'.
            serviceProvider.GetRequiredService<App>().Exit();
        }
        catch
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}