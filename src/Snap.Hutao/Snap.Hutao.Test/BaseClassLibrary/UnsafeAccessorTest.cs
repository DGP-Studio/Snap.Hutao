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

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_TestProperty")]
    private static extern int InternalGetInterfaceProperty(ITestInterface instance);

    internal interface ITestInterface
    {
        internal int TestProperty { get; }
    }

    internal sealed class TestClass : ITestInterface
    {
        public int TestProperty { get; } = 3;
    }
}