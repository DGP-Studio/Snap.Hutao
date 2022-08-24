// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Control;
using Snap.Hutao.Control.Cancellable;
using Snap.Hutao.Control.Extension;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Message;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Achievement;
using Snap.Hutao.Service.Metadata;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.View.Dialog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 成就视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class AchievementViewModel
    : ObservableObject,
    ISupportCancellation,
    INavigationRecipient,
    IRecipient<AchievementArchiveChangedMessage>,
    IRecipient<MainWindowClosedMessage>
{
    private readonly IMetadataService metadataService;
    private readonly IAchievementService achievementService;
    private readonly IInfoBarService infoBarService;
    private readonly JsonSerializerOptions options;
    private readonly IPickerFactory pickerFactory;

    private readonly TaskCompletionSource<bool> openUICompletionSource = new();

    private AdvancedCollectionView? achievements;
    private IList<AchievementGoal>? achievementGoals;
    private AchievementGoal? selectedAchievementGoal;
    private ObservableCollection<Model.Entity.AchievementArchive>? archives;
    private Model.Entity.AchievementArchive? selectedArchive;

    /// <summary>
    /// 构造一个新的成就视图模型
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="achievementService">成就服务</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="pickerFactory">文件选择器工厂</param>
    /// <param name="messenger">消息器</param>
    public AchievementViewModel(
        IMetadataService metadataService,
        IAchievementService achievementService,
        IInfoBarService infoBarService,
        JsonSerializerOptions options,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        IPickerFactory pickerFactory,
        IMessenger messenger)
    {
        this.metadataService = metadataService;
        this.achievementService = achievementService;
        this.infoBarService = infoBarService;
        this.options = options;
        this.pickerFactory = pickerFactory;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        ImportUIAFFromClipboardCommand = asyncRelayCommandFactory.Create(ImportUIAFFromClipboardAsync);
        ImportUIAFFromFileCommand = asyncRelayCommandFactory.Create(ImportUIAFFromFileAsync);
        AddArchiveCommand = asyncRelayCommandFactory.Create(AddArchiveAsync);
        RemoveArchiveCommand = asyncRelayCommandFactory.Create(RemoveArchiveAsync);

        messenger.Register<AchievementArchiveChangedMessage>(this);
        messenger.Register<MainWindowClosedMessage>(this);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 成就存档集合
    /// </summary>
    public ObservableCollection<Model.Entity.AchievementArchive>? Archives
    {
        get => archives;
        set => SetProperty(ref archives, value);
    }

    /// <summary>
    /// 选中的成就存档
    /// </summary>
    public Model.Entity.AchievementArchive? SelectedArchive
    {
        get => selectedArchive;
        set
        {
            if (SetProperty(ref selectedArchive, value))
            {
                achievementService.CurrentArchive = value;
            }
        }
    }

    /// <summary>
    /// 成就视图
    /// </summary>
    public AdvancedCollectionView? Achievements
    {
        get => achievements;
        set => SetProperty(ref achievements, value);
    }

    /// <summary>
    /// 成就分类
    /// </summary>
    public IList<AchievementGoal>? AchievementGoals
    {
        get => achievementGoals;
        set => SetProperty(ref achievementGoals, value);
    }

    /// <summary>
    /// 选中的成就分类
    /// </summary>
    public AchievementGoal? SelectedAchievementGoal
    {
        get => selectedAchievementGoal;
        set
        {
            SetProperty(ref selectedAchievementGoal, value);
            UpdateAchievementFilter(value);
        }
    }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 添加存档命令
    /// </summary>
    public ICommand AddArchiveCommand { get; }

    /// <summary>
    /// 删除存档命令
    /// </summary>
    public ICommand RemoveArchiveCommand { get; }

    /// <summary>
    /// 从剪贴板导入UIAF命令
    /// </summary>
    public ICommand ImportUIAFFromClipboardCommand { get; }

    /// <summary>
    /// 从文件导入UIAF命令
    /// </summary>
    public ICommand ImportUIAFFromFileCommand { get; }

    /// <inheritdoc/>
    public void Receive(MainWindowClosedMessage message)
    {
        SaveAchievements();
    }

    /// <inheritdoc/>
    public void Receive(AchievementArchiveChangedMessage message)
    {
        HandleArchiveChangeAsync(message.OldValue, message.NewValue).SafeForget();
    }

    /// <summary>
    /// 保存当前用户的成就
    /// </summary>
    public void SaveAchievements()
    {
        if (Achievements != null && SelectedArchive != null)
        {
            achievementService.SaveAchievements(SelectedArchive, (Achievements.Source as IList<Model.Binding.Achievement>)!);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ReceiveAsync(INavigationData data)
    {
        if (await openUICompletionSource.Task)
        {
            if (data.Data is "InvokeByUri")
            {
                await ImportUIAFFromClipboardAsync();
                return true;
            }
        }

        return false;
    }

    private static Task<ContentDialogResult> ShowImportFailDialogAsync(string message)
    {
        return new ContentDialog2(Ioc.Default.GetRequiredService<MainWindow>())
        {
            Title = "导入失败",
            Content = message,
            PrimaryButtonText = "确认",
            DefaultButton = ContentDialogButton.Primary,
        }.ShowAsync().AsTask();
    }

    private async Task HandleArchiveChangeAsync(Model.Entity.AchievementArchive? oldArchieve, Model.Entity.AchievementArchive? newArchieve)
    {
        if (oldArchieve != null && Achievements != null)
        {
            achievementService.SaveAchievements(oldArchieve, (Achievements.Source as IList<Model.Binding.Achievement>)!);
        }

        if (newArchieve != null)
        {
            await UpdateAchievementsAsync(newArchieve);
        }
    }

    private async Task OpenUIAsync()
    {
        bool metaInitialized = await metadataService.InitializeAsync(CancellationToken);

        if (metaInitialized)
        {
            List<AchievementGoal> goals = await metadataService.GetAchievementGoalsAsync(CancellationToken);
            AchievementGoals = goals.OrderBy(goal => goal.Order).ToList();

            Archives = achievementService.GetArchiveCollection();

            SelectedArchive = Archives.SingleOrDefault(a => a.IsSelected == true);

            if (SelectedArchive == null)
            {
                infoBarService.Warning("请创建一个成就存档");
            }
        }

        openUICompletionSource.TrySetResult(metaInitialized);
    }

    private async Task UpdateAchievementsAsync(Model.Entity.AchievementArchive archive)
    {
        List<Achievement> rawAchievements = await metadataService.GetAchievementsAsync(CancellationToken);
        List<Model.Binding.Achievement> combined = achievementService.GetAchievements(archive, rawAchievements);
        Achievements = new(combined, true);

        UpdateAchievementFilter(SelectedAchievementGoal);
    }

    private async Task AddArchiveAsync()
    {
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        (bool isOk, string name) = await new AchievementArchiveCreateDialog(mainWindow).GetInputAsync();

        if (isOk)
        {
            ArchiveAddResult result = await achievementService.TryAddArchiveAsync(Model.Entity.AchievementArchive.Create(name));

            switch (result)
            {
                case ArchiveAddResult.Added:
                    infoBarService.Success($"存档 [{name}] 添加成功");
                    break;
                case ArchiveAddResult.InvalidName:
                    infoBarService.Information($"不能添加名称无效的存档");
                    break;
                case ArchiveAddResult.AlreadyExists:
                    infoBarService.Information($"不能添加名称重复的存档 [{name}]");
                    break;
                default:
                    throw Must.NeverHappen();
            }
        }
    }

    private async Task RemoveArchiveAsync()
    {
        if (Archives != null && SelectedArchive != null)
        {
            MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            ContentDialogResult result = await new ContentDialog2(mainWindow)
            {
                Title = $"确定要删除存档 {SelectedArchive.Name} 吗？",
                Content = "该操作是不可逆的，该存档和其内的所有成就状态会丢失。",
                PrimaryButtonText = "确认",
                CloseButtonText = "取消",
                DefaultButton = ContentDialogButton.Close,
            }
            .ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await achievementService.RemoveArchiveAsync(SelectedArchive);

                // reselect first archive
                SelectedArchive = Archives.FirstOrDefault();
            }
        }
    }

    private async Task ImportUIAFFromClipboardAsync()
    {
        if (achievementService.CurrentArchive == null)
        {
            // TODO: automatically create a archive.
            infoBarService.Information("必须选择一个用户才能导入成就");
            return;
        }

        if (await GetUIAFFromClipboardAsync() is UIAF uiaf)
        {
            await TryImportUIAFInternalAsync(achievementService.CurrentArchive, uiaf);
        }
        else
        {
            await ShowImportFailDialogAsync("数据格式不正确");
        }
    }

    private async Task ImportUIAFFromFileAsync()
    {
        if (achievementService.CurrentArchive == null)
        {
            infoBarService.Information("必须选择一个用户才能导入成就");
            return;
        }

        FileOpenPicker picker = pickerFactory.GetFileOpenPicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add(".json");

        if (await picker.PickSingleFileAsync() is StorageFile file)
        {
            if (await GetUIAFFromFileAsync(file) is UIAF uiaf)
            {
                await TryImportUIAFInternalAsync(achievementService.CurrentArchive, uiaf);
            }
            else
            {
                await ShowImportFailDialogAsync("数据格式不正确");
            }
        }
    }

    private async Task<UIAF?> GetUIAFFromClipboardAsync()
    {
        UIAF? uiaf = null;
        string json;
        try
        {
            json = await Clipboard.GetContent().GetTextAsync();
        }
        catch (COMException ex)
        {
            infoBarService?.Error(ex);
            return null;
        }

        try
        {
            uiaf = JsonSerializer.Deserialize<UIAF>(json, options);
        }
        catch (Exception ex)
        {
            infoBarService?.Error(ex);
        }

        return uiaf;
    }

    private async Task<UIAF?> GetUIAFFromFileAsync(StorageFile file)
    {
        UIAF? uiaf = null;
        try
        {
            using (IRandomAccessStreamWithContentType fileSream = await file.OpenReadAsync())
            {
                using (Stream stream = fileSream.AsStream())
                {
                    uiaf = await JsonSerializer.DeserializeAsync<UIAF>(stream, options);
                }
            }
        }
        catch (Exception ex)
        {
            infoBarService?.Error(ex);
        }

        return uiaf;
    }

    private async Task<bool> TryImportUIAFInternalAsync(Model.Entity.AchievementArchive archive, UIAF uiaf)
    {
        if (uiaf.IsCurrentVersionSupported())
        {
            MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
            (bool isOk, ImportOption option) = await new AchievementImportDialog(mainWindow, uiaf).GetImportOptionAsync();

            if (isOk)
            {
                ContentDialog2 importingDialog = new(Ioc.Default.GetRequiredService<MainWindow>())
                {
                    Title = "导入成就中",
                    Content = new ProgressBar() { IsIndeterminate = true },
                    DefaultButton = ContentDialogButton.Primary,
                };

                ImportResult result;
                using (importingDialog.BlockInteraction())
                {
                    result = await achievementService.ImportFromUIAFAsync(archive, uiaf.List, option);
                }

                infoBarService.Success(result.ToString());
                await UpdateAchievementsAsync(archive);
                return true;
            }
        }
        else
        {
            await ShowImportFailDialogAsync("数据的 UIAF 版本过低，无法导入");
        }

        return false;
    }

    private void UpdateAchievementFilter(AchievementGoal? goal)
    {
        if (Achievements != null)
        {
            Achievements.Filter = goal != null
                ? ((object o) => o is Model.Binding.Achievement achi && achi.Inner.Goal == goal.Id)
                : ((object o) => true);
        }
    }
}