// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Game.Scheme;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Branch;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using Snap.Hutao.Web.Hoyolab.HoyoPlay.Connect.Package;

namespace Snap.Hutao.Service.Game.Package;

internal readonly struct PackageConverterContext
{
    public readonly LaunchScheme Scheme;
    public readonly string GameDirectory;
    public readonly BranchWrapper? CurrentBranch;
    public readonly BranchWrapper? TargetBranch;
    public readonly GamePackage? TargetPackage;
    public readonly GameChannelSDK? GameChannelSDK;
    public readonly DeprecatedFilesWrapper? DeprecatedFiles;
    public readonly IProgress<PackageConvertStatus> Progress;

    public PackageConverterContext(LaunchScheme scheme, string gameDirectory, BranchWrapper currentBranch, BranchWrapper targetBranch, GameChannelSDK? gameChannelSDK, IProgress<PackageConvertStatus> progress)
    {
        Scheme = scheme;
        GameDirectory = gameDirectory;
        CurrentBranch = currentBranch;
        TargetBranch = targetBranch;
        GameChannelSDK = gameChannelSDK;
        Progress = progress;
    }

    public PackageConverterContext(LaunchScheme scheme, string gameDirectory, GamePackage gamePackage, GameChannelSDK? gameChannelSDK, DeprecatedFilesWrapper? deprecatedFiles, IProgress<PackageConvertStatus> progress)
    {
        Scheme = scheme;
        GameDirectory = gameDirectory;
        TargetPackage = gamePackage;
        GameChannelSDK = gameChannelSDK;
        DeprecatedFiles = deprecatedFiles;
        Progress = progress;
    }
}