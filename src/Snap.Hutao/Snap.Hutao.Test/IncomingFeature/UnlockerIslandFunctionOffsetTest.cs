using System;
using System.Text.Json;

namespace Snap.Hutao.Test.IncomingFeature;

[TestClass]
public class UnlockerIslandFunctionOffsetTest
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
    };

    [TestMethod]
    public void GenerateJson()
    {
        // FunctionOffsetMickeyWonderMethod:
        // public static byte[] AnonymousMethod43(int nType) -> jmp xxxxxxxx -> another xref to xxxxxxxx
        // FunctionOffsetMickeyWonderMethodPartner:
        // public static string get_unityVersion() -> jmp
        // FunctionOffsetMickeyWonderMethodPartner2:
        // "4C 8B 05 ?? ?? ?? ?? 4C 89 F1 48 89 FA E8 ?? ?? ?? ?? 90 48 83 C4 28 5B 5F 5E 41 5E C3"
        // FunctionOffsetSetFieldOfView:
        // Camera: public void set_fieldOfView(float value) -> jmp xxxxxxxx
        // FunctionOffsetSetTargetFrameRate:
        // public static void set_targetFrameRate(int value) -> jmp xxxxxxxxx (to the end)
        // FunctionOffsetSetEnableFogRendering:
        // public static void set_enableFogRendering(bool value) -> jmp xxxxxxxxx (to the end)
        // FunctionOffsetOpenTeam: FunctionOffsetOpenTeamPageAccordingly: GOD HELP US
        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Chinese = new()
            {
                MickeyWonderMethod = 0x08474680,
                MickeyWonderMethodPartner = 0x00950C30,
                MickeyWonderMethodPartner2 = 0x0588D680,
                SetFieldOfView = 0x013F38A0,
                SetTargetFrameRate = 0x015DB980,
                SetEnableFogRendering = 0x0136FB00,
                OpenTeam = 0x08699B90,
                OpenTeamPageAccordingly = 0x08695AE0,
            },
            Oversea = new()
            {
                MickeyWonderMethod = 0x0916F3B0,
                MickeyWonderMethodPartner = 0x00955AB0,
                MickeyWonderMethodPartner2 = 0x05892DE0,
                SetFieldOfView = 0x013F87C0,
                SetTargetFrameRate = 0x015E08A0,
                SetEnableFogRendering = 0x013FBA20,
                OpenTeam = 0x086A32A0,
                OpenTeamPageAccordingly = 0x086A4C60,
            },
        };

        Console.WriteLine(JsonSerializer.Serialize(wrapper, Options));
    }

    private sealed class UnlockerIslandConfigurationWrapper
    {
        public required UnlockerIslandFunctionOffset Chinese { get; set; }

        public required UnlockerIslandFunctionOffset Oversea { get; set; }
    }

    private sealed class UnlockerIslandFunctionOffset
    {
        public required uint MickeyWonderMethod { get; set; }

        public required uint MickeyWonderMethodPartner { get; set; }

        public required uint MickeyWonderMethodPartner2 { get; set; }

        public required uint SetFieldOfView { get; set; }

        public required uint SetTargetFrameRate { get; set; }

        public required uint SetEnableFogRendering { get; set; }

        public required uint OpenTeam { get; set; }

        public required uint OpenTeamPageAccordingly { get; set; }
    }
}