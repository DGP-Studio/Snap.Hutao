// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Globalization;

namespace Snap.Hutao.Model;

internal sealed class NameCultureInfoValue : NameValue<CultureInfo>
{
    public NameCultureInfoValue(string name, CultureInfo value, LocalizationSource localizationSource)
        : base(name, value)
    {
        IsMaintainedByHutao = localizationSource.HasFlag(LocalizationSource.Hutao);
        IsMaintainedByCrowdin = localizationSource.HasFlag(LocalizationSource.Crowdin);
        IsMaintainedByGemini = localizationSource.HasFlag(LocalizationSource.Gemini);
    }

    public bool IsMaintainedByHutao { get; }

    public bool IsMaintainedByCrowdin { get; }

    public bool IsMaintainedByGemini { get; }
}