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

    private const string SmapleNumberObjectJson = """
        {
            "A" : ""
        }
        """;

    private const string SmapleNumberDictionaryJson = """
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
    public void EmptyStringCannotSerializeAsNumber()
    {
        bool caught = false;
        try
        {
            // Throw
            StringNumberSample sample = JsonSerializer.Deserialize<StringNumberSample>(SmapleNumberObjectJson)!;
            Assert.AreEqual(sample.A, 0);
        }
        catch
        {
            caught = true;
        }

        Assert.IsTrue(caught);
    }

    [TestMethod]
    public void NumberStringKeyCanSerializeAsKey()
    {
        JsonSerializerOptions options = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        Dictionary<int,string> sample = JsonSerializer.Deserialize<Dictionary<int, string>>(SmapleNumberDictionaryJson, options)!;
        Assert.AreEqual(sample[111], "12");
    }

    private class Sample
    {
        public int A { get => B; set => B = value; }
        public int B { get; set; }
    }

    private class StringNumberSample
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public int A { get; set; }
    }
}