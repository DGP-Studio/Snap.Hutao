namespace Windows.Win32.Foundation;
public partial struct RECT
{
    public RECT(int left, int top, int right, int bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }

    public int Size
    {
        get
        {
            return (right - left) * (bottom - top);
        }
    }
}
