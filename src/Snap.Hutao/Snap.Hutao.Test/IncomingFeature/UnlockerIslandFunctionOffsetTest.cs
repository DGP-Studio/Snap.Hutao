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
        // public static void set_targetFrameRate(int value) -> jmp xxxxxxxxx (to the end)
        // public static void set_enableFogRendering(bool value) -> jmp xxxxxxxxx (to the end)

        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Chinese = new()
            {
                FunctionOffsetSetFieldOfView = 0x01136D30,
                FunctionOffsetSetTargetFrameRate = 0x0131E600,
                FunctionOffsetSetEnableFogRendering = 0x015DC790,
            },
            Oversea = new()
            {
                FunctionOffsetSetFieldOfView = 0x01136F30,
                FunctionOffsetSetTargetFrameRate = 0x0131E800,
                FunctionOffsetSetEnableFogRendering = 0x015DC990,
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