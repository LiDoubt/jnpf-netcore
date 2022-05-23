using System;

namespace JNPF.System.Entitys.Model._3h6h
{
    public class ICU_BUNDLE_3H6H_ITEMAndGropuRes
    {

        public string ICU_BUNDLE_3H6H_UUId { get; set; }
        /// <summary>
        /// 患者id
        /// </summary>
        public string PatientId { get; set; }
        /// <summary>
        /// 起始治疗时间
        /// </summary>
        public DateTime StartCurdTime { get; set; }

        /// <summary>
        /// 开始治疗人员ID
        /// </summary>
        public string StartUserId { get; set; }

        /// <summary>
        /// 终止治疗时间
        /// </summary>
        public DateTime EndCureTime { get; set; }

        /// <summary>
        /// 结束治疗人员ID
        /// </summary>
        public string EndUserId { get; set; }



        public string ICU_BUNDLE_3H6H_ITEMUUId { get; set; }

        /// <summary>
        /// BUNDLE_3H6H主键
        /// </summary>
        public string Bundle3h6huuId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 医嘱号,根据医嘱号得到起始时间，终止时间，如果是点类型的数据只有起始时间有值
        /// </summary>
        public string OrderNo { get; set; }


        /// <summary>
        /// 数据起始时间
        /// </summary>
        public DateTime DataStartTime { get; set; }


        /// <summary>
        /// 数据终止时间
        /// </summary>
        public DateTime DataEndTime { get; set; }





        /// <summary>
        /// 测量的值(治疗效果)
        /// </summary>
        public decimal ItemValue { get; set; }


        /// <summary>
        /// 操作人员ID
        /// </summary>
        public string OperationUserId { get; set; }


        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperationTime { get; set; }


        public string GROUPNAME { get; set; }
    }
}