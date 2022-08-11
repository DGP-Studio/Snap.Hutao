using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Win32;

/// <summary>
/// 包装不安全的代码
/// </summary>
public class Unsafe
{
    /// <summary>
    /// 使用指针操作简化封送
    /// </summary>
    /// <param name="lParam">lParam</param>
    /// <param name="minWidth">最小宽度</param>
    /// <param name="minHeight">最小高度</param>
    public static unsafe void SetMinTrackSize(nint lParam, double minWidth, double minHeight)
    {
        MINMAXINFO* info = (MINMAXINFO*)lParam;
        info->ptMinTrackSize.x = (int)Math.Max(minWidth, info->ptMinTrackSize.x);
        info->ptMinTrackSize.y = (int)Math.Max(minHeight, info->ptMinTrackSize.y);
    }
}
