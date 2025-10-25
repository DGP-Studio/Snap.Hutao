# Core 层开发规范

适用于 `Snap.Hutao/Core` 目录。

## 角色定位
- 提供跨领域的基础设施（缓存、IO、并发、序列化、依赖注入扩展等）。任何业务逻辑、UI 逻辑或网络协议实现都不应放在此处。
- Core 中的类型默认是 `internal`。只有在需要被多个项目复用并确认 API 稳定时才公开为 `public`。

## 编码约定
- 使用文件作用域命名空间与 `sealed`/`readonly` 修饰符提升性能；对值类型考虑 `readonly struct`。
- 所有 `unsafe` 代码都必须附带明确注释解释其必要性，并确保运行在 x64。
- 资源释放采用 `IDisposable`/`IAsyncDisposable`，优先使用 `using` 声明或 `DisposeAsync`。
- 不要在 Core 中直接引用 WinUI 或 ViewModel；若需要与 UI 交互，请通过接口与事件回调在上层实现。

## 性能与诊断
- 性能敏感操作需避免临时分配；使用 `ArrayPool`, `MemoryPool`, `Span<T>` 等优化。提交前在调试日志中留有诊断入口（如 `ILogger` 扩展）。
- 对并发工具类需使用单元测试覆盖多线程路径，确保线程安全并记录使用限制。

## 依赖策略
- 禁止在 Core 中引入新的第三方依赖；若确需使用外部库，请先在更高层封装或使用 BCL 替代方案。
- 当 Core 类型需要访问配置或运行时参数时，优先使用 `Options` 模式或现有的 `Core.Setting` API。
