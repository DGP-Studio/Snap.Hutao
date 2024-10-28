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
        // FunctionOffsetMickeyWonderMethod: public static byte[] AnonymousMethod43(int nType) -> jmp xxxxxxxx -> another xref to xxxxxxxx
        // FunctionOffsetMickeyWonderMethodPartner: AnonymousMethod43 jmp -> ret jmp
        // FunctionOffsetSetFieldOfView: public void set_fieldOfView(float value) -> jmp xxxxxxxx
        // FunctionOffsetSetTargetFrameRate: public static void set_targetFrameRate(int value) -> jmp xxxxxxxxx (to the end)
        // FunctionOffsetSetEnableFogRendering: public static void set_enableFogRendering(bool value) -> jmp xxxxxxxxx (to the end)
        // FunctionOffsetOpenTeam: public static void AJODMEAHOGI()
        // FunctionOffsetOpenTeamPageAccordingly: public static void OEEFGJDOCJJ(bool KCBOKOCOGEI = true)
        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Chinese = new()
            {
                FunctionOffsetMickeyWonderMethod = 0x099D1D80,
                FunctionOffsetMickeyWonderMethodPartner = 0x002ED400,
                FunctionOffsetSetFieldOfView = 0x01136D30,
                FunctionOffsetSetTargetFrameRate = 0x0131E600,
                FunctionOffsetSetEnableFogRendering = 0x015DC790,
                FunctionOffsetOpenTeam = 0x07806530,
                FunctionOffsetOpenTeamPageAccordingly = 0x0781D3F0,
            },
            Oversea = new()
            {
                FunctionOffsetMickeyWonderMethod = 0x09B37E60,
                FunctionOffsetMickeyWonderMethodPartner = 0x002ED390,
                FunctionOffsetSetFieldOfView = 0x01136F30,
                FunctionOffsetSetTargetFrameRate = 0x0131E800,
                FunctionOffsetSetEnableFogRendering = 0x015DC990,
                FunctionOffsetOpenTeam = 0x0777E4F0,
                FunctionOffsetOpenTeamPageAccordingly = 0x07779870,
            },
        };

        Console.WriteLine(JsonSerializer.Serialize(wrapper, Options));
    }

    private sealed class UnlockerIslandConfigurationWrapper
    {
        public required UnlockerIslandConfiguration Chinese { get; set; }

        public required UnlockerIslandConfiguration Oversea { get; set; }
    }

    private sealed class UnlockerIslandConfiguration
    {
        public required uint FunctionOffsetMickeyWonderMethod { get; set; }

        public required uint FunctionOffsetMickeyWonderMethodPartner { get; set; }

        public required uint FunctionOffsetSetFieldOfView { get; set; }

        public required uint FunctionOffsetSetTargetFrameRate { get; set; }

        public required uint FunctionOffsetSetEnableFogRendering { get; set; }

        public required uint FunctionOffsetOpenTeam { get; set; }

        public required uint FunctionOffsetOpenTeamPageAccordingly { get; set; }
    }
}