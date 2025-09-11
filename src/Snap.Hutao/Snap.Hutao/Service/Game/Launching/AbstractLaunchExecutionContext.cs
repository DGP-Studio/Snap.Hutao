// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;
using Snap.Hutao.Core.Diagnostics;
using Snap.Hutao.Service.Game.FileSystem;

namespace Snap.Hutao.Service.Game.Launching;

// This part is tear out from LaunchExecutionContext to enforce generated ctor don't raise warning for unset fields
[ConstructorGenerated]
internal abstract partial class AbstractLaunchExecutionContext : IDisposable
{
    private IGameFileSystem? gameFileSystem;

    public LaunchExecutionResult Result { get; } = new();

    public partial IServiceProvider ServiceProvider { get; }

    public partial ITaskContext TaskContext { get; }

    public partial ILogger<LaunchExecutionContext> Logger { get; }

    public partial LaunchOptions Options { get; }

    public string? AuthTicket { get; set; }

    public bool ChannelOptionsChanged { get; set; }

    /// <summary>
    /// Requires <see cref="Handler.LaunchExecutionStatusProgressHandler"/> to execute before getting the value.
    /// </summary>
    public IProgress<LaunchStatus?> Progress { get; set; } = default!;

    /// <summary>
    /// Requires <see cref="Handler.LaunchExecutionGameProcessInitializationHandler"/> to execute before getting the value.
    /// </summary>
    public IProcess Process { get; set; } = default!;

    protected Lock SyncRoot { get; } = new();

    protected bool Disposed { get; set; }

    public bool TryGetGameFileSystem([NotNullWhen(true)] out IGameFileSystemView? gameFileSystemView)
    {
        lock (SyncRoot)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            if (this.gameFileSystem is not null)
            {
                gameFileSystemView = this.gameFileSystem;
                return true;
            }

            if (!Options.TryGetGameFileSystem(out IGameFileSystem? gameFileSystem))
            {
                Result.Kind = LaunchExecutionResultKind.NoActiveGamePath;
                Result.ErrorMessage = SH.ServiceGameLaunchExecutionGamePathNotValid;
                gameFileSystemView = default;
                return false;
            }

            this.gameFileSystem = gameFileSystem;
            gameFileSystemView = gameFileSystem;
            return true;
        }
    }

    public void Dispose()
    {
        if (Disposed)
        {
            return;
        }

        lock (SyncRoot)
        {
            Disposed = true;
            DisposableMarshal.DisposeAndClear(ref gameFileSystem);
        }
    }

    protected void CheckDisposedAndDispose()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);
        DisposableMarshal.DisposeAndClear(ref gameFileSystem);
    }
}