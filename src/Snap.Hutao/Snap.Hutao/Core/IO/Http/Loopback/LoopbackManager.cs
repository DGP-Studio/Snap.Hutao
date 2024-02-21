// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;
using Snap.Hutao.Win32.Security;
using System.Runtime.InteropServices;
using static Snap.Hutao.Win32.AdvApi32;
using static Snap.Hutao.Win32.ApiMsWinNetIsolation;
using static Snap.Hutao.Win32.Macros;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Injection(InjectAs.Singleton)]
internal sealed unsafe class LoopbackManager : ObservableObject
{
    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    private readonly string hutaoContainerStringSID = default!;
    private bool isLoopbackEnabled;

    public LoopbackManager(IServiceProvider serviceProvider)
    {
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        INET_FIREWALL_APP_CONTAINER* pContainers = default;
        try
        {
            {
                WIN32_ERROR error = NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out uint acCount, out pContainers);
                Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));
                for (uint i = 0; i < acCount; i++)
                {
                    INET_FIREWALL_APP_CONTAINER* pContainer = pContainers + i;
                    ReadOnlySpan<char> appContainerName = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(pContainer->appContainerName);
                    if (appContainerName.Equals(runtimeOptions.FamilyName, StringComparison.Ordinal))
                    {
                        ConvertSidToStringSidW(pContainer->appContainerSid, out PWSTR stringSid);
                        hutaoContainerStringSID = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(stringSid).ToString();
                        break;
                    }
                }
            }
        }
        finally
        {
            // This function returns 1 rather than 0 specfied in the document.
            _ = NetworkIsolationFreeAppContainers(pContainers);
        }

        {
            WIN32_ERROR error = NetworkIsolationGetAppContainerConfig(out uint accCount, out SID_AND_ATTRIBUTES* pSids);
            Marshal.ThrowExceptionForHR(HRESULT_FROM_WIN32(error));
            for (uint i = 0; i < accCount; i++)
            {
                ConvertSidToStringSidW((pSids + i)->Sid, out PWSTR stringSid);
                ReadOnlySpan<char> stringSidSpan = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(stringSid);
                if (stringSidSpan.Equals(hutaoContainerStringSID, StringComparison.Ordinal))
                {
                    IsLoopbackEnabled = true;
                    break;
                }
            }
        }
    }

    public bool IsLoopbackEnabled { get => isLoopbackEnabled; private set => SetProperty(ref isLoopbackEnabled, value); }

    public void EnableLoopback()
    {
        NetworkIsolationGetAppContainerConfig(out uint accCount, out SID_AND_ATTRIBUTES* pSids);
        List<SID_AND_ATTRIBUTES> sids = new((int)(accCount + 1));
        for (uint i = 0; i < accCount; i++)
        {
            sids.Add(*(pSids + i));
        }

        ConvertStringSidToSidW(hutaoContainerStringSID, out PSID pSid);
        SID_AND_ATTRIBUTES sidAndAttributes = default;
        sidAndAttributes.Sid = pSid;
        sids.Add(sidAndAttributes);
        IsLoopbackEnabled = NetworkIsolationSetAppContainerConfig(CollectionsMarshal.AsSpan(sids)) is WIN32_ERROR.ERROR_SUCCESS;
    }
}