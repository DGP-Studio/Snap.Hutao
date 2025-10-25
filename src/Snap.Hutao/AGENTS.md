# Snap Hutao solution guidelines

适用于 `src/Snap.Hutao` 目录及其子目录，除非更深层的 `AGENTS.md` 另有说明。

## 解决方案结构
- `Snap.Hutao.sln` 是唯一需要维护的顶层解决方案；不要新增额外的解决方案文件，除非经架构评审同意。
- WinUI 3 主项目位于 `Snap.Hutao/`，测试项目位于 `Snap.Hutao.Test/`。保持项目文件中的 `TargetFramework`、`LangVersion` 与 `Nullable` 设置不变。
- 更新 NuGet 引用时仅提升版本或移除不用的包，禁止降低版本，也不要引入与现有功能重叠的库。

## 构建与配置
- 所有项目必须能通过 `dotnet build Snap.Hutao.sln -c Debug` 编译。提交前确保解决方案在 Release 模式同样无警告。
- 需要添加新项目或更改现有项目属性时，确保 CI 可在无 GUI 环境下运行（不要依赖手工步骤）。
- `.slnx` 与 `ResXManager.config.xml` 由工具生成，除非同步更新配置，否则不要手动编辑。

## 文件组织
- 代码文件应放入与功能相匹配的项目与命名空间，命名空间必须与文件夹结构保持一致。
- 所有新增的公共 API 必须在 XML 文档注释中说明用途，并在需要时补充单元测试或示例。

## Git 约定
- 不要提交临时生成文件（例如 `bin/`、`obj/`、`*.user`）；如遇生成工具创建的新配置文件，请在 `.gitignore` 之前确认是否需要版本控制。
