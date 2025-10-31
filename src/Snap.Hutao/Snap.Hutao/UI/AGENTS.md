# UI 层规范

适用于 `Snap.Hutao/UI` 及其子目录，除非更深层另有文件。

## XAML 与代码隐藏
- 遵循 WinUI 3 和 Fluent Design 规范，尽量使用现有控件与样式。新增控件前先搜索 `UI/Content`、`UI/Shell` 中是否已有相似组件。
- 视图逻辑尽量通过绑定、行为（`CommunityToolkit.WinUI.Behaviors`）或 `DependencyProperty` 实现，避免在 `code-behind` 中写复杂逻辑。
- 在代码隐藏中访问 ViewModel 时使用强类型属性并通过 DI/`Ioc` 注入，禁止直接 `new` ViewModel。

## 性能与可用性
- 避免在 `Loaded` 事件中执行长耗时操作；使用 `async void` 事件处理程序时必须捕获异常并记录。
- 列表、虚拟化控件应启用 `ItemsRepeater`/`ListView` 的虚拟化设置，并考虑 `IncrementalLoading`。
- UI 动画使用 `WinUI`/`Composition` API，保持 60 FPS。若动画依赖资源，确保在低端设备上可降级。

## 资源使用
- 图片、几何等静态资源放入 `Resource` 目录并通过绑定或静态资源引用。不要在 UI 目录重复存放资源文件。
- 所有可本地化文本使用 `x:Uid` 或绑定 `SH` 资源，不得硬编码字符串。

## 验证
- 交互复杂的控件需提供可观察的公共属性或事件，便于行为验证与问题诊断。请在 PR 中说明手动验证路径与结果。
