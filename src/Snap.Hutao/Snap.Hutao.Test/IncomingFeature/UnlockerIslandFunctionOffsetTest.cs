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
                MickeyWonderMethod = 135331312,
                MickeyWonderMethodPartner = 4622768,
                MickeyWonderMethodPartner2 = 109471280,
                SetFieldOfView = 15436752,
                SetEnableFogRendering = 276053264,
                SetTargetFrameRate = 276586352,
                OpenTeam = 102865760,
                OpenTeamPageAccordingly = 102804496,
                CheckCanEnter = 123456848,
                SetupView = 0x0E521BD0,
                FindGameObject = 0x107B3AE0,
                SetActive = 0x107B3840,
            },
            Oversea = new()
            {
                MickeyWonderMethod = 138413872,
                MickeyWonderMethodPartner = 4616352,
                MickeyWonderMethodPartner2 = 108665520,
                SetFieldOfView = 15428576,
                SetEnableFogRendering = 276114192,
                SetTargetFrameRate = 276640176,
                OpenTeam = 101850864,
                OpenTeamPageAccordingly = 127059184,
                CheckCanEnter = 110655936,
                SetupView = 0x06C73730,
                FindGameObject = 0x107C0CF0,
                SetActive = 0x107C0A50,
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

        public required uint SetupView { get; set; }

        public required uint FindGameObject { get; set; }

        public required uint SetActive { get; set; }
    }
}