using System.Net.Http;

namespace Snap.Hutao.Test.BaseClassLibrary;

[TestClass]
public sealed class HttpClientTest
{
    [TestMethod]
    public void RedirectionHeaderTest()
    {
        HttpClientHandler handler = new()
        {
            UseCookies = false,
            AllowAutoRedirect = false,
        };

        using (handler)
        {
            using (HttpClient httpClient = new(handler))
            {
                using (HttpRequestMessage request = new(HttpMethod.Get, "https://api.snapgenshin.com/patch/hutao/download"))
                {
                    using (HttpResponseMessage response = httpClient.Send(request))
                    {
                        _ = response;
                    }
                }
            }
        }
    }
}