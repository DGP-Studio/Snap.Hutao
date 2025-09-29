// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;
using System.Security.Cryptography;
using System.Text;
using Windows.ApplicationModel;

namespace Snap.Hutao.Core.ApplicationModel;

internal static class LimitedAccessFeatures
{
    private static readonly string PackagePublisherId = Package.Current.Id.PublisherId;
    private static readonly string PackageFamilyName = Package.Current.Id.FamilyName;

    private static readonly FrozenDictionary<string, string> Features = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create("com.microsoft.services.cortana.cortanaactionableinsights_v1", "nEVyyzytE6ankNk1CIAu6sZsh8vKLw3Q7glTOHB11po="),
        KeyValuePair.Create("com.microsoft.windows.additional_foreground_boost_processes", "c0abf97e-e14d-468a-b6bc-35be5c62610a"),

        // Windows Conversational Agent API
        KeyValuePair.Create("com.microsoft.windows.applicationmodel.conversationalagent_v1", "hhrovbOc/z8TgeoWheL4RF5vLLJrKNAQpdyvhlTee6I"),
        KeyValuePair.Create("com.microsoft.windows.applicationmodel.phonelinetransportdevice_v1", "cb9WIvVfhp+8lFhaSrB6V6zUBGqctteKi/f/9AIeoZ4"),

        // Application Window API
        KeyValuePair.Create("com.microsoft.windows.applicationwindow", "e5a85131-319b-4a56-9577-1c1d9c781218"),

        // Shell, Window Focus API
        KeyValuePair.Create("com.microsoft.windows.focussessionmanager.1", "ba3faac1-0878-4bb9-9b35-2224aa0ee7cf"),

        // Location Override APIs
        KeyValuePair.Create("com.microsoft.windows.geolocationprovider", "6D1544E3-55CB-40D2-A022-31F24E139708"),

        // Dual Engine Interface API
        KeyValuePair.Create("com.microsoft.windows.internetexplorer.iemode", "33951EE6-0B59-40EC-90D6-76B019316C16"),

        // Phi Silica API
        KeyValuePair.Create("com.microsoft.windows.modelcontextprotocolservercatalog", "DC76200A35E1543A3F4E64D8267833BBD88E583FACAC160B860BC813D26EAFEF"),

        // Remote App Windowing APIs
        KeyValuePair.Create("com.microsoft.windows.remote_app_windowing_apis", "f86b14bc-8690-4206-9101-72847823d265"),

        // Rich Edit Math API
        KeyValuePair.Create("com.microsoft.windows.richeditmath", "RDZCQjY2M0YtQkFDMi00NkIwLUI3NzEtODg4NjMxMEVENkFF"),

        // Share Window Command API
        KeyValuePair.Create("com.microsoft.windows.shell.sharewindowcommandsource_v1", "yDvrila5HS/y8SctohQM3WJZOby8NbSoL2hEPTyIRco="),
        KeyValuePair.Create("com.microsoft.windows.storageprovidersuggestionshandler_v1", "BGoeg9Bd5WID7YZ84xr4V37w4d0pOues5QHpAm3krJw="),

        // Remote Desktop Provider API
        KeyValuePair.Create("com.microsoft.windows.system.remotedesktop.provider_v1", "2F712169EF57A9FB0D590593743819F5F47E2DD13E4D9A5458DDA77608CC5E10"),

        // TaskbarManager Pinning API
        KeyValuePair.Create("com.microsoft.windows.taskbar.pin", "4096B239A7295B635C090E647E867B5707DA6AB6CB78340B01FE4E0C8F4953D4"),

        // TaskbarManager Pinning API (Secondary Tile)
        KeyValuePair.Create("com.microsoft.windows.taskbar.requestPinSecondaryTile", "04c19204-10d9-450a-95c4-2910c8f72be3"),
        KeyValuePair.Create("com.microsoft.windows.textinputmethod", "QUYxMTREMjY2QUIwRTE0RkU3NTQ4QTRENjJFMTVDMkUxNjlDQjY1MDg3MEZGMDc1NTI0Nzg5Njk3NkQ0NkQzQw=="),

        // Toast Occlusion Manager API
        KeyValuePair.Create("com.microsoft.windows.ui.notifications.preview.toastOcclusionManagerPreview", "738a6acf-45c1-44ed-85a4-5eb11dc2d084"),

        // Update Orchestrator API
        KeyValuePair.Create("com.microsoft.windows.updateorchestrator.1", "20C662033A4007A55375BF00D986C280B41A418F"),

        // Window Decoration API
        KeyValuePair.Create("com.microsoft.windows.windowdecorations", "425261a8-7f73-4319-8a53-fc13f87e1717")
    ]);

    public static LimitedAccessFeatureRequestResult TryUnlockFeature(string featureId)
    {
        return Windows.ApplicationModel.LimitedAccessFeatures.TryUnlockFeature(featureId, GetToken(featureId), GetAttestation(featureId));
    }

    private static string GetToken(string featureId)
    {
        byte[] source = Encoding.UTF8.GetBytes($"{featureId}!{Features[featureId]}!{PackageFamilyName}");
        return Convert.ToBase64String(CryptographicOperations.HashData(HashAlgorithmName.SHA256, source).AsSpan(0, 16));
    }

    private static string GetAttestation(string featureId)
    {
        return $"{PackagePublisherId} has registered their use of {featureId} with Microsoft and agrees to the terms of use.";
    }
}