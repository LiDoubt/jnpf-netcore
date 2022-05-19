using JNPF.Common.Const;
using JNPF.Common.Entity;
using SqlSugar;

namespace JNPF.System.Entitys.System
{
    /// <summary>
    /// 数据接口
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [SugarTable("BASE_DATAINTERFACE")]
    [Tenant(ClaimConst.TENANT_ID)]
    public class DataInterfaceEntity : CLDEntityBase
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        [SugarColumn(ColumnName = "F_CATEGORYID")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 接口名
        /// </summary>
        [SugarColumn(ColumnName = "F_FULLNAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 接口链接
        /// </summary>
        [SugarColumn(ColumnName = "F_PATH")]
        public string Path { get; set; }

        /// <summary>
        /// 请求方式
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTMETHOD")]
        public string RequestMethod { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        [SugarColumn(ColumnName = "F_RESPONSETYPE")]
        public string ResponseType { get; set; }

        /// <summary>
        /// 查询语句
        /// </summary>
        [SugarColumn(ColumnName = "F_QUERY")]
        public string Query { get; set; }

        /// <summary>
        /// 接口入参
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTPARAMETERS")]
        public string RequestParameters { get; set; }

        /// <summary>
        /// 接口出参
        /// </summary>
        [SugarColumn(ColumnName = "F_RESPONSEPARAMETERS")]
        public string ResponseParameters { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [SugarColumn(ColumnName = "F_ENCODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [SugarColumn(ColumnName = "F_SORTCODE")]
        public long? SortCode { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 数据源id
        /// </summary>
        [SugarColumn(ColumnName = "F_DBLINKID")]
        public string DBLinkId { get; set; }

        /// <summary>
        /// 数据类型(1-SQL数据，2-静态数据，3-Api数据)
        /// </summary>
        [SugarColumn(ColumnName = "F_DATATYPE")]
        public int? DataType { get; set; }

        /// <summary>
        /// 验证规则(0-不验证，1-授权，2-域名)
        /// </summary>
        [SugarColumn(ColumnName = "F_CHECKTYPE")]
        public int? CheckType { get; set; }

        /// <summary>
        /// 请求头
        /// </summary>
        [SugarColumn(ColumnName = "F_REQUESTHEADERS")]
        public string RequestHeaders { get; set; }
    }
}
