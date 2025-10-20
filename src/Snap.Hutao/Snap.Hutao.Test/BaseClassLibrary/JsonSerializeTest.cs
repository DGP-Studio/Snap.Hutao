using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
        Assert.AreEqual(1, sample.B);
    }

    [TestMethod]
    public void EmptyStringCannotSerializeAsNumber()
    {
        SampleStringReadWriteNumberPropertyClass sample = default!;
        Assert.Throws<JsonException>(() =>
        {
            sample = JsonSerializer.Deserialize<SampleStringReadWriteNumberPropertyClass>(SampleEmptyStringObjectJson)!;
            Assert.AreEqual(0, sample.A);
        });
    }

    [TestMethod]
    public void EmptyStringCanSerializeAsUri()
    {
        SampleEmptyUriClass sample = JsonSerializer.Deserialize<SampleEmptyUriClass>(SampleEmptyStringObjectJson)!;
        Uri.TryCreate("", UriKind.RelativeOrAbsolute, out Uri? value);
        Console.WriteLine(value);
        Assert.AreEqual(value, sample.A);
    }

    [TestMethod]
    public void NumberStringKeyCanSerializeAsKey()
    {
        Dictionary<int, string> sample = JsonSerializer.Deserialize<Dictionary<int, string>>(SampleNumberKeyDictionaryJson, AlowStringNumberOptions)!;
        Assert.AreEqual("12", sample[111]);
    }

    [TestMethod]
    public void ByteArraySerializeAsBase64()
    {
        SampleByteArrayPropertyClass sample = new()
        {
            Array = [1, 2, 3, 4, 5],
        };

        string result = JsonSerializer.Serialize(sample);
        Assert.AreEqual("""{"Array":"AQIDBAU="}""", result);
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
        Assert.AreEqual("""{"A":1,"B":2}""", result);
    }

    [TestMethod]
    public void LowercaseStringCanDeserializeAsEnum()
    {
        string source = """
            {
              "Value": "a"
            }
            """;

        SampleClassHoldEnum sample = JsonSerializer.Deserialize<SampleClassHoldEnum>(source)!;
        Assert.AreEqual(SampleEnum.A, sample.Value);
    }

    [TestMethod]
    public void InitPropertyCanDeserialize()
    {
        string source = """
            {
              "Value": "A"
            }
            """;

        SampleClassHoldEnumInitOnly sample = JsonSerializer.Deserialize<SampleClassHoldEnumInitOnly>(source)!;
        Assert.AreEqual(SampleEnum.A, sample.Value);
    }

    [TestMethod]
    public void NewEmptyObjectSerializeAsEmptyObject()
    {
        object sample = new();
        string result = JsonSerializer.Serialize(sample);
        Assert.AreEqual("{}", result);
    }

    [TestMethod]
    public void StructCanDeserialize()
    {
        SampleStruct sample = JsonSerializer.Deserialize<SampleStruct>(SampleObjectJson);
        Assert.AreEqual(1, sample.A);
    }

    [TestMethod]
    public void DerivedTypeTest()
    {
        Parent p = new Child()
        {
            A = 1,
            B = 2,
            C = 3,
        };

        Dictionary<string, Parent> dict = new()
        {
            ["key1"] = p,
        };

#pragma warning disable CA1869
        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            {
                Modifiers =
                {
                    HandleDerivedType,
                }
            }
        };
#pragma warning restore CA1869

        string result = JsonSerializer.Serialize(dict, options);
        Assert.AreEqual("""{"key1":{"$type":"Child","C":3,"B":2,"A":1}}""", result);
    }

    private void HandleDerivedType(JsonTypeInfo info)
    {
        Type? current = info.Type;
        HashSet<JsonDerivedType> jsonDerivedTypes = info.PolymorphismOptions is null ? [] : [.. info.PolymorphismOptions.DerivedTypes];

        while (true)
        {
            if (current is null || current.BaseType is null)
            {
                break;
            }

            foreach (CustomAttributeData attributeData in current.BaseType.CustomAttributes)
            {
                if (attributeData.AttributeType != typeof(JsonDerivedTypeAttribute))
                {
                    continue;
                }

                if (attributeData.ConstructorArguments[0].Value is not Type derivedType)
                {
                    continue;
                }

                if (!derivedType.IsAssignableTo(info.Type))
                {
                    continue;
                }

                if (derivedType == info.Type)
                {
                    continue;
                }

                switch (attributeData.ConstructorArguments[1].Value)
                {
                    case string name:
                        jsonDerivedTypes.Add(new JsonDerivedType(derivedType, name));
                        break;
                    case int value:
                        jsonDerivedTypes.Add(new JsonDerivedType(derivedType, value));
                        break;
                    default:
                        jsonDerivedTypes.Add(new JsonDerivedType(derivedType));
                        break;
                }
            }

            current = current.BaseType;
        }

        if (jsonDerivedTypes.Count <= 0)
        {
            return;
        }

        info.PolymorphismOptions ??= new();
        foreach (JsonDerivedType derivedType in jsonDerivedTypes)
        {
            info.PolymorphismOptions.DerivedTypes.Add(derivedType);
        }
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

    [JsonConverter(typeof(JsonStringEnumConverter))]
    private enum SampleEnum
    {
        A,
        B,
    }

    private sealed class SampleClassHoldEnum
    {
        public SampleEnum Value { get; set; }
    }

    private sealed class SampleClassHoldEnumInitOnly
    {
        public SampleEnum Value { get; init; }
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

    [SuppressMessage("", "CS0649")]
    private struct SampleStruct
    {
        [JsonInclude]
        public int A;
    }

    [JsonDerivedType(typeof(Parent), nameof(Parent))]
    [JsonDerivedType(typeof(Child), nameof(Child))]
    private class GrandParent
    {
        public int A { get; set; }
    }

    private class Parent : GrandParent
    {
        public int B { get; set; }
    }

    private class Child : Parent
    {
        public int C { get; set; }
    }
}