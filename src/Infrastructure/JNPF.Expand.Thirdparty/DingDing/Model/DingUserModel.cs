using JNPF.Dependency;

namespace JNPF.Expand.Thirdparty.DingDing
{
    /// <summary>
    /// 钉钉用户
    /// </summary>
    [SuppressSniffer]
    public class DingUserModel
    {
        /// <summary>
        /// 入职时间，Unix时间戳，单位毫秒
        /// </summary>
        public long? HiredDate { get; set; }
        /// <summary>
        /// 备注，长度最大2000个字符
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 员工的企业邮箱
        /// </summary>
        public string OrgEmail { get; set; }
        /// <summary>
        /// 员工名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 手机号码，企业内必须唯一，不可重复
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 员工工号
        /// </summary>
        public string JobNumber { get; set; }
        /// <summary>
        /// 钉钉专属帐号初始密码
        /// </summary>
        public string InitPassword { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 是否号码隐藏
        /// </summary>
        public bool? HideMobile { get; set; }
        /// <summary>
        /// 扩展属性，可以设置多种属性，最大长度2000个字符
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 员工邮箱，长度最大50个字符。企业内必须唯一，不可重复
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 员工在对应的部门中的职位
        /// </summary>
        public string DeptTitleList { get; set; }
        /// <summary>
        /// 员工在对应的部门中的排序
        /// </summary>
        public string DeptOrderList { get; set; }
        /// <summary>
        /// 所属部门id列表
        /// </summary>
        public string DeptIdList { get; set; }
        /// <summary>
        /// 员工唯一标识ID（不可修改），企业内必须唯一
        /// </summary>
        public string Userid { get; set; }
        /// <summary>
        /// 办公地点
        /// </summary>
        public string WorkPlace { get; set; }
        /// <summary>
        /// 是否开启高管模式
        /// </summary>
        public bool? SeniorMode { get; set; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string Telephone { get; set; }
    }
}
