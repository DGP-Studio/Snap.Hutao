# ViewModel 层规范

适用于 `Snap.Hutao/ViewModel` 目录及其子目录。

## MVVM 要点
- ViewModel 必须可被 DI 容器解析，构造函数依赖通过接口注入。禁止在 ViewModel 中直接实例化 Service。
- 使用 `CommunityToolkit.Mvvm` 的 `ObservableObject`, `ObservableProperty`, `RelayCommand` 等特性。避免手写冗长的通知逻辑。
- ViewModel 不应持有 WinUI 控件引用。需要交互时通过接口、事件或 `INavigationService` 等抽象沟通。

## 异步与状态管理
- 暴露给 UI 的异步命令使用 `AsyncRelayCommand` 或自定义命令封装，确保异常被捕获并记录。
- 所有可绑定集合使用 `ObservableCollection<T>` 或项目已有的可观察集合类型。长时间运行任务需提供取消机制。
- 对于可序列化状态（如用户设置）在 `Service` 层处理持久化，ViewModel 只维护内存态。

## 测试
- 为关键 ViewModel 编写单元测试验证命令与属性变更逻辑。通过依赖注入的接口可在测试中替换为 mocks/stubs。

## 文档
- 在类或重要属性上添加 XML 注释，说明绑定目的和依赖的服务。跨模块的 ViewModel 需记录导航或生命周期约束。
