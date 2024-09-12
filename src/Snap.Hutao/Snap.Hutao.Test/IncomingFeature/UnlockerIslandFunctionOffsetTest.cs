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
        // public void set_fieldOfView(float value) -> jmp xxxxxxxx
        // public static void set_targetFrameRate(int value)
        // public static void set_enableFogRendering(bool value)

        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Oversea = new()
            {
                FunctionOffsetSetFieldOfView = 0x00000000_0165A1D0,
                FunctionOffsetSetTargetFrameRate = 0x00000000_10FF7800,
                FunctionOffsetSetEnableFogRendering = 0x00000000_10F3ACE0,
            },
            Chinese = new()
            {
                FunctionOffsetSetFieldOfView = 0x00000000_0165F1D0,
                FunctionOffsetSetTargetFrameRate = 0x00000000_10FF7D20,
                FunctionOffsetSetEnableFogRendering = 0x00000000_10F31FA0,
            },
        };

        Console.WriteLine(JsonSerializer.Serialize(wrapper, Options));
    }

    private sealed class UnlockerIslandConfigurationWrapper
    {
        public required UnlockerIslandConfiguration Oversea { get; set; }

        public required UnlockerIslandConfiguration Chinese { get; set; }
    }

    private sealed class UnlockerIslandConfiguration
    {
        public required uint FunctionOffsetSetFieldOfView { get; set; }

        public required uint FunctionOffsetSetTargetFrameRate { get; set; }

        public required uint FunctionOffsetSetEnableFogRendering { get; set; }
    }
}