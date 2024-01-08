using System;
using Windows.Win32.Foundation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Registry;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="CoCreateInstance(Guid*, object, CLSCTX, Guid*, out object)"/>
    internal static unsafe HRESULT CoCreateInstance<TClass, TInterface>(object? pUnkOuter, CLSCTX dwClsContext, out TInterface ppv)
        where TInterface : class
    {
        HRESULT hr = CoCreateInstance(typeof(TClass).GUID, pUnkOuter, dwClsContext, typeof(TInterface).GUID, out object o);
        ppv = (TInterface)o;
        return hr;
    }

    internal static unsafe WIN32_ERROR RegOpenKeyEx(HKEY hKey, string lpSubKey, uint ulOptions, REG_SAM_FLAGS samDesired, out HKEY phkResult)
    {
        HKEY hKeyResult;
        WIN32_ERROR __result = PInvoke.RegOpenKeyEx(hKey, lpSubKey, ulOptions, samDesired, &hKeyResult);
        phkResult = hKeyResult;
        return __result;
    }
}