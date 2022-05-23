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
    /// 8888
    /// </summary>
    [SugarTable("ICU_BUNDLE_3H6H_ITEMS")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class ICU_BUNDLE_3H6H_ITEMSEntity
    {


        [SugarColumn(ColumnName = "UUID")]
        public string UUId { get; set; }

        /// <summary>
        /// BUNDLE_3H6H主键
        /// </summary>
        [SugarColumn(ColumnName = "BUNDLE_3H6H_UUID")]
        public string Bundle3h6huuId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        [SugarColumn(ColumnName = "ITEMNAME")]
        public string ItemName { get; set; }

        /// <summary>
        /// 医嘱号,根据医嘱号得到起始时间，终止时间，如果是点类型的数据只有起始时间有值
        /// </summary>
        [SugarColumn(ColumnName = "ORDER_NO")]
        public string OrderNo { get; set; }


        /// <summary>
        /// 数据起始时间
        /// </summary>
        [SugarColumn(ColumnName = "DATA_START_TIME")]
        public DateTime DataStartTime { get; set; }


        /// <summary>
        /// 数据终止时间
        /// </summary>
        [SugarColumn(ColumnName = "DATA_END_TIME")]
        public DateTime DataEndTime { get; set; }


        /// <summary>
        /// 绘制起始时间
        /// </summary>
        [SugarColumn(ColumnName = "DRAW_START_DATETIME")]
        public DateTime DrawStartDateTime { get; set; }


        /// <summary>
        /// 绘制终止时间
        /// </summary>
        [SugarColumn(ColumnName = "DRAW_END_DATETIME")]
        public DateTime DrawEndDateTime { get; set; }


        /// <summary>
        /// 测量的值(治疗效果)
        /// </summary>
        [SugarColumn(ColumnName = "ITEMVALUE")]
        public decimal ItemValue { get; set; }


        /// <summary>
        /// 操作人员ID
        /// </summary>
        [SugarColumn(ColumnName = "OPERATION_USERID")]
        public string OperationUserId { get; set; }


        /// <summary>
        /// 操作日期
        /// </summary>
        [SugarColumn(ColumnName = "OPERATION_TIME")]
        public DateTime OperationTime { get; set; }
    }
}
