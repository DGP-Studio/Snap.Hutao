namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public class PartialPropertyTest
{
    [TestMethod]
    public void Test()
    {
        PartialPropertyClass partialPropertyClass = new();
        Assert.AreEqual("Test", partialPropertyClass.Property);
    }

    private partial class PartialPropertyClass
    {
        public partial string Property { get; }
    }

    partial class PartialPropertyClass
    {
        public PartialPropertyClass()
        {
            Property = "Test";
        }

        public partial string Property { get => field; }
    }
}