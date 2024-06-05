using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Snap.Hutao.Test.RuntimeBehavior;

[TestClass]
public sealed class HttpClientBehaviorTest
{
    private const int MessageNotYetSent = 0;

    [TestMethod]
    public async Task RetrySendHttpRequestMessage()
    {
        using (HttpClient httpClient = new())
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Post, "https://jsonplaceholder.typicode.com/posts");
            JsonContent content = JsonContent.Create(new Point(12, 34));
            requestMessage.Content = content;
            using (requestMessage)
            {
                await httpClient.SendAsync(requestMessage).ConfigureAwait(false);
            }

            Interlocked.Exchange(ref GetPrivateSendStatus(requestMessage), MessageNotYetSent);
            Volatile.Write(ref GetPrivateDisposed(content), false);
            await httpClient.SendAsync(requestMessage).ConfigureAwait(false);
        }
    }

    // private int _sendStatus
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_sendStatus")]
    private static extern ref int GetPrivateSendStatus(HttpRequestMessage message);

    // private bool _disposed
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposed")]
    private static extern ref bool GetPrivateDisposed(HttpRequestMessage message);

    // private bool _disposed
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposed")]
    private static extern ref bool GetPrivateDisposed(HttpContent content);
}