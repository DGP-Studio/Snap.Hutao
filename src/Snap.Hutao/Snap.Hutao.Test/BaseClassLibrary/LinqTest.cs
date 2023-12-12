using System;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class LinqTest
{
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void LinqOrderByWithWrapperStructThrow()
    {
        List<MyUInt32> list = [1, 5, 2, 6, 3, 7, 4, 8];
        string result = string.Join(", ", list.OrderBy(i => i).Select(i => i.Value));
        Console.WriteLine(result);
    }

    private readonly struct MyUInt32
    {
        public readonly uint Value;

        public MyUInt32(uint value)
        {
            Value = value;
        }

        public static implicit operator MyUInt32(uint value)
        {
            return new(value);
        }
    }
}