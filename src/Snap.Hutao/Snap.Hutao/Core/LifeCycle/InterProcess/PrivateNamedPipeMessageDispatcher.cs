// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Factory.Process;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class PrivateNamedPipeMessageDispatcher
{
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial PrivateNamedPipeMessageDispatcher(IServiceProvider serviceProvider);

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
        // Note: This method can be called from any thread,
        // so we have to switch to the main thread to call
        // the Exit method of the App instance.
        try
        {
            // Cannot access a disposed object. Object name: 'IServiceProvider'.
            ITaskContext taskContext = serviceProvider.GetRequiredService<ITaskContext>();
            App app = serviceProvider.GetRequiredService<App>();
            taskContext.InvokeOnMainThread(app.Exit);
        }
        catch
        {
            ProcessFactory.KillCurrent();
        }
    }
}