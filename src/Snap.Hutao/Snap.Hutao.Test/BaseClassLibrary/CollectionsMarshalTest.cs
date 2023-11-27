using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public class CollectionsMarshalTest
{
    [TestMethod]
    public void DictionaryMarshalGetValueRefOrNullRefIsNullRef()
    {
#if NET8_0_OR_GREATER
        Dictionary<uint, string> dictionaryValueKeyRefValue = [];
        Dictionary<uint, uint> dictionaryValueKeyValueValue = [];
        Dictionary<string, uint> dictionaryRefKeyValueValue = [];
        Dictionary<string, string> dictionaryRefKeyRefValue = [];
#else
        Dictionary<uint, string> dictionaryValueKeyRefValue = new();
        Dictionary<uint, uint> dictionaryValueKeyValueValue = new();
        Dictionary<string, uint> dictionaryRefKeyValueValue = new();
        Dictionary<string, string> dictionaryRefKeyRefValue = new();
#endif

        Assert.IsTrue(Unsafe.IsNullRef(ref CollectionsMarshal.GetValueRefOrNullRef(dictionaryValueKeyRefValue, 1U)));
        Assert.IsTrue(Unsafe.IsNullRef(ref CollectionsMarshal.GetValueRefOrNullRef(dictionaryValueKeyValueValue, 1U)));
        Assert.IsTrue(Unsafe.IsNullRef(ref CollectionsMarshal.GetValueRefOrNullRef(dictionaryRefKeyValueValue, "no such key")));
        Assert.IsTrue(Unsafe.IsNullRef(ref CollectionsMarshal.GetValueRefOrNullRef(dictionaryRefKeyRefValue, "no such key")));
    }

    [TestMethod]
    public void DictionaryMarshalGetValueRefOrAddDefaultIsDefault()
    {
#if NET8_0_OR_GREATER
        Dictionary<uint, string> dictionaryValueKeyRefValue = [];
        Dictionary<uint, uint> dictionaryValueKeyValueValue = [];
        Dictionary<string, uint> dictionaryRefKeyValueValue = [];
        Dictionary<string, string> dictionaryRefKeyRefValue = [];
#else
        Dictionary<uint, string> dictionaryValueKeyRefValue = new();
        Dictionary<uint, uint> dictionaryValueKeyValueValue = new();
        Dictionary<string, uint> dictionaryRefKeyValueValue = new();
        Dictionary<string, string> dictionaryRefKeyRefValue = new();
#endif

        Assert.IsTrue(CollectionsMarshal.GetValueRefOrAddDefault(dictionaryValueKeyRefValue, 1U, out _) == default);
        Assert.IsTrue(CollectionsMarshal.GetValueRefOrAddDefault(dictionaryValueKeyValueValue, 1U, out _) == default);
        Assert.IsTrue(CollectionsMarshal.GetValueRefOrAddDefault(dictionaryRefKeyValueValue, "no such key", out _) == default);
        Assert.IsTrue(CollectionsMarshal.GetValueRefOrAddDefault(dictionaryRefKeyRefValue, "no such key", out _) == default);
    }
}