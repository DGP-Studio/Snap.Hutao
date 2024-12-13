using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public class ImmutableCollectionTest
{
    [TestMethod]
    public void ImmutableArrayUnsafeRefWrite()
    {
        ImmutableArray<int> array = [1, 2, 3, 4, 5, 6, 7];
        Unsafe.AsRef(in array.AsSpan()[3]) = 8;
        Assert.AreEqual(8, array[3]);
    }
}