using System;
using System.Collections.Generic;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class ForEachRuntimeBehaviorTest
{
    [TestMethod]
    public void ListOfStringCanEnumerateAsReadOnlySpanOfChar()
    {
        List<string> strings =
#if NET8_0_OR_GREATER
            ["a", "b", "c"];
#else
            new() { "a", "b", "c" };
#endif
        int count = 0;
        foreach (ReadOnlySpan<char> chars in strings)
        {
            Assert.IsTrue(chars.Length == 1);
            ++count;
        }

        Assert.AreEqual(3, count);
    }
}