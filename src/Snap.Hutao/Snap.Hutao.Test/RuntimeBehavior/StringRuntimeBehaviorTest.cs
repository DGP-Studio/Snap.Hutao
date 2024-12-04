using System;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class StringRuntimeBehaviorTest
{
    [TestMethod]
    public unsafe void NullStringFixedIsNullPointer()
    {
        string testStr = null!;
        fixed (char* pStr = testStr)
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
    public unsafe void EmptyStringAsSpanIsZeroLength()
    {
        string testStr = string.Empty;
        ReadOnlySpan<char> testSpan = testStr;
        Assert.IsTrue(testSpan.Length == 0);
    }
}