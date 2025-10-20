using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class UnsafeRuntimeBehaviorTest
{
    [TestMethod]
    public unsafe void UInt32AllSetIsUInt32MaxValue()
    {
        byte[] bytes = [0xFF, 0xFF, 0xFF, 0xFF];
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

    [TestMethod]
    public unsafe void UnsafeLiteralUtf8StringReference()
    {
        void* ptr = Unsafe.AsPointer(ref MemoryMarshal.GetReference("test"u8));
        GC.Collect(GC.MaxGeneration);
        ReadOnlySpan<byte> bytes = MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ptr);
        Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
    }

    [TestMethod]
    public unsafe void UnsafeSizeInt32ToRectInt32Test()
    {
        RectInt32 rectInt32 = ToRectInt32(new(100, 200));
        Assert.AreEqual(rectInt32.X, 0);
        Assert.AreEqual(rectInt32.Y, 0);
        Assert.AreEqual(rectInt32.Width, 100);
        Assert.AreEqual(rectInt32.Height, 200);

        unsafe RectInt32 ToRectInt32(SizeInt32 sizeInt32)
        {
            byte* pBytes = stackalloc byte[sizeof(RectInt32)];
            *(SizeInt32*)(pBytes + 8) = sizeInt32;
            return *(RectInt32*)pBytes;
        }
    }

    private struct RectInt32
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public RectInt32(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }

    private struct SizeInt32
    {
        public int Width;
        public int Height;

        public SizeInt32(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    [SuppressMessage("", "CS0649")]
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