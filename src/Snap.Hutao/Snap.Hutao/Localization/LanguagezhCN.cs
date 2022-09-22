// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Localization;

/// <summary>
/// 中文翻译 zh-CN
/// </summary>
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
internal class LanguagezhCN : ITranslation
{
    private readonly Dictionary<string, string> translations = new()
    {
        ["AppName"] = "胡桃",

        ["NavigationViewItem_Activity"] = "活动",
        ["NavigationViewItem_Achievement"] = "成就",
        ["NavigationViewItem_Wiki_Avatar"] = "角色",
        ["NavigationViewItem_GachaLog"] = "祈愿记录",

        ["UserPanel_Account"] = "账号",
        ["UserPanel_Add_Account"] = "添加新账号",
        ["UserPanel_GameRole"] = "角色",

        ["Achievement_Search_PlaceHolder"] = "搜索成就名称，描述或编号",
        ["Achievement_Create_Archive"] = "创建新存档",
        ["Achievement_Delete_Archive"] = "删除当前存档",
        ["Achievement_Import"] = "导入",
        ["Achievement_Import_From_Clipboard"] = "从剪贴板导入",
        ["Achievement_Import_From_File"] = "从 UIAF 文件导入",
        ["Achievement_IncompleteItemFirst"] = "优先未完成",

        ["Wiki_Avatar_Filter"] = "筛选",
        ["Wiki_Avatar_Filter_Element"] = "元素",
        ["Wiki_Avatar_Filter_Association"] = "所属",
        ["Wiki_Avatar_Filter_Weapon"] = "武器",
        ["Wiki_Avatar_Filter_Quality"] = "星级",
        ["Wiki_Avatar_Filter_Body"] = "体型",
        ["Wiki_Avatar_Fetter_Native"] = "所属",
        ["Wiki_Avatar_Fetter_Constellation"] = "命之座",
        ["Wiki_Avatar_Fetter_Birth"] = "生日",
        ["Wiki_Avatar_Fetter_CvChinese"] = "汉语 CV",
        ["Wiki_Avatar_Fetter_CvJapanese"] = "日语 CV",
        ["Wiki_Avatar_Fetter_CvEnglish"] = "英语 CV",
        ["Wiki_Avatar_Fetter_CvKorean"] = "韩语 CV",
        ["Wiki_Avatar_Subtitle_Skill"] = "天赋",
        ["Wiki_Avatar_Subtitle_Talent"] = "命之座",
        ["Wiki_Avatar_Subtitle_Other"] = "其他",
        ["Wiki_Avatar_Expander_Costumes"] = "衣装",
        ["Wiki_Avatar_Expander_Fetters"] = "资料",
        ["Wiki_Avatar_Expander_FetterStories"] = "故事",

        ["DescParamComboBox_Level"] = "等级",

        ["GachaLog_Refresh"] = "刷新",
        ["GachaLog_Refresh_WebCache"] = "从缓存刷新",
        ["GachaLog_Refresh_ManualInput"] = "手动输入Url",
        ["GachaLog_Refresh_Aggressive"] = "全量刷新",
        ["GachaLog_Import"] = "导入",
        ["GachaLog_Import_UIGFJ"] = "从 UIGF Json 文件导入",
        ["GachaLog_Import_UIGFW"] = "从 UIGF Excel 文件导入",
        ["GachaLog_Export"] = "导出",
        ["GachaLog_Export_UIGFJ"] = "导出到 UIGF Json 文件",
        ["GachaLog_Export_UIGFW"] = "导出到 UIGF Excel 文件",
        ["GachaLog_PivotItem_Summary"] = "总览",
        ["GachaLog_PivotItem_History"] = "历史",
        ["GachaLog_PivotItem_Avatar"] = "角色",
        ["GachaLog_PivotItem_Weapon"] = "武器",

        ["StatisticsCard_Guarantee"] = "保底",
        ["StatisticsCard_Up"] = "保底",
        ["StatisticsCard_Pull"] = "抽",
        ["StatisticsCard_Orange"] = "五星",
        ["StatisticsCard_Purple"] = "四星",
        ["StatisticsCard_Blue"] = "三星",
        ["StatisticsCard_OrangeAverage"] = "五星平均抽数",
        ["StatisticsCard_UpOrangeAverage"] = "UP 平均抽数",

        ["Setting_Group_AboutHutao"] = "关于 胡桃",
        ["Setting_HutaoIcon_Description_Part1"] = "胡桃 图标由 ",
        ["Setting_HutaoIcon_Description_Part2"] = "纸绘，并由 ",
        ["Setting_HutaoIcon_Description_Part3"] = " 后期处理后，授权使用。",
        ["Setting_Feedback_Header"] = "反馈",
        ["Setting_Feedback_Description"] = "只处理在 Github 上反馈的问题",
        ["Setting_Feedback_Hyperlink"] = "只处理在 Github 上反馈的问题",
        ["Setting_UpdateCheck_Header"] = "检查更新",
        ["Setting_UpdateCheck_Description"] = "根本没有检查更新选项",
        ["Setting_UpdateCheck_Info"] = "都说了没有了",
        ["Setting_Group_Experimental"] = "测试功能",
        ["Setting_DataFolder_Header"] = "打开 数据 文件夹",
        ["Setting_DataFolder_Description"] = "用户数据/日志/元数据在此处存放",
        ["Setting_DataFolder_Action"] = "打开",
        ["Setting_CacheFolder_Header"] = "打开 缓存 文件夹",
        ["Setting_CacheFolder_Description"] = "图片缓存在此处存放",
        ["Setting_CacheFolder_Action"] = "打开",
    };

    /// <inheritdoc/>
    public string this[string key]
    {
        get
        {
            if (translations.TryGetValue(key, out string? result))
            {
                return result;
            }

            return string.Empty;
        }
    }
}