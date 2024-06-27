using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class JsonSerializeTest
{
    private readonly JsonSerializerOptions AlowStringNumberOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    private const string SampleObjectJson = """
        {
            "A" :1
        }
        """;

    private const string SampleEmptyStringObjectJson = """
        {
            "A" : ""
        }
        """;

    private const string SampleNumberKeyDictionaryJson = """
        {
            "111" : "12",
            "222" : "34"
        }
        """;

    [TestMethod]
    public void DelegatePropertyCanSerialize()
    {
        SampleDelegatePropertyClass sample = JsonSerializer.Deserialize<SampleDelegatePropertyClass>(SampleObjectJson)!;
        Assert.AreEqual(sample.B, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void EmptyStringCannotSerializeAsNumber()
    {
        SampleStringReadWriteNumberPropertyClass sample = JsonSerializer.Deserialize<SampleStringReadWriteNumberPropertyClass>(SampleEmptyStringObjectJson)!;
        Assert.AreEqual(sample.A, 0);
    }

    [TestMethod]
    public void EmptyStringCanSerializeAsUri()
    {
        SampleEmptyUriClass sample = JsonSerializer.Deserialize<SampleEmptyUriClass>(SampleEmptyStringObjectJson)!;
        Uri.TryCreate("", UriKind.RelativeOrAbsolute, out Uri? value);
        Console.WriteLine(value);
        Assert.AreEqual(sample.A, value);
    }

    [TestMethod]
    public void NumberStringKeyCanSerializeAsKey()
    {
        Dictionary<int, string> sample = JsonSerializer.Deserialize<Dictionary<int, string>>(SampleNumberKeyDictionaryJson, AlowStringNumberOptions)!;
        Assert.AreEqual(sample[111], "12");
    }

    [TestMethod]
    public void ByteArraySerializeAsBase64()
    {
        SampleByteArrayPropertyClass sample = new()
        {
            Array = [1, 2, 3, 4, 5],
        };

        string result = JsonSerializer.Serialize(sample);
        Assert.AreEqual(result, """{"Array":"AQIDBAU="}""");
    }

    [TestMethod]
    public void InterfaceDefaultMethodCanSerializeActualInstanceMember()
    {
        ISampleInterface sample = new SampleClassImplementedInterface()
        {
            A = 1,
            B = 2,
        };

        string result = sample.ToJson();
        Console.WriteLine(result);
        Assert.AreEqual(result, """{"A":1,"B":2}""");
    }

    private sealed class SampleDelegatePropertyClass
    {
        public int A { get => B; set => B = value; }
        public int B { get; set; }
    }

    private sealed class SampleStringReadWriteNumberPropertyClass
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
        public int A { get; set; }
    }

    private sealed class SampleEmptyUriClass
    {
        public Uri A { get; set; } = default!;
    }

    private sealed class SampleByteArrayPropertyClass
    {
        public byte[]? Array { get; set; }
    }

    private sealed class SampleClassImplementedInterface : ISampleInterface
    {
        public int A { get; set; }

        public int B { get; set; }
    }

    [JsonDerivedType(typeof(SampleClassImplementedInterface))]
    private interface ISampleInterface
    {
        int A { get; set; }

        string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}