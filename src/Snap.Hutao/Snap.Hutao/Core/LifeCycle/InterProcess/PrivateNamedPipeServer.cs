// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;
using Snap.Hutao.Core.LifeCycle.InterProcess.Model;
using Snap.Hutao.Core.Security.Principal;
using System.IO.Pipes;
using System.Security.AccessControl;

namespace Snap.Hutao.Core.LifeCycle.InterProcess;

[Injection(InjectAs.Singleton)]
internal sealed partial class PrivateNamedPipeServer : IDisposable
{
    private readonly BetterGenshinImpactNamedPipeServer betterGenshinImpactNamedPipeServer;
    private readonly PrivateNamedPipeMessageDispatcher messageDispatcher;
    private readonly ILogger<PrivateNamedPipeServer> logger;

    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly AsyncLock serverLock = new();

    private readonly NamedPipeServerStream serverStream;

    public PrivateNamedPipeServer(IServiceProvider serviceProvider)
    {
        betterGenshinImpactNamedPipeServer = serviceProvider.GetRequiredService<BetterGenshinImpactNamedPipeServer>();
        messageDispatcher = serviceProvider.GetRequiredService<PrivateNamedPipeMessageDispatcher>();
        logger = serviceProvider.GetRequiredService<ILogger<PrivateNamedPipeServer>>();

        PipeSecurity pipeSecurity = new();
        pipeSecurity.AddAccessRule(new(SecurityIdentifiers.Everyone, PipeAccessRights.FullControl, AccessControlType.Allow));

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

    public void Start()
    {
        RunAsync().SafeForget();
    }

    private async ValueTask RunAsync()
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
                    break;
                }
            }
        }
    }

    private void RunPacketSession(NamedPipeServerStream serverStream, CancellationToken token)
    {
        while (serverStream.IsConnected && !token.IsCancellationRequested)
        {
            serverStream.ReadPacket(out PipePacketHeader header);
            logger.LogInformation("Pipe packet: [Type:{Type}] [Command:{Command}]", header.Type, header.Command);
            switch ((header.Type, header.Command))
            {
                case (PipePacketType.Request, PipePacketCommand.RequestElevationStatus):
                    ElevationStatusResponse resp = new(HutaoRuntime.IsProcessElevated, Environment.ProcessId);
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

                case (PipePacketType.Request, PipePacketCommand.BetterGenshinImpactToSnapHutaoRequest):
                    PipeRequest<JsonElement>? request = serverStream.ReadJsonContent<PipeRequest<JsonElement>>(in header);
                    PipeResponse response = betterGenshinImpactNamedPipeServer.DispatchRequest(request);
                    serverStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Response, PipePacketCommand.SnapHutaoToBetterGenshinImpactResponse, response);
                    serverStream.Flush();
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