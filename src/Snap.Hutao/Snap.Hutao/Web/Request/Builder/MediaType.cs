// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Request.Builder;

internal static class MediaType
{
    public const string ApplicationOctetStream = $"{Application}octet-stream";
    public const string ApplicationJson = $"{Application}json";
    public const string ApplicationPdf = $"{Application}pdf";
    public const string ApplicationRtf = $"{Application}rtf";
    public const string ApplicationSoapXml = $"{Application}soap+xml";
    public const string ApplicationJavaScript = $"{Application}javascript";
    public const string ApplicationZip = $"{Application}zip";
    public const string ApplicationWwwFormEncoded = $"{Application}x-www-form-urlencoded";

    public const string TextPlain = $"{Text}plain";
    public const string TextHtml = $"{Text}html";
    public const string TextCss = $"{Text}css";
    public const string TextJavaScript = $"{Text}javascript";
    public const string TextXml = $"{Text}xml";
    public const string TextRichText = $"{Text}richtext";
    public const string TextCsv = $"{Text}csv";

    public const string ImagePng = $"{Image}png";
    public const string ImageJpeg = $"{Image}jpeg";
    public const string ImageTiff = $"{Image}tiff";
    public const string ImageBmp = $"{Image}bmp";
    public const string ImageGif = $"{Image}gif";

    private const string Application = "application/";
    private const string Text = "text/";
    private const string Image = "image/";
}