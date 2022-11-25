namespace Snap.Hutao.Web.Bridge.Model;

/// <summary>
/// Js 参数
/// </summary>
public class JsParam
{
    /// <summary>
    /// 方法名称
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 数据
    /// </summary>
    [JsonPropertyName("payload")]
    public JsonElement Data { get; set; }

    /// <summary>
    /// 回调名称
    /// </summary>
    [JsonPropertyName("callback")]
    public string CallbackName { get; set; } = string.Empty;

    /// <summary>
    /// 对应的调用桥
    /// </summary>
    internal MiHoYoJsBridge Bridge { get; set; } = null!;

    /// <summary>
    /// 执行回调
    /// </summary>
    /// <param name="resultFactory">结果工厂</param>
    public void Callback(Func<JsResult>? resultFactory = null)
    {
        JsResult? result = resultFactory?.Invoke() ?? new();
        Callback(result?.ToString());
    }

    /// <summary>
    /// 执行回调
    /// </summary>
    /// <param name="resultModifier">结果工厂</param>
    public void Callback(Action<JsResult> resultModifier)
    {
        JsResult result = new();
        resultModifier(result);
        Callback(result?.ToString());
    }

    /// <summary>
    /// 执行回调
    /// </summary>
    /// <param name="result">结果</param>
    public void Callback(string? result = null)
    {
        Bridge.InvokeJsCallbackAsync(CallbackName, result).GetAwaiter().GetResult();
    }
}