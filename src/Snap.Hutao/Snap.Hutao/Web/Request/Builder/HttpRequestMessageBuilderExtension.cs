// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Notification;
using Snap.Hutao.Web.Response;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Snap.Hutao.Web.Request.Builder;

internal static class HttpRequestMessageBuilderExtension
{
    internal static HttpRequestMessageBuilder Resurrect(this HttpRequestMessageBuilder builder)
    {
        builder.HttpRequestMessage.Resurrect();
        return builder;
    }

    internal static ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, CancellationToken token)
        where TResult : class
    {
        return SendAsync<TResult>(builder, httpClient, HttpCompletionOption.ResponseContentRead, true, token);
    }

    internal static ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, bool logException, CancellationToken token)
        where TResult : class
    {
        return SendAsync<TResult>(builder, httpClient, HttpCompletionOption.ResponseContentRead, logException, token);
    }

    internal static ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, HttpCompletionOption completionOption, CancellationToken token)
        where TResult : class
    {
        return SendAsync<TResult>(builder, httpClient, completionOption, true, token);
    }

    internal static async ValueTask<TypedHttpResponse<TResult>> SendAsync<TResult>(this HttpRequestMessageBuilder builder, HttpClient httpClient, HttpCompletionOption completionOption, bool logException, CancellationToken token)
        where TResult : class
    {
        HttpContext context = new()
        {
            HttpClient = httpClient,
            CompletionOption = completionOption,
            RequestAborted = token,
            LogException = logException,
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

                showInfo = false;
                TResult? body = await builder.HttpContentSerializer.DeserializeAsync<TResult>(context.Response.Content, token).ConfigureAwait(false);
                return new(context.Response.Headers, body);
            }
            catch (OperationCanceledException)
            {
                showInfo = false;

                // Populate to caller
                throw;
            }
            catch (Exception ex)
            {
                if (logException)
                {
                    HttpRequestExceptionHandling.TryHandle(messageBuilder, builder, ex);
                }

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
        try
        {
            context.Request = builder.HttpRequestMessage;
            context.Response = await context.HttpClient.SendAsync(context.Request, context.CompletionOption, context.RequestAborted).ConfigureAwait(false);
            context.Response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            context.Exception = ExceptionDispatchInfo.Capture(ex);
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
        catch (HttpRequestException)
        {
        }
        catch (IOException)
        {
        }
        catch (JsonException)
        {
        }
        catch (HttpContentSerializationException)
        {
        }
        catch (SocketException)
        {
        }
    }
}