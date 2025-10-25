# Web 层规范

适用于 `Snap.Hutao/Web`。

## 作用
- 处理所有 HTTP/IPC/WebView2 交互，包括请求 DTO、响应 DTO、客户端封装与桥接逻辑。业务流程仍由 `Service` 层协调。

## 请求与响应
- DTO 使用 `record` 或 `class`，保持属性命名与外部 API 一致。需要转换的字段在 Service 层处理。
- JSON 序列化使用 `System.Text.Json`，必要时自定义 `JsonSerializerContext` 或 `JsonConverter`，不要引入第三方 JSON 库。
- 在响应类型上明确 `required` 属性，避免后续空引用问题。对于可选字段使用 `JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)`。

## HTTP 客户端
- 统一通过 `IHttpClientFactory` 或 `HttpClientBuilder` 获取客户端，集中配置默认请求头、超时与重试策略。
- 网络异常需要包装为项目自定义异常或返回明确的错误结构，并在调用方进行日志记录。

## WebView2 与脚本
- WebView2 相关代码放在 `WebView2/`，脚本注入需最小化并记录来源。禁止执行未经审查的远程脚本。
- 与前端交互的桥接对象需实现线程安全，并验证输入以防注入。
