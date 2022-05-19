using JNPF.Common.Const;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Entitys.Entity.Permission
{

    /// <summary>
    /// Bundle治疗3小时和6小时治疗项目总体记录数据
    /// </summary>
    [SugarTable("BASE_AUTHORIZE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ICU_BUNDLE_3H6H
    {
        [SugarColumn(ColumnName = "UUID")]
        public string UUID { get; set; }
    }
}
