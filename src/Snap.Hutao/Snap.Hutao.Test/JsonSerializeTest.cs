using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Test;

[TestClass]
public class JsonSerializeTest
{
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
        JsonSerializerOptions options = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        Dictionary<int, string> sample = JsonSerializer.Deserialize<Dictionary<int, string>>(SmapleNumberKeyDictionaryJson, options)!;
        Assert.AreEqual(sample[111], "12");
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
}