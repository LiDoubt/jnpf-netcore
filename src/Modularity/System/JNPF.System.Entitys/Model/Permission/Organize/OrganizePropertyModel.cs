using JNPF.Dependency;

namespace JNPF.System.Entitys.Model.Permission.Organize
{
    /// <summary>
    /// 机构扩展属性
    /// </summary>
    [SuppressSniffer]
    public class OrganizePropertyModel
    {
        /// <summary>
        /// 公司简称
        /// </summary>
        public string shortName { get; set; }

        /// <summary>
        /// 公司主页
        /// </summary>
        public string webSite { get; set; }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string industry { get; set; }

        /// <summary>
        /// 成立时间(时间戳)
        /// </summary>
        public string foundedTime { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 公司法人
        /// </summary>
        public string managerName { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string managerTelePhone { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary>
        public string managerMobilePhone { get; set; }

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string manageEmail { get; set; }

        /// <summary>
        /// 开启银行
        /// </summary>
        public string bankName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string bankAccount { get; set; }

        /// <summary>
        /// 经营范围
        /// </summary>
        public string businessscope { get; set; }

        /// <summary>
        /// 公司性质(id)
        /// </summary>
        public string enterpriseNature { get; set; }

        /// <summary>
        /// 公司传真
        /// </summary>
        public string fax { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        public string telePhone { get; set; }
    }
}
