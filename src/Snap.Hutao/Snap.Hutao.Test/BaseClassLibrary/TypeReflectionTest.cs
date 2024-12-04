using System;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class TypeReflectionTest
{
    [TestMethod]
    public void TypeCodeOfEnumIsUserlyingTypeTypeCode()
    {
        Assert.AreEqual(Type.GetTypeCode(typeof(TestEnum)), TypeCode.Int32);
    }

    private enum TestEnum
    {
        A,
        B,
    }
}