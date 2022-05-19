using System.Collections.Generic;

namespace JNPF.System.Entitys.Model.Permission.Authorize
{
    /// <summary>
    /// 数据权限条件
    /// </summary>
    public class AuthorizeModuleResourceConditionModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Logic { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AuthorizeModuleResourceConditionItemModel> Groups { get; set; }
    }
}
