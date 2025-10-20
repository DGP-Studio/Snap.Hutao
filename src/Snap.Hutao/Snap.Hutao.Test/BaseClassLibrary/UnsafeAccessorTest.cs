using System;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public class UnsafeAccessorTest
{
    [TestMethod]
    public void UnsafeAccessorCanGetInterfaceProperty()
    {
        TestClass test = new();
        int value = InternalGetInterfaceProperty(test);
        Assert.AreEqual(3, value);
    }

    [TestMethod]
    public void BehaviorTest()
    {
        DateTimeOffset dto = new(2000, 2, 3, 4, 5, 6, TimeSpan.FromHours(7));
        Assert.AreEqual(420, RefValueGetFieldRef(ref dto));
        Assert.AreEqual(420, RefValueGetFieldRefReadonly(ref dto));
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_TestProperty")]
    private static extern int InternalGetInterfaceProperty(ITestInterface instance);

    // private readonly int _offsetMinutes;
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_offsetMinutes")]
    private static extern ref short RefValueGetFieldRef(ref DateTimeOffset dto);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_offsetMinutes")]
    private static extern ref readonly short RefValueGetFieldRefReadonly(ref DateTimeOffset dto);

    internal interface ITestInterface
    {
        internal int TestProperty { get; }
    }

    internal sealed class TestClass : ITestInterface
    {
        public int TestProperty { get; } = 3;
    }
}