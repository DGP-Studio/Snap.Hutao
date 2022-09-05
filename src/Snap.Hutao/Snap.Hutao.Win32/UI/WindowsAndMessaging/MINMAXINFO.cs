namespace Windows.Win32.UI.WindowsAndMessaging;
public partial struct MINMAXINFO
{
    public static unsafe ref MINMAXINFO FromPointer(nint value)
    {
        return ref *(MINMAXINFO*)value;
    }
}