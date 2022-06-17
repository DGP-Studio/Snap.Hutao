using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 用户视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class UserViewModel : ObservableObject
{
    private readonly IUserService userService;
    private readonly UserGameRoleClient userGameRoleClient;
    private readonly ILogger<UserViewModel> logger;

    private User? selectedUserInfo;
    private ObservableCollection<User>? userInfos;
    private UserGameRole? selectedUserGameRole;
    private ObservableCollection<UserGameRole>? userGameRoles;

    /// <summary>
    /// 构造一个新的用户视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="userGameRoleClient">用户角色信息客户端</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public UserViewModel(IUserService userService, UserGameRoleClient userGameRoleClient, IAsyncRelayCommandFactory asyncRelayCommandFactory, ILogger<UserViewModel> logger)
    {
        this.userService = userService;
        this.userGameRoleClient = userGameRoleClient;
        this.logger = logger;
        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
    }

    /// <summary>
    /// 当前选择的用户信息
    /// </summary>
    public User? SelectedUser
    {
        get => selectedUserInfo;
        set
        {
            if (SetProperty(ref selectedUserInfo, value) && value != null)
            {
                UpdateUserGameRolesAsync().SafeForget(logger);
            }
        }
    }

    /// <summary>
    /// 用户信息集合
    /// </summary>
    public ObservableCollection<User>? Users { get => userInfos; set => SetProperty(ref userInfos, value); }

    /// <summary>
    /// 选择的角色信息
    /// </summary>
    public UserGameRole? SelectedUserGameRole { get => selectedUserGameRole; set => SetProperty(ref selectedUserGameRole, value); }

    /// <summary>
    /// 角色信息集合
    /// </summary>
    public ObservableCollection<UserGameRole>? UserGameRoles
    {
        get => userGameRoles;
        set
        {
            if (SetProperty(ref userGameRoles, value))
            {
                if (value != null)
                {
                    SelectedUserGameRole = value.FirstOrDefault(role => role.IsChosen) ?? value.FirstOrDefault();
                }
                else
                {
                    SelectedUserGameRole = null;
                }
            }
        }
    }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    private async Task OpenUIAsync()
    {
        Users = new(await userService.GetInitializedUsersAsync());
        SelectedUser = Users.FirstOrDefault();
    }

    private async Task UpdateUserGameRolesAsync()
    {
        UserGameRoles = new(await userGameRoleClient.GetUserGameRolesAsync());
    }
}
