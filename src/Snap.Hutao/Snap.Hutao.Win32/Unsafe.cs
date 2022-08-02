using Windows.Win32.UI.WindowsAndMessaging;

namespace Snap.Hutao.Win32;
public class Unsafe
{
    /// <summary>
    /// 使用指针操作简化封送
    /// </summary>
    /// <param name="lPARAM">lParam</param>
    /// <param name="minWidth">最小宽度</param>
    /// <param name="minHeight">最小高度</param>
    public static unsafe void SetMinTrackSize(nint lPARAM, float minWidth, float minHeight)
    {
        MINMAXINFO* rect2 = (MINMAXINFO*)lPARAM;
        rect2->ptMinTrackSize.x = (int)Math.Max(minWidth, rect2->ptMinTrackSize.x);
        rect2->ptMinTrackSize.y = (int)Math.Max(minHeight, rect2->ptMinTrackSize.y);
    }
}
