using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace Windows.Win32.UI.WindowsAndMessaging;
public partial struct WINDOWPLACEMENT
{
    /// <summary>
    /// Gets the default (empty) value.
    /// </summary>
    public static WINDOWPLACEMENT Default
    {
        get
        {
            return new WINDOWPLACEMENT()
            {
                length = (uint)Marshal.SizeOf<WINDOWPLACEMENT>(),
            };
        }
    }

    /// <summary>
    /// 构造一个新的<see cref="WINDOWPLACEMENT"/>
    /// </summary>
    /// <param name="ptMaxPosition">最大点</param>
    /// <param name="rcNormalPosition">正常位置</param>
    /// <param name="showCmd">显示命令</param>
    /// <returns>窗体位置</returns>
    public static WINDOWPLACEMENT Create(POINT ptMaxPosition, RECT rcNormalPosition, SHOW_WINDOW_CMD showCmd)
    {
        WINDOWPLACEMENT result = Default;

        result.ptMaxPosition = ptMaxPosition;
        result.rcNormalPosition = rcNormalPosition;
        result.showCmd = showCmd;

        return result;
    }
}
