namespace Snap.Hutao.Factory.Progress;

internal interface IProgressFactory
{
    IProgress<T> CreateForMainThread<T>(Action<T> handler);
}