// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Text.Json.Converter;

namespace Snap.Hutao.Service.Game.Island;

// IMPORTANT: DO NOT REORDER FIELDS
internal struct IslandFunctionOffsets
{
#pragma warning disable CS0649
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint GameManagerAwake;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint MickeyWonderMethod;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint MickeyWonderMethodPartner;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint MickeyWonderMethodPartner2;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetLastUid;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetFieldOfView;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetEnableFogRendering;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint GetTargetFrameRate;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetTargetFrameRate;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint OpenTeam;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint OpenTeamPageAccordingly;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint CheckCanEnter;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetupQuestBanner;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint FindGameObject;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetActive;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint EventCameraMove;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint ShowOneDamageTextEx;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SwitchInputDeviceToTouchScreen;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint MickeyWonderCombineEntryMethod;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint MickeyWonderCombineEntryMethodPartner;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint SetupResinList;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint ResinList;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint ResinListGetCount;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint ResinListGetItem;
    [JsonInclude]
    [JsonConverter(typeof(HexNumberConverter<uint>))]
    public uint ResinListRemove;
#pragma warning restore CS0649
}