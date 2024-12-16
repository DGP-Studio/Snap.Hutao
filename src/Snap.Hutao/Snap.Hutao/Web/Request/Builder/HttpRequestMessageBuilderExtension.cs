// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestMessageBuilderExtension
{
    private const string RequestErrorMessage = "请求异常已忽略: {Uri}";

    internal static HttpRequestMessageBuilder Resurrect(this HttpRequestMessageBuilder builder)
    {
        builder.HttpRequestMessage.Resurrect();
        return builder;
    }

    internal static async ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger, CancellationToken token)
        where TResult : class
    {
        HttpContext context = new()
        {
            HttpClient = httpClient,
            Logger = logger,
            RequestAborted = token,
        };

        using (context)
        {
            await SendAsync(builder, context).ConfigureAwait(false);

            StringBuilder messageBuilder = new();
            bool showInfo = true;

            try
            {
                context.Exception?.Throw();
                ArgumentNullException.ThrowIfNull(context.Response);

                context.Response.EnsureSuccessStatusCode();
                showInfo = false;
                TResult? body = await builder.HttpContentSerializer.DeserializeAsync<TResult>(context.Response.Content, token).ConfigureAwait(false);
                return new(context.Response.Headers, body);
            }
            catch (OperationCanceledException)
            {
                showInfo = false;
                throw;
            }
            catch (Exception ex)
            {
                ExceptionFormat.Format(messageBuilder, ex);
                logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
                return new(context.Response?.Headers, default);
            }
            finally
            {
                if (showInfo)
                {
                    builder.ServiceProvider.GetRequiredService<IInfoBarService>().Error(messageBuilder.ToString());
                }
            }
        }
    }

    internal static async ValueTask SendAsync(this HttpRequestMessageBuilder builder, HttpContext context)
    {
        string? baseUrl = builder.HttpRequestMessage.RequestUri?.GetLeftPart(UriPartial.Path);
        try
        {
            context.Request = builder.HttpRequestMessage;
            context.Response = await context.HttpClient.SendAsync(context.Request, context.RequestAborted).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            ex.Data.Add("RequestUrlNoQuery", baseUrl ?? "Unknown");

            await TryAttachHutaoGenericApiTraceInfoAsync(context, ex).ConfigureAwait(false);
            await TryAttachNameServerInfoAsync(context, ex).ConfigureAwait(false);

            context.Exception = ExceptionDispatchInfo.Capture(ex);
            context.Logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
    }

    internal static void Send(this HttpRequestMessageBuilder builder, HttpClient httpClient, ILogger logger)
    {
        try
        {
            using (httpClient.Send(builder.HttpRequestMessage))
            {
            }
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (HttpContentSerializationException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
        catch (SocketException ex)
        {
            logger.LogWarning(ex, RequestErrorMessage, builder.RequestUri);
        }
    }

    private static async ValueTask TryAttachHutaoGenericApiTraceInfoAsync(HttpContext context, Exception ex)
    {
        if (context.Response is not { Content: { } content })
        {
            return;
        }

        if (!context.Response.Headers.TryGetValues("x-powered-by", out IEnumerable<string>? values))
        {
            return;
        }

        if (values.SingleOrDefault() is not "Hutao Generic API")
        {
            return;
        }

        if (context.Response.Headers.TryGetValues("x-generic-id", out IEnumerable<string>? ids))
        {
            ex.Data.Add("GenericTraceId", ids.LastOrDefault());
        }

        if (context.Response.Content.Headers?.ContentType?.MediaType is "text/plain")
        {
            string contentString = await content.ReadAsStringAsync(context.RequestAborted).ConfigureAwait(false);
            context.Logger.LogDebug("Response Content: {Content}", contentString);
        }
    }

    private static async ValueTask TryAttachNameServerInfoAsync(HttpContext context, Exception ex)
    {
        if (ex is not HttpRequestException { InnerException: SocketException })
        {
            return;
        }

        string? host = context.Request?.RequestUri?.Host;

        if (host is null)
        {
            return;
        }

        try
        {
            ex.Data.Add("RequestDns", JsonSerializer.Serialize(await Dns.GetHostEntryAsync(host, context.RequestAborted).ConfigureAwait(false)));
        }
        catch
        {
            // Query DNS can fail
        }
    }
}