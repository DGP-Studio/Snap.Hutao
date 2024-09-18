// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle.InterProcess.Model;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
internal sealed partial class PrivateNamedPipeServer : IDisposable
{
    private readonly PrivateNamedPipeMessageDispatcher messageDispatcher;
    private readonly RuntimeOptions runtimeOptions;
    private readonly ILogger<PrivateNamedPipeServer> logger;

    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly AsyncLock serverLock = new();

    private readonly NamedPipeServerStream serverStream;

    public PrivateNamedPipeServer(IServiceProvider serviceProvider)
    {
        messageDispatcher = serviceProvider.GetRequiredService<PrivateNamedPipeMessageDispatcher>();
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        logger = serviceProvider.GetRequiredService<ILogger<PrivateNamedPipeServer>>();

        PipeSecurity? pipeSecurity = default;

        if (runtimeOptions.IsElevated)
        {
            SecurityIdentifier everyOne = new(WellKnownSidType.WorldSid, null);

            pipeSecurity = new();
            pipeSecurity.AddAccessRule(new PipeAccessRule(everyOne, PipeAccessRights.FullControl, AccessControlType.Allow));
        }

        serverStream = NamedPipeServerStreamAcl.Create(
            PrivateNamedPipe.Name,
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough,
            0,
            0,
            pipeSecurity);
    }

    public void Dispose()
    {
        serverTokenSource.Cancel();
        using AsyncLock.Releaser discard = serverLock.LockAsync().GetAwaiter().GetResult();
        serverTokenSource.Dispose();
        serverStream.Dispose();
    }

    public async ValueTask RunAsync()
    {
        using (await serverLock.LockAsync().ConfigureAwait(false))
        {
            while (!serverTokenSource.IsCancellationRequested)
            {
                try
                {
                    await serverStream.WaitForConnectionAsync(serverTokenSource.Token).ConfigureAwait(false);
                    logger.LogInformation("Pipe session created");
                    RunPacketSession(serverStream, serverTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }

    private unsafe void RunPacketSession(NamedPipeServerStream serverStream, CancellationToken token)
    {
        while (serverStream.IsConnected && !token.IsCancellationRequested)
        {
            serverStream.ReadPacket(out PipePacketHeader header);
            logger.LogInformation("Pipe packet: [Type:{Type}] [Command:{Command}]", header.Type, header.Command);
            switch ((header.Type, header.Command))
            {
                case (PipePacketType.Request, PipePacketCommand.RequestElevationStatus):
                    ElevationStatusResponse resp = new(runtimeOptions.IsElevated);
                    serverStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Response, PipePacketCommand.ResponseElevationStatus, resp);
                    serverStream.Flush();
                    break;

                case (PipePacketType.Request, PipePacketCommand.RedirectActivation):
                    HutaoActivationArguments? hutaoArgs = serverStream.ReadJsonContent<HutaoActivationArguments>(in header);
                    if (hutaoArgs is not null)
                    {
                        logger.LogInformation("Redirect activation: [Kind:{Kind}] [Arguments:{Arguments}]", hutaoArgs.Kind, hutaoArgs.LaunchActivatedArguments);
                    }

                    messageDispatcher.RedirectActivation(hutaoArgs);
                    break;

                case (PipePacketType.SessionTermination, _):
                    serverStream.Disconnect();
                    if (header.Command is PipePacketCommand.Exit)
                    {
                        messageDispatcher.ExitApplication();
                    }

                    return;
            }
        }
    }
}