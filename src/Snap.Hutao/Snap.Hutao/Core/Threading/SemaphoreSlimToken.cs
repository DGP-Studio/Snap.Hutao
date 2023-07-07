namespace Snap.Hutao.Core.Threading;

[SuppressMessage("", "SA1201")]
internal readonly struct SemaphoreSlimToken : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim;

    public SemaphoreSlimToken(SemaphoreSlim semaphoreSlim)
    {
        this.semaphoreSlim = semaphoreSlim;
    }

    public void Dispose()
    {
        semaphoreSlim.Release();
    }
}