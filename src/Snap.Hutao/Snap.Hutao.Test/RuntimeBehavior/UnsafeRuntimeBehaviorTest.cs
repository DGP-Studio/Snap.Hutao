namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class UnsafeRuntimeBehaviorTest
{
    [TestMethod]
    public unsafe void UInt32AllSetIs()
    {
        byte[] bytes = { 0xFF, 0xFF, 0xFF, 0xFF, };

        fixed (byte* pBytes = bytes)
        {
            Assert.AreEqual(uint.MaxValue, *(uint*)pBytes);
        }
    }
}