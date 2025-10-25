# Snap Hutao app project instructions

本文件覆盖 WinUI 主项目 `Snap.Hutao` 目录下的所有文件，除非子目录另有 `AGENTS.md`。

## 架构原则
- 采用 MVVM：视图 (`UI`) 仅负责布局与绑定，`ViewModel` 处理呈现逻辑，`Service` 封装业务与外部依赖，`Model` 描述领域对象，`Core` 与 `Extension` 提供基础设施。
- 通过 `Microsoft.Extensions.DependencyInjection` 进行依赖注入。新增服务时在 `Core.DependencyInjection` 的配置扩展中注册，保持生命周期与线程安全。
- 不要引入新的全局单例或静态状态，除非已有模式（例如 `HutaoRuntime`）明确需要。
- 优先复用现有帮助类、缓存策略与 telemetry。必要的新工具请放入最贴近的命名空间，并提供单元测试。

## WinUI / XAML
- 所有 `XAML` 文件需配套 `partial` 代码隐藏或 ViewModel，避免在 `x:Code` 中编写逻辑。
- 使用现有资源字典与样式；新增资源前确认没有重复。对于主题色、图标、动画等公共元素，放入 `Resource` 对应文件夹并在资源字典中引用。
- 避免阻塞 UI 线程；长耗时操作使用 `async` 与 `Task`，必要时借助 `DispatcherQueue` 切回 UI 线程。

## 可维护性
- 新增公共或内部 API 时补充 XML 注释，并确保名称含义清晰。避免过度缩写。
- 使用 `partial` 或 `file-scoped namespace` 维持一致的代码风格。保持 `#nullable enable`，不要使用 `#pragma warning disable` 覆盖全局规则。
- 对于需要非托管互操作的代码，请优先查看 `Win32` 目录的现有封装，并确保 `AllowUnsafeBlocks` 下的代码经过审慎审查。

## 性能与内存
- 对性能敏感路径使用 `Span<T>`、`Memory<T>` 等现代 API；当需要池化时复用项目中现有的 `ObjectPool` 或 `RecyclableMemoryStream`。
- 使用 `ValueTask` 与结构体枚举器时注意可重入与重用；避免产生额外装箱或 `LINQ` 分配。

## 资源与本地化
- 新增图片或二进制资源时应在 `Snap.Hutao.csproj` 中声明，并确保尺寸、缩放级别符合 Fluent 设计指南。
- 字符串使用 `Resource.Localization` 的 `SH` 资源键，并通过 `XAML` 绑定或 `SH.GetString` 获取。

## 测试与验证
- 涉及 Core/Service 逻辑的改动应考虑在 `Snap.Hutao.Test` 中增加 MSTest 覆盖，或更新现有测试。
- 手动验证 UI 变更时需在 PR 描述中说明操作路径与结果。
