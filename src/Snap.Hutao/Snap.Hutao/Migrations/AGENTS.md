# EF Core Migrations 指南

适用于 `Snap.Hutao/Migrations`。

- 迁移文件由 `dotnet ef migrations add` 生成。不要手写 Designer 文件，仅在生成后做最小化调整（如注释）。
- 新增迁移需同步更新数据库上下文（`Service/Database` 相关）并验证 `dotnet ef database update` 可成功执行。
- 保持迁移命名含义明确（`YYYYMMDDHHMMSS_Description`）。删除或重排历史迁移需经维护者确认。
- 在迁移中避免执行不可逆或长耗时的原始 SQL；必要时提供 `Down` 回滚逻辑或附带数据备份说明。
