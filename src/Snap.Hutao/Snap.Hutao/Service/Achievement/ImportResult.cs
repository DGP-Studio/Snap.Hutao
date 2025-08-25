// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.Achievement;

internal readonly struct ImportResult
{
    public readonly int Add;
    public readonly int Update;
    public readonly int Remove;

    public ImportResult(int add, int update, int remove)
    {
        Add = add;
        Update = update;
        Remove = remove;
    }

    public override string ToString()
    {
        return SH.FormatServiceAchievementImportResult(Add, Update, Remove);
    }
}