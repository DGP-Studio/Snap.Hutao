using System.Collections.Generic;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class ListTest
{
    [TestMethod]
    public void IndexOfNullIsNegativeOne()
    {
        List<object> list = [new()];
        Assert.AreEqual(-1, list.IndexOf(default!));
    }
}