using JNPF.Common.Const;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Entitys.Entity._3h6h
{
    [SugarTable("ICU_BUNDLE_3H6H_ITEMSGROUP")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ICU_BUNDLE_3H6H_ITEMSGROUPEntity
    {
        [SugarColumn(ColumnName = "UUID")]
        public string UUId { get; set; }
        [SugarColumn(ColumnName = "ITEMNAME")]
        public string ITEMNAME { get; set; }
        [SugarColumn(ColumnName = "GROUPNAME")]
        public string GROUPNAME { get; set; }
    }
}
