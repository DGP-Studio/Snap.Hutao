# Win32 互操作规范

适用于 `Snap.Hutao/Win32`。

## 目标
- 封装 Win32/COM/PInvoke 调用，为上层提供安全、可测试的接口。尽量复用现有封装，避免重复声明。

## 编码注意事项
- 所有 P/Invoke 声明必须标记正确的 `DllImport` 属性（`ExactSpelling`, `SetLastError`, `CharSet` 等）并确保与目标平台匹配。
- 结构体与枚举应使用 `StructLayout` 精确对齐，并提供注释说明来源（Win32 API 文档链接或 SDK 头文件）。
- 使用 `SafeHandle` 或 `using` 模式管理原生资源，禁止泄漏句柄。需要 `unsafe` 指针操作时注明生存期与所有权。

## 错误处理
- 失败时返回的 Win32 错误码需转换为可读异常或结果对象。不要忽略 `Marshal.GetLastWin32Error()`。
- 互操作层不得直接依赖 UI 逻辑；如需回调，使用委托并在托管端做好同步。

## 审核
- 添加新 API 前确认 Windows SDK 中已有声明；若需手动抄写，请附带注释说明版本与出处。
