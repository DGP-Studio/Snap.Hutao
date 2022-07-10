// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Metadata.Avatar;
using Snap.Hutao.Service.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 角色资料视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class WikiAvatarViewModel : ObservableObject
{
    private readonly IMetadataService metadataService;

    private List<Avatar>? avatars;
    private Avatar? selected;

    /// <summary>
    /// 构造一个新的角色资料视图模型
    /// </summary>
    /// <param name="metadataService">元数据服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public WikiAvatarViewModel(IMetadataService metadataService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.metadataService = metadataService;
        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
    }

    /// <summary>
    /// 角色列表
    /// </summary>
    public List<Avatar>? Avatars { get => avatars; set => SetProperty(ref avatars, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Avatar? Selected { get => selected; set => SetProperty(ref selected, value); }

    /// <summary>
    /// 打开页面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync()
    {
        if (await metadataService.InitializeAsync())
        {
            IEnumerable<Avatar>? avatars = await metadataService.GetAvatarsAsync();
            IOrderedEnumerable<Avatar> sorted = avatars
                .OrderBy(avatar => avatar.BeginTime)
                .ThenBy(avatar => avatar.Sort);

            Avatars = new List<Avatar>(sorted);
            Selected = Avatars[0];
        }
    }
}
