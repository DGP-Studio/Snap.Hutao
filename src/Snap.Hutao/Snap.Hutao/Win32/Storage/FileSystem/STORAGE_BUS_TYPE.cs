// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Win32.Storage.FileSystem;

internal enum STORAGE_BUS_TYPE
{
    BusTypeUnknown = 0,
    BusTypeScsi = 1,
    BusTypeAtapi = 2,
    BusTypeAta = 3,
    BusType1394 = 4,
    BusTypeSsa = 5,
    BusTypeFibre = 6,
    BusTypeUsb = 7,
    BusTypeRAID = 8,
    BusTypeiScsi = 9,
    BusTypeSas = 10,
    BusTypeSata = 11,
    BusTypeSd = 12,
    BusTypeMmc = 13,
    BusTypeVirtual = 14,
    BusTypeFileBackedVirtual = 15,
    BusTypeSpaces = 16,
    BusTypeNvme = 17,
    BusTypeSCM = 18,
    BusTypeUfs = 19,
    BusTypeMax = 20,
    BusTypeMaxReserved = 127,
}