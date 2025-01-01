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
        // MickeyWonderMethod:
        // public static byte[] AnonymousMethod43(int nType) -> jmp xxxxxxxx -> another xref to xxxxxxxx
        // MickeyWonderMethodPartner:
        // public static string get_unityVersion() -> jmp
        // MickeyWonderMethodPartner2:
        // "4C 8B 05 ?? ?? ?? ?? 4C 89 F1 48 89 FA E8 ?? ?? ?? ?? 90 48 83 C4 28 5B 5F 5E 41 5E C3"
        // SetFieldOfView:
        // Camera: public void set_fieldOfView(float value) -> jmp xxxxxxxx
        // SetTargetFrameRate:
        // public static void set_targetFrameRate(int value) -> jmp xxxxxxxxx (to the end)
        // SetEnableFogRendering:
        // public static void set_enableFogRendering(bool value) -> jmp xxxxxxxxx (to the end)
        // OpenTeam: OpenTeamPageAccordingly: CheckCanEnter GOD HELP US
        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Chinese = new()
            {
                MickeyWonderMethod = 0x0929D180,
                MickeyWonderMethodPartner = 0x00460FE0,
                MickeyWonderMethodPartner2 = 0x05BCEC60,
                SetFieldOfView = 0x00F0A370,
                SetTargetFrameRate = 0x00F0EC30,
                SetEnableFogRendering = 0x00F0D5A0,
                OpenTeam = 0x07C97130,
                OpenTeamPageAccordingly = 0x07CB71C0,
                CheckCanEnter = 0x069F0C00
            },
            Oversea = new()
            {
                MickeyWonderMethod = 0x08FC3AA0,
                MickeyWonderMethodPartner = 0x0045F640,
                MickeyWonderMethodPartner2 = 0x05D1D050,
                SetFieldOfView = 0x00F08370,
                SetTargetFrameRate = 0x00F0CC30,
                SetEnableFogRendering = 0x00F0B5A0,
                OpenTeam = 0x07EB04C0,
                OpenTeamPageAccordingly = 0x07EC3A40,
                CheckCanEnter = 0x06147B80
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

        public required uint SetEnableFogRendering { get; set; }

        public required uint SetTargetFrameRate { get; set; }

        public required uint OpenTeam { get; set; }

        public required uint OpenTeamPageAccordingly { get; set; }

        public required uint CheckCanEnter { get; set; }
    }
}