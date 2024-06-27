using System.Net.Http;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class HttpClientTest
{
    [TestMethod]
    public void RedirectionHeaderTest()
    {
        HttpClient httpClient = new(new HttpClientHandler()
        {
            UseCookies = false,
            AllowAutoRedirect = false,
        });

        using (httpClient)
        {
            using (HttpRequestMessage request = new(HttpMethod.Get, "https://api.snapgenshin.com/patch/hutao/download"))
            {
                using (HttpResponseMessage response = httpClient.Send(request))
                {
                    _ = 1;
                }
            }
        }
    }
}