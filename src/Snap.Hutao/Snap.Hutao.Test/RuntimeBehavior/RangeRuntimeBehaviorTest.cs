using System;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class RangeRuntimeBehaviorTest
{
    [TestMethod]
    public void RangeTrimLastOne()
    {
        int[] array = [1, 2, 3, 4];
        int[] test = [1, 2, 3];
        int[] result = array[..^1];
        Assert.AreEqual(3, result.Length);
        Assert.IsTrue(MemoryExtensions.SequenceEqual<int>(test, result));
    }
}