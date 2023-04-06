namespace Snap.Hutao.Test;

[TestClass]
public class CSharpLanguageFeatureTest
{
    [TestMethod]
    public unsafe void NullStringFixedAlsoNullPointer()
    {
        string testStr = null!;
        fixed(char* pStr = testStr)
        {
            Assert.IsTrue(pStr == null);
        }
    }

    [TestMethod]
    public unsafe void EmptyStringFixedIsNullTerminator()
    {
        string testStr = string.Empty;
        fixed (char* pStr = testStr)
        {
            Assert.IsTrue(*pStr == '\0');
        }
    }
}