# Snap.Hutao.Test 项目规范

适用于 `Snap.Hutao/Snap.Hutao.Test`。

## 测试框架
- 使用 MSTest (`[TestClass]`, `[TestMethod]`, `[DataTestMethod]`)。不要混用 xUnit 或 NUnit。
- 保持 `LangVersion=preview`，可以使用最新 C# 语法，但需确保测试对旧环境无副作用。

## 结构
- 依照命名空间组织文件，例如 `RuntimeBehavior`, `PlatformExtensions`。新增测试时放入相应子目录或创建新目录，并保持命名清晰。
- 共用的测试工具、假实现放在 `BaseClassLibrary` 或新的 `Helpers` 文件夹，并避免依赖生产代码中的 UI 组件。

## 编写测试
- 避免测试之间的状态耦合。使用 `TestInitialize`/`TestCleanup` 或 `ClassInitialize` 进行资源准备与释放。
- 涉及异步调用的测试使用 `async Task` 并在断言前等待操作完成。对外部依赖要使用 stub/mocks，禁止真实 HTTP 调用。

## 覆盖率
- 关键业务逻辑的变更须增加测试覆盖，并在 PR 描述中记录测试范围。必要时通过 `Assert.ThrowsException` 验证异常路径。
