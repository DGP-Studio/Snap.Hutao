# Resource 目录规范

适用于 `Snap.Hutao/Resource`。

## 分类
- 将资源按用途分类存放（`Localization`, `Navigation`, `ItemIcon`, `TeachingTip` 等）。新增资源时遵循现有结构，不要创建含糊的目录名称。

## 本地化
- `Localization/SH.resx` 是新增字符串的唯一入口。保持键名 PascalCase，并附带中文描述。其他语言资源由 Crowdin 同步，不要直接修改。
- `Localization/SHRegex.*.resx` 仅用于正则表达式配置，新增项需同步更新所有语种或解释为何仅限部分语种。

## 图像与媒体
- 图片需使用 PNG 或 SVG（如有）并压缩到合理体积。为多分辨率资源提供 scale-100/200/400 版本。
- 添加新资源后更新 `Snap.Hutao.csproj` 的 `Content Include` 条目。若资源仅在测试或临时场景使用，请放入 `res/` 顶层，而不是应用包。

## 版本控制
- 不要提交源文件的 PSD/AI 等设计稿。若需要保留，请在 `res/` 目录维护并在 README 中记录。

## 校验
- 在 PR 中说明资源来源与授权情况。对于第三方图标需确认拥有使用权。
