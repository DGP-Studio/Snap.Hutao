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

    [TestMethod]
    public void DelegatePropertyCanSerialize()
    {
        Sample sample = JsonSerializer.Deserialize<Sample>(SmapleObjectJson)!;
        Assert.AreEqual(sample.B, 1);
    }

    [TestMethod]
    public void EmptyStringCanSerializeAsNumber()
    {
        // Throw
        StringNumberSample sample = JsonSerializer.Deserialize<StringNumberSample>(SmapleNumberObjectJson)!;
        Assert.AreEqual(sample.A, 0);
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