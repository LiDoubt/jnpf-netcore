using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;
using System;

namespace JNPF.Extend.Entitys
{
    /// <summary>
    /// 职员信息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01  
    /// </summary>
    [SugarTable("EXT_EMPLOYEE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class EmployeeEntity : CLDEntityBase
    {
        /// <summary>
        /// 工号
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [SugarColumn(ColumnName = "F_GENDER")]
        public string Gender { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [SugarColumn(ColumnName = "F_DEPARTMENTNAME")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [SugarColumn(ColumnName = "F_POSITIONNAME")]
        public string PositionName { get; set; }
        /// <summary>
        /// 用工性质
        /// </summary>
        [SugarColumn(ColumnName = "F_WORKINGNATURE")]
        public string WorkingNature { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [SugarColumn(ColumnName = "F_IDNUMBER")]
        public string IdNumber { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        [SugarColumn(ColumnName = "F_TELEPHONE")]
        public string Telephone { get; set; }
        /// <summary>
        /// 参加工作
        /// </summary>
        [SugarColumn(ColumnName = "F_ATTENDWORKTIME")]
        public DateTime? AttendWorkTime { get; set; }
        /// <summary>
        /// 出生年月
        /// </summary>
        [SugarColumn(ColumnName = "F_BIRTHDAY")]
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 最高学历
        /// </summary>
        [SugarColumn(ColumnName = "F_EDUCATION")]
        public string Education { get; set; }
        /// <summary>
        /// 所学专业
        /// </summary>
        [SugarColumn(ColumnName = "F_MAJOR")]
        public string Major { get; set; }
        /// <summary>
        /// 毕业院校
        /// </summary>
        [SugarColumn(ColumnName = "F_GRADUATIONACADEMY")]
        public string GraduationAcademy { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        [SugarColumn(ColumnName = "F_GRADUATIONTIME")]
        public DateTime? GraduationTime { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }
    }
}
