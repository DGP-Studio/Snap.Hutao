// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Task;
using Snap.Hutao.Win32.Foundation;
using System.Collections.Immutable;
using System.IO.Pipes;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

[Injection(InjectAs.Singleton)]
internal sealed partial class BetterGenshinImpactNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", "BetterGenshinImpact.NamedPipe", PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

    private uint contractVersion;

    public void Dispose()
    {
        clientStream.Dispose();
    }

    public bool TryGetContractVersion()
    {
        contractVersion = default;

        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            PipeRequest<Void> getContractVersionRequest = new()
            {
                Kind = PipeRequestKind.GetContractVersion,
                Data = default,
            };
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, getContractVersionRequest);
            clientStream.ReadPacket(out _, out PipeResponse<JsonElement>? response);

            if (response is { Kind: PipeResponseKind.Number })
            {
                contractVersion = response.Data.GetUInt32();
                return true;
            }
            else
            {
                return false;
            }
        }
        finally
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();
        }
    }

    public bool TryStartCapture(HWND hwnd)
    {
        if (contractVersion < 1)
        {
            return false;
        }

        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            PipeRequest<long> startCaptureRequest = new() { Kind = PipeRequestKind.StartCapture, Data = hwnd };
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, startCaptureRequest);
            clientStream.ReadPacket(out _, out PipeResponse<JsonElement>? response);

            return response is { Kind: PipeResponseKind.Boolean } && response.Data.GetBoolean();
        }
        finally
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();
        }
    }

    public bool TryStopCapture()
    {
        if (contractVersion < 1)
        {
            return false;
        }

        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            PipeRequest<Void> stopCaptureRequest = new()
            {
                Kind = PipeRequestKind.StopCapture,
                Data = default,
            };
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, stopCaptureRequest);
            clientStream.ReadPacket(out _, out PipeResponse<JsonElement>? response);

            return response is { Kind: PipeResponseKind.Boolean } && response.Data.GetBoolean();
        }
        finally
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();
        }
    }

    public bool TryEndSwitchToNextGameAccount()
    {
        if (contractVersion < 1)
        {
            return false;
        }

        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            PipeRequest<Void> endSwitchToNextGameAccountRequest = new()
            {
                Kind = PipeRequestKind.EndSwitchToNextGameAccount,
                Data = default,
            };
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, endSwitchToNextGameAccountRequest);
            clientStream.ReadPacket(out _, out PipeResponse<JsonElement>? response);

            return response is { Kind: PipeResponseKind.Boolean } && response.Data.GetBoolean();
        }
        finally
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();
        }
    }

    public bool TryQueryTaskArray(out ImmutableArray<AutomationTaskDefinition> taskDefinitions)
    {
        taskDefinitions = default;

        if (contractVersion < 1)
        {
            return false;
        }

        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            PipeRequest<Void> queryTaskArrayRequest = new()
            {
                Kind = PipeRequestKind.QueryTaskArray,
                Data = default,
            };
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, queryTaskArrayRequest);
            clientStream.ReadPacket(out _, out PipeResponse<JsonElement>? response);

            if (response is { Kind: PipeResponseKind.Array })
            {
                taskDefinitions = response.Data.Deserialize<ImmutableArray<AutomationTaskDefinition>>();
                return true;
            }
            else
            {
                return false;
            }
        }
        finally
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();
        }
    }

    public bool TryStartTask(string id)
    {
        if (contractVersion < 1)
        {
            return false;
        }

        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            PipeRequest<string> startTaskRequest = new()
            {
                Kind = PipeRequestKind.StartTask,
                Data = id,
            };
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.Version, PipePacketType.Request, PipePacketCommand.SnapHutaoToBetterGenshinImpactRequest, startTaskRequest);
            clientStream.ReadPacket(out _, out PipeResponse<JsonElement>? response);

            return response is { Kind: PipeResponseKind.Boolean } && response.Data.GetBoolean();
        }
        finally
        {
            clientStream.WritePacket(PrivateNamedPipe.Version, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();
        }
    }
}