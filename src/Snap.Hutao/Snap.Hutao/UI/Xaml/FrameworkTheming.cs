// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Win32.System.Diagnostics.Debug;
using Snap.Hutao.Win32.System.SystemService;
using System.Diagnostics;
using static Snap.Hutao.Win32.Kernel32;

namespace Snap.Hutao.UI.Xaml;

internal static unsafe class FrameworkTheming
{
    private static readonly nint Module = GetModuleHandleW("microsoft.ui.xaml.dll");

    private static readonly delegate* unmanaged[Stdcall]<DXamlCoreAbi**, int> DXamlInstanceStorageGetValue;

    private static readonly delegate* unmanaged[Stdcall]<FrameworkThemingAbi*, bool, int> FrameworkThemingOnThemeChanged;

    static FrameworkTheming()
    {
        IMAGE_DOS_HEADER* pImageDosHeader = (IMAGE_DOS_HEADER*)Module;
        IMAGE_NT_HEADERS64* pImageNtHeader = (IMAGE_NT_HEADERS64*)(pImageDosHeader->e_lfanew + Module);
        ReadOnlySpan<byte> moduleMemory = new((void*)Module, (int)pImageNtHeader->OptionalHeader.SizeOfImage);

        // DXamlInstanceStorage::GetValue(void** phValue)
        // _Check_return_ HRESULT GetValue(_Outptr_result_maybenull_ Handle* phValue)
        // 40 53                    push    rbx
        // 48 83 EC 20              sub     rsp, 20h
        // 48 8B D9                 mov     rbx, phValue(rcx)
        // 8B 0D ?? ?? ?? ??        mov     ecx, cs:?g_dwTlsIndex@DXamlInstanceStorage@@3KA ; dwTlsIndex
        ReadOnlySpan<byte> patternDXamlInstanceStorageGetValue = [0x40, 0x53, 0x48, 0x83, 0xEC, 0x20, 0x48, 0x8B, 0xD9, 0x8B, 0x0D];

        DXamlInstanceStorageGetValue = (delegate* unmanaged[Stdcall]<DXamlCoreAbi**, int>)(Module + moduleMemory.IndexOf(patternDXamlInstanceStorageGetValue));

        // FrameworkTheming::OnThemeChanged(FrameworkTheming* this, bool forceUpdate)
        // _Check_return_ HRESULT OnThemeChanged(bool forceUpdate = false)
        // 48 89 5C 24 10           mov     [rsp+arg_8(10h)], rbx
        // 48 89 6C 24 18           mov     [rsp+arg_10(18h)], rbp
        // 48 89 74 24 20           mov     [rsp+arg_18(20h)], rsi
        // 57                       push    rdi
        // 41 56                    push    r14
        // 41 57                    push    r15
        // 48 83 EC 40              sub     rsp, 40h
        // 48 8B 05 ?? ?? ?? ??     mov     rax, cs:__security_cookie
        // 48 33 C4                 xor     rax, rsp
        // 48 89 44 24 30           mov     [rsp+58h+var_28(-28h)], rax
        // F6 05 ?? ?? ?? ?? 08     test    byte ptr cs:Microsoft_Windows_XAMLEnableBits, 8
        ReadOnlySpan<byte> patternFrameworkThemingOnThemeChanged1 = [0x48, 0x89, 0x5C, 0x24, 0x10, 0x48, 0x89, 0x6C, 0x24, 0x18, 0x48, 0x89, 0x74, 0x24, 0x20, 0x57, 0x41, 0x56, 0x41, 0x57, 0x48, 0x83, 0xEC, 0x40, 0x48, 0x8B, 0x05];
        ReadOnlySpan<byte> patternFrameworkThemingOnThemeChanged2 = [0x48, 0x33, 0xC4, 0x48, 0x89, 0x44, 0x24, 0x30, 0xF6, 0x05];
        ReadOnlySpan<byte> patternFrameworkThemingOnThemeChanged3 = [0x08];

        int offsetFrameworkThemingOnThemeChanged = 0;
        while (offsetFrameworkThemingOnThemeChanged < moduleMemory.Length)
        {
            offsetFrameworkThemingOnThemeChanged += moduleMemory[offsetFrameworkThemingOnThemeChanged..].IndexOf(patternFrameworkThemingOnThemeChanged1);
            ReadOnlySpan<byte> part1 = moduleMemory[offsetFrameworkThemingOnThemeChanged..];

            ReadOnlySpan<byte> part2 = part1[(patternFrameworkThemingOnThemeChanged1.Length + 4)..];
            if (part2.StartsWith(patternFrameworkThemingOnThemeChanged2))
            {
                ReadOnlySpan<byte> part3 = part2[(patternFrameworkThemingOnThemeChanged2.Length + 4)..];
                if (part3.StartsWith(patternFrameworkThemingOnThemeChanged3))
                {
                    break;
                }
            }

            offsetFrameworkThemingOnThemeChanged += patternFrameworkThemingOnThemeChanged1.Length + 4 + patternFrameworkThemingOnThemeChanged2.Length + 4 + patternFrameworkThemingOnThemeChanged3.Length;
        }

        Debug.Assert(offsetFrameworkThemingOnThemeChanged < moduleMemory.Length);

        FrameworkThemingOnThemeChanged = (delegate* unmanaged[Stdcall]<FrameworkThemingAbi*, bool, int>)(Module + offsetFrameworkThemingOnThemeChanged);
    }

    public static void SetTheme(Theme theme)
    {
        DXamlCoreAbi* pXamlCore = default;
        DXamlInstanceStorageGetValue(&pXamlCore);

        // CCoreServices DirectUI::DXamlServices::GetHandle
        CCoreServiceAbi* pCoreService = (CCoreServiceAbi*)((ulong*)pXamlCore + 8);

        // CApplication::SetValue
        // std::unique_ptr<FrameworkTheming> m_spTheming
        FrameworkThemingAbi* theming = *(FrameworkThemingAbi**)(*(ulong*)pCoreService + 0x670L);
        ((Theme*)theming)[0x50] = theme;
        FrameworkThemingOnThemeChanged(theming, true);
    }

    private readonly struct CCoreServiceAbi;

    private readonly struct DXamlCoreAbi;

    private readonly struct FrameworkThemingAbi;
}