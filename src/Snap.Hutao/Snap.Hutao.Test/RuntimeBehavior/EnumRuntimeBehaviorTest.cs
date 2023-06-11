using System;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
internal sealed class EnumRuntimeBehaviorTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void EnumParseCanNotHandleEmptyString()
    {
        Enum.Parse<EnumA>(string.Empty);
    }

    [TestMethod]
    public void EnumParseCanHandleNumberString()
    {
        EnumA a = Enum.Parse<EnumA>("2");
        Assert.AreEqual(a, EnumA.ValueB);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
    public void EnumToStringDecimal()
    {
        Assert.AreEqual("2", EnumA.ValueB.ToString("D"));
    }

    private enum EnumA
    {
        None = 0,
        ValueA = 1,
        ValueB = 2,
        ValueC = 3,
    }
}