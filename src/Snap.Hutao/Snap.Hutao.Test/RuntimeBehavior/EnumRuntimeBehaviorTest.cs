using System;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class EnumRuntimeBehaviorTest
{
    [TestMethod]
    public void EnumParseCanNotHandleEmptyString()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Enum.Parse<EnumA>(string.Empty);
        });
    }

    [TestMethod]
    public void EnumParseCanHandleNumberString()
    {
        EnumA a = Enum.Parse<EnumA>("2");
        Assert.AreEqual(a, EnumA.ValueB);
    }

    [TestMethod]
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