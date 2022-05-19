using JNPF.Dependency;

namespace JNPF.Expand.Thirdparty.DingDing
{
    /// <summary>
    /// 钉钉部门
    /// </summary>
    [SuppressSniffer]
    public class DingDepModel
    {
        /// <summary>
        /// 部门群是否包含外包部门
        /// </summary>
        public bool? GroupContainOuterDept { get; set; }
        /// <summary>
        /// 父部门ID
        /// </summary>
        public long? ParentId { get; set; }
        /// <summary>
        /// 部门员工可见员工列表
        /// </summary>
        public string OuterPermitUsers { get; set; }
        /// <summary>
        /// 部门员工可见部门列表
        /// </summary>
        public string OuterPermitDepts { get; set; }
        /// <summary>
        /// 本部门成员是否只能看到所在部门及下级部门通讯录
        /// </summary>
        public bool? OuterDeptOnlySelf { get; set; }
        /// <summary>
        /// 是否限制本部门成员查看通讯录
        /// </summary>
        public bool? OuterDept { get; set; }
        /// <summary>
        /// 企业群群主的userid
        /// </summary>
        public string OrgDeptOwner { get; set; }
        /// <summary>
        /// 在父部门中的排序值
        /// </summary>
        public long? Order { get; set; }
        /// <summary>
        /// 部门名称，长度限制为1~64个字符，不允许包含字符‘-’‘，’以及‘,’
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 是否隐藏本部门
        /// </summary>
        public bool? HideDept { get; set; }
        /// <summary>
        /// 部门群是否包含子部门
        /// </summary>
        public bool? GroupContainSubDept { get; set; }
        /// <summary>
        /// 指定可以查看本部门的用户userid列表
        /// </summary>
        public string UserPermits { get; set; }
        /// <summary>
        /// 部门群是否包含隐藏部门
        /// </summary>
        public bool? GroupContainHiddenDept { get; set; }
        /// <summary>
        /// 扩展
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 指定可以查看本部门的其他部门列表
        /// </summary>
        public string DeptPermits { get; set; }
        /// <summary>
        /// 部门的主管userid列表
        /// </summary>
        public string DeptManagerUseridList { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public long? DeptId { get; set; }
        /// <summary>
        /// 是否创建一个关联此部门的企业群
        /// </summary>
        public bool? CreateDeptGroup { get; set; }
        /// <summary>
        /// 当部门群已经创建后，有新人加入部门时是否会自动加入该群
        /// </summary>
        public bool? AutoAddUser { get; set; }
        /// <summary>
        /// 部门标识字段，开发者可用该字段来唯一标识一个部门，并与钉钉外部通讯录里的部门做映射
        /// </summary>
        public string SourceIdentifier { get; set; }
    }
}
