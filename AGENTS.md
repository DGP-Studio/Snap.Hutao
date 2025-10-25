# Snap Hutao repository instructions

## 项目定位
- Snap Hutao 是基于 WinUI 3 的现代化 .NET 桌面应用，使用最新稳定或预览的 .NET SDK 与 Windows App SDK。任何改动都必须维持现有的目标框架、语言版本与 MSIX 打包设置。
- 核心目标是在不修改原神客户端的前提下提供工具能力。避免引入破坏性或违反用户隐私的功能。

## 全局开发规范
- **依赖管理**：优先使用内置 BCL、Windows App SDK 与既有依赖。除非经过维护者确认，不要新增第三方包；如必须添加，请解释原因并确认符合自带签名与 MSIX 发布策略。
- **代码风格**：遵循 `.editorconfig`、`stylecop.json`、`BannedSymbols.txt` 与分析器要求。严禁通过禁用规则绕过告警；修复或调整实现以满足规则。
- **语言特性**：保持 `LangVersion=preview`，充分利用现代 C# 特性（例如 required 成员、泛型 math、Span）。禁止退回旧语言模式或关闭 nullable。
- **异步与并发**：UI 线程保持响应；IO、网络、数据库操作应使用异步 API 并正确处理取消与异常。
- **错误处理与日志**：使用既有的 `Core.ExceptionService`、`Validation` 与 `Microsoft.Extensions.Logging` 管线。记录的信息需避免泄露凭据或敏感数据。
- **本地化**：所有用户可见文字必须通过资源实现本地化，且至少提供中文（简体）翻译；可选提供英文。新增字符串时只修改 `Resource/Localization/SH.resx`，不要直接写死文字或修改其他语言资源，这些资源将由 Crowdin 在 PR 合并后同步。
- **测试**：存在非 UI 逻辑变更时需更新或新增 MSTest 单元测试。优先使用已有的测试工具与基类。

## 工作流
1. 恢复依赖：`dotnet restore src/Snap.Hutao/Snap.Hutao.sln`
2. 构建：`dotnet build src/Snap.Hutao/Snap.Hutao.sln -c Debug`
3. 测试（如适用）：`dotnet test src/Snap.Hutao/Snap.Hutao.sln`
4. Visual Studio 发布或 CI 打包时不要调整证书、版本策略或安装通道，除非需求明确。

## PR 与提交
- 所有开发工作必须从 `develop` 分支拉取最新代码并创建功能分支，除非需求明确指定其他基线。
- 功能与修复需以 `develop` 为目标分支，并附带清晰的变更说明与测试步骤。
- 提交消息保持简洁、陈述式；避免无意义的“update”或 “fix”。
- 每次修改仅触及必要文件，保持 PR 易于审查。

本文件适用于整个仓库。若某子目录存在更细的说明，请以就近目录的 `AGENTS.md` 为准。
