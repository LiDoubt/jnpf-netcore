using JNPF.Common.Const;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Entitys.Entity._3h6h
{
    /// <summary>
    /// Bundle治疗3小时和6小时治疗项目总体记录数据
    /// </summary>
    [SugarTable("ICU_BUNDLE_3H6H")]
    [Tenant(ClaimConst.TENANT_ID)]

    public class ICU_BUNDLE_3H6HEntity
    {
        [SugarColumn(ColumnName = "UUID")]
        public string UUId { get; set; }
        /// <summary>
        /// 患者id
        /// </summary>
        [SugarColumn(ColumnName = "PATIENT_UUID")]
        public string PatientId { get; set; }
        /// <summary>
        /// 起始治疗时间
        /// </summary>
        [SugarColumn(ColumnName = "START_CURE_TIME")]
        public DateTime StartCurdTime { get; set; }

        /// <summary>
        /// 开始治疗人员ID
        /// </summary>
        [SugarColumn(ColumnName = "START_USERID")]
        public string StartUserId { get; set; }

        /// <summary>
        /// 终止治疗时间
        /// </summary>
        [SugarColumn(ColumnName = "END_CURE_TIME")]
        public DateTime EndCureTime { get; set; }

        /// <summary>
        /// 结束治疗人员ID
        /// </summary>
        [SugarColumn(ColumnName = "END_USERID")]
        public string EndUserId { get; set; }
    }
}
