using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class JsonSerializeTest
{
    public TestContext? TestContext { get; set; }

    private readonly JsonSerializerOptions AlowStringNumberOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private const string SmapleObjectJson = """
        {
            "A" :1
        }
        """;

    private const string SmapleEmptyStringObjectJson = """
        {
            "A" : ""
        }
        """;

    private const string SmapleNumberKeyDictionaryJson = """
        {
            "111" : "12",
            "222" : "34"
        }
        """;

    [TestMethod]
    public void DelegatePropertyCanSerialize()
    {
        Sample sample = JsonSerializer.Deserialize<Sample>(SmapleObjectJson)!;
        Assert.AreEqual(sample.B, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void EmptyStringCannotSerializeAsNumber()
    {
        StringNumberSample sample = JsonSerializer.Deserialize<StringNumberSample>(SmapleEmptyStringObjectJson)!;
        Assert.AreEqual(sample.A, 0);
    }

    [TestMethod]
    public void NumberStringKeyCanSerializeAsKey()
    {
        Dictionary<int, string> sample = JsonSerializer.Deserialize<Dictionary<int, string>>(SmapleNumberKeyDictionaryJson, AlowStringNumberOptions)!;
        Assert.AreEqual(sample[111], "12");
    }

    [TestMethod]
    public void ByteArraySerializeAsBase64()
    {
        byte[] array =
#if NET8_0_OR_GREATER
            [1, 2, 3, 4, 5];
#else
            { 1, 2, 3, 4, 5 };
#endif
        ByteArraySample sample = new()
        {
            Array = array,
        };

        string result = JsonSerializer.Serialize(sample);
        TestContext!.WriteLine($"ByteArray Serialize Result: {result}");
        Assert.AreEqual(result, """
            {"Array":"AQIDBAU="}
            """);
    }

    private sealed class Sample
    {
        public int A { get => B; set => B = value; }
        public int B { get; set; }
    }

    private sealed class StringNumberSample
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public int A { get; set; }
    }

    private sealed class ByteArraySample
    {
        public byte[]? Array { get; set; }
    }
}