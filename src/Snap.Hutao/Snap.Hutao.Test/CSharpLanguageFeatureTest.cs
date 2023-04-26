using System;
using System.Collections.Generic;

namespace Snap.Hutao.Test;

[TestClass]
public class CSharpLanguageFeatureTest
{
    [TestMethod]
    public unsafe void NullStringFixedAlsoNullPointer()
    {
        string testStr = null!;
        fixed(char* pStr = testStr)
        {
            Assert.IsTrue(pStr == null);
        }
    }

    [TestMethod]
    public unsafe void EmptyStringFixedIsNullTerminator()
    {
        string testStr = string.Empty;
        fixed (char* pStr = testStr)
        {
            Assert.IsTrue(*pStr == '\0');
        }
    }

    [TestMethod]
    public void EnumParseCanNotHandleEmptyString()
    {
        bool caught = false;
        try
        {
            Enum.Parse<EnumA>(string.Empty);
        }
        catch (ArgumentException)
        {
            caught = true;
        }

        Assert.IsTrue(caught);
    }

    [TestMethod]
    public void EnumParseCanHandleNumberString()
    {
        EnumA a = Enum.Parse<EnumA>("2");
        Assert.AreEqual(a, EnumA.ValueB);
    }

    [TestMethod]
    public void EnumToStringDecimal()
    {
        Assert.AreEqual("2", EnumA.ValueB.ToString("D"));
    }

    private enum EnumA
    {
        None = 0,
        ValueA = 1,
        ValueB = 2,
        ValueC = 3,
    }

    [TestMethod]
    public void GetTwiceOnPropertyResultsNotSame()
    {
        Assert.AreNotEqual(UUID, UUID);
    }

    [TestMethod]
    public void ListOfStringCanEnumerateAsReadOnlySpanOfChar()
    {
        List<string> strings = new()
        {
            "a", "b", "c"
        };

        int count = 0;
        foreach (ReadOnlySpan<char> chars in strings)
        {
            Assert.IsTrue(chars.Length == 1);
            ++count;
        }

        Assert.AreEqual(3, count);
    }

    public static Guid UUID { get => Guid.NewGuid(); }
}