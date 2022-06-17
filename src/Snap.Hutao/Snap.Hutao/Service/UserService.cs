using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Service;

/// <summary>
/// 用户服务
/// </summary>
[Injection(InjectAs.Transient)]
internal class UserService : IUserService
{


    public User Current { get => throw new NotImplementedException(); }
}
