namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class UnsafeRuntimeBehaviorTest
{
    [TestMethod]
    public unsafe void UInt32AllSetIs()
    {
        byte[] bytes =
#if NET8_0_OR_GREATER
            [0xFF, 0xFF, 0xFF, 0xFF];
#else
            { 0xFF, 0xFF, 0xFF, 0xFF, };
#endif

        fixed (byte* pBytes = bytes)
        {
            Assert.AreEqual(uint.MaxValue, *(uint*)pBytes);
        }
    }
}