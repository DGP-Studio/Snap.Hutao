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
        // public void set_fieldOfView(float value)
        // public static void set_targetFrameRate(int value)
        // public static void set_enableFogRendering(bool value)

        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Oversea = new()
            {
                FunctionOffsetFieldOfView = 0x00000000_01B0A690,
                FunctionOffsetTargetFrameRate = 0x00000000_10FF7800,
                FunctionOffsetFog = 0x00000000_10F3ACE0,
            },
            Chinese = new()
            {
                FunctionOffsetFieldOfView = 0x00000000_01B0F690,
                FunctionOffsetTargetFrameRate = 0x00000000_10FF7D20,
                FunctionOffsetFog = 0x00000000_10F31FA0,
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
        public required uint FunctionOffsetFieldOfView { get; set; }

        public required uint FunctionOffsetTargetFrameRate { get; set; }

        public required uint FunctionOffsetFog { get; set; }
    }
}