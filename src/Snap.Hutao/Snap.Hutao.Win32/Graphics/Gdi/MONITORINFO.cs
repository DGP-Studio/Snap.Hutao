using System.Runtime.InteropServices;

namespace Windows.Win32.Graphics.Gdi;

public partial struct MONITORINFO
{
    public static MONITORINFO Default
    {
        get => new() { cbSize = (uint)Marshal.SizeOf<MONITORINFO>() };
    }
}
