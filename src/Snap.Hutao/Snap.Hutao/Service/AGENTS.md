# Service 层规范

适用于 `Snap.Hutao/Service` 目录及子目录，除非更深层另有文件。

## 职责
- Service 负责封装业务流程、网络访问、数据存取与后台任务。保持方法可组合、易替换，并对外暴露接口或抽象类以利依赖注入。
- 任何需要与 UI 交互的逻辑通过事件、回调或 `ViewModel` 调用完成，Service 本身不直接引用 XAML 控件。

## 设计要求
- 所有 Service 需定义接口（`I*Service`）并注册到 DI 容器。实现类命名遵循 `*Service` 或 `*Client`，确保生命周期明确（`Singleton`/`Scoped`/`Transient`）。
- 网络请求使用 `HttpClient` 工厂或 `Web` 层封装；不要在 Service 内手动 new `HttpClient`。
- IO 与数据库操作使用异步 API，并支持 `CancellationToken`。若操作可能失败，返回 `Result`/`Try` 模式或抛出已记录的自定义异常。

## 审计与日志
- 记录关键操作日志，使用 `ILogger<T>` 注入。日志内容需遵循隐私策略，避免写入用户凭证。
- 与用户数据相关的 Service 需考虑缓存一致性与清理流程，并在注释中记录副作用。

## 验证
- 添加新 Service 时需提供可替换的模拟实现或记录验证步骤，确保关键路径在无 UI 环境下可复现。对于依赖外部 API 的逻辑使用接口和可模拟抽象。
