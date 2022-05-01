namespace Snap.Hutao.Core.DependencyInjection;

/// <summary>
/// 注入方法
/// </summary>
public enum InjectAs
{
    /// <summary>
    /// 指示应注册为单例对象
    /// </summary>
    Singleton,

    /// <summary>
    /// 指示应注册为短期对象
    /// </summary>
    Transient,
}
