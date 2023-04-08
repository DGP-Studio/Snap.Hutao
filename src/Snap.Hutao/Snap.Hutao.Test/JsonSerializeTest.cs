using System.Text.Json;

namespace Snap.Hutao.Test;

[TestClass]
public class JsonSerializeTest
{
    private const string SmapleObjectJson = """
        {
            "A" :1
        }
        """;

    [TestMethod]
    public void DelegatePropertyCanSerialize()
    {
        Sample sample = JsonSerializer.Deserialize<Sample>(SmapleObjectJson)!;
        Assert.AreEqual(sample.B, 1);
    }

    private class Sample
    {
        public int A { get => B; set => B = value; }
        public int B { get; set; }
    }
}