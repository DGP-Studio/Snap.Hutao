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
        UnlockerIslandConfigurationWrapper wrapper = new()
        {
            Oversea = new()
            {
                FunctionOffsetFieldOfView = 0x00000000_01688E60,
                FunctionOffsetTargetFrameRate = 0x00000000_018834D0,
                FunctionOffsetFog = 0x00000000_00FB2AD0,
            },
            Chinese = new()
            {
                FunctionOffsetFieldOfView = 0x00000000_01684560,
                FunctionOffsetTargetFrameRate = 0x00000000_0187EBD0,
                FunctionOffsetFog = 0x00000000_00FAE1D0,
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