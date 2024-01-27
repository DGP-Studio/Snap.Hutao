// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Win32.Foundation;
using Snap.Hutao.Win32.NetworkManagement.WindowsFirewall;
using Snap.Hutao.Win32.Security;
using static Snap.Hutao.Win32.AdvApi32;
using static Snap.Hutao.Win32.ApiMsWinNetIsolation;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Injection(InjectAs.Singleton)]
internal sealed unsafe class LoopbackManager : ObservableObject, IDisposable
{
    private readonly INET_FIREWALL_APP_CONTAINER* pContainers;
    private readonly INET_FIREWALL_APP_CONTAINER* pHutaoContainer;

    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    public LoopbackManager(IServiceProvider serviceProvider)
    {
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        NetworkIsolationEnumAppContainers(NETISO_FLAG.NETISO_FLAG_MAX, out uint acCount, out pContainers);
        for (uint i = 0; i < acCount; i++)
        {
            INET_FIREWALL_APP_CONTAINER* pContainer = pContainers + i;
            if (new string(pContainer->appContainerName) == runtimeOptions.FamilyName)
            {
                pHutaoContainer = pContainer;
                break;
            }
        }

        NetworkIsolationGetAppContainerConfig(out uint accCount, out SID_AND_ATTRIBUTES* pSids);
        for (uint i = 0; i < accCount; i++)
        {
            if (EqualSid(new PSID { Value = pHutaoContainer->appContainerSid }, (pSids + i)->Sid))
            {
                IsLoopbackEnabled = true;
                break;
            }
        }
    }

    public bool IsLoopbackEnabled { get; private set; }

    public void EnableLoopback()
    {
        List<SID_AND_ATTRIBUTES> sids = [];

        NetworkIsolationGetAppContainerConfig(out uint accCount, out SID_AND_ATTRIBUTES* pSids);
        for (uint i = 0; i < accCount; i++)
        {
            sids.Add(*(pSids + i));
        }

        sids.Add(new SID_AND_ATTRIBUTES
        {
            Sid = new PSID { Value = pHutaoContainer->appContainerSid },
            Attributes = 0,
        });

        IsLoopbackEnabled = NetworkIsolationSetAppContainerConfig(sids.ToArray()) is 0U;

        taskContext.InvokeOnMainThread(() => OnPropertyChanged(nameof(IsLoopbackEnabled)));
    }

    public void Dispose()
    {
        _ = NetworkIsolationFreeAppContainers(pContainers);
    }
}
