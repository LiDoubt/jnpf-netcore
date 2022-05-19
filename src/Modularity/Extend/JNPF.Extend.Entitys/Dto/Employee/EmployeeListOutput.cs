using JNPF.Dependency;
using System;

namespace JNPF.Extend.Entitys.Dto.Employee
{
    /// <summary>
    /// 获取职员列表(分页)
    /// </summary>
    [SuppressSniffer]
    public class EmployeeListOutput
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string enCode { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string fullName { get; set; }
        /// <summary>
        /// 性别ID
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string departmentName { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        public string positionName { get; set; }
        /// <summary>
        /// 用工性质
        /// </summary>
        public string workingNature { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string iDNumber { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? birthday { get; set; }
        /// <summary>
        /// 参加工作时间
        /// </summary>
        public DateTime? attendWorkTime { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        public string education { get; set; }
        /// <summary>
        /// 所学专业
        /// </summary>
        public string major { get; set; }
        /// <summary>
        /// 毕业院校
        /// </summary>
        public string graduationAcademy { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        public DateTime? graduationTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? creatorTime { get; set; }
    }
}
