using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Web.Hoyolab.App.Account;

/// <summary>
/// Ltoken 包装器
/// </summary>
public class LtokenWrapper
{
    /// <summary>
    /// Ltoken
    /// </summary>
    [JsonPropertyName("ltoken")]
    public string Ltoken { get; set; } = default!;
}