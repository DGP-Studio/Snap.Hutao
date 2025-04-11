// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.Foundation;
using Microsoft.UI.Composition;
using Microsoft.UI.Dispatching;
using System.Runtime.InteropServices;
using WinRT;

// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
namespace Microsoft.UI.Content;
#pragma warning restore IDE0130

[WindowsRuntimeType("Microsoft.UI")]
[Guid("1054BF83-B35B-5FDE-8DD7-AC3BB3E6CE27")]
[WindowsRuntimeHelperType(typeof(global::ABI.Microsoft.UI.Content.IContentExternalBackdropLink))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal interface IContentExternalBackdropLink
{
    DispatcherQueue DispatcherQueue { get; }

    CompositionBorderMode ExternalBackdropBorderMode { get; set; }

    Visual PlacementVisual { get; }
}

[WindowsRuntimeType("Microsoft.UI")]
[Guid("46CAC6FB-BB51-510A-958D-E0EB4160F678")]
[WindowsRuntimeHelperType(typeof(global::ABI.Microsoft.UI.Content.IContentExternalBackdropLinkStatics))]
[global::Windows.Foundation.Metadata.ContractVersion(typeof(WindowsAppSDKContract), 65543u)]
internal interface IContentExternalBackdropLinkStatics
{
    ContentExternalBackdropLink Create(Compositor compositor);
}