# Extension 目录规范

适用于 `Snap.Hutao/Extension`。

## 目的
- 提供对 .NET BCL、WinUI 控件或项目内部类型的扩展方法。扩展方法必须具备通用性与高可读性，不得包含业务专用逻辑。

## 编码要求
- 每个文件聚焦在一个扩展目标（例如 `ListExtension`、`StringExtension`），避免杂糅。新增扩展时优先查找是否已有同类实现。
- 扩展方法应标记为 `static` 且归于 `internal static partial class`，必要时使用 `AggressiveInlining` 提升性能。
- 对可能引起分配的操作（如 `ToList()`）需在注释中标明成本，并在调用站点考虑替代方案。

## 文档
- 为每个公开的扩展方法编写 XML 注释，说明行为、输入限制与异常。涉及复杂算法的扩展需记录性能与边界情况，必要时提供使用示例。

## 依赖限制
- 不得在扩展中引用业务层命名空间（`Service`, `ViewModel`, `Web` 等）。若需要共享逻辑，请将实现拆分到 `Core` 后在此转调。
