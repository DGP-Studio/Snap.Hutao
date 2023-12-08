using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class UnsafeRuntimeBehaviorTest
{
    [TestMethod]
    public unsafe void UInt32AllSetIsUInt32MaxValue()
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

    [TestMethod]
    public unsafe void UInt32LayoutIsLittleEndian()
    {
        ulong testValue = 0x1234567887654321;
        ref BuildVersion version = ref Unsafe.As<ulong, BuildVersion>(ref testValue);

        Assert.AreEqual(0x1234, version.Major);
        Assert.AreEqual(0x5678, version.Minor);
        Assert.AreEqual(0x8765, version.Patch);
        Assert.AreEqual(0x4321, version.Build);
    }

    [TestMethod]
    public unsafe void ReadOnlyStructCanBeModifiedInCtor()
    {
        TestStruct testStruct = new([4444, 7878, 5656, 1212]);

        Assert.AreEqual(4444, testStruct.Value1);
        Assert.AreEqual(7878, testStruct.Value2);
        Assert.AreEqual(5656, testStruct.Value3);
        Assert.AreEqual(1212, testStruct.Value4);
    }



    private readonly struct TestStruct
    {
        public readonly int Value1;
        public readonly int Value2;
        public readonly int Value3;
        public readonly int Value4;

        public TestStruct(List<int> list)
        {
            CollectionsMarshal.AsSpan(list).CopyTo(MemoryMarshal.CreateSpan(ref Unsafe.As<TestStruct, int>(ref this), 4));
        }
    }

    private readonly struct BuildVersion
    {
        public readonly ushort Build;
        public readonly ushort Patch;
        public readonly ushort Minor;
        public readonly ushort Major;
    }
}