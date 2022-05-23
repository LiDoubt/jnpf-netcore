using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    /// 业务实现：角色信息
    /// </summary>
    [ApiDescriptionSettings(Tag = "Permission", Name = "Role", Order = 167)]
    [Route("api/permission/[controller]")]
    public class ICUuBundleService :IUsersService, IDynamicApiController, ITransient
    {
    }
}
