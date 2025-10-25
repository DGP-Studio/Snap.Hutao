# Core 层开发规范

适用于 `Snap.Hutao/Core` 目录。

## 角色定位
- 提供跨领域的基础设施（缓存、IO、并发、序列化、依赖注入扩展等）。任何业务逻辑、UI 逻辑或网络协议实现都不应放在此处。
- Core 以及整个项目的类型默认保持 `internal`。仅 `Bootstrap`、`App`、Protobuf 自动生成类型以及 XAML 附加属性辅助类可公开为 `public`。

## 编码约定
- 使用文件作用域命名空间与 `sealed`/`readonly` 修饰符提升性能；对值类型考虑 `readonly struct`。
- 所有 `unsafe` 代码都必须附带明确注释解释其必要性，并确保运行在 x64。
- 资源释放采用 `IDisposable`，优先使用 `using` 声明确保确定性清理。
- 不要在 Core 中直接引用 WinUI 或 ViewModel；若需要与 UI 交互，请通过接口与事件回调在上层实现。

## 性能与诊断
- 性能敏感操作需避免临时分配；使用 `ArrayPool`, `MemoryPool`, `Span<T>` 等优化。提交前在调试日志中留有诊断入口（如 `ILogger` 扩展）。
- 对并发工具类需记录线程安全约束，并通过日志或调试开关提供诊断入口。

## 依赖策略
- 禁止在 Core 中引入新的第三方依赖；若确需使用外部库，请先在更高层封装或使用 BCL 替代方案。
- 当 Core 类型需要访问配置或运行时参数时，优先使用 `Options` 模式或现有的 `Core.Setting` API。
