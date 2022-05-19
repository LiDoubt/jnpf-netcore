namespace JNPF.VisualDev.Entitys.Dto.VisualDev
{
    /// <summary>
    /// 下载代码表单输入
    /// </summary>
    public class DownloadCodeFormInput
    {
        /// <summary>
        /// 所属模块
        /// </summary>
        public string module { get; set; }

        /// <summary>
        /// 主功能名称
        /// </summary>
        public string className { get; set; }

        /// <summary>
        /// 子表名称集合
        /// </summary>
        public string subClassName { get; set; }

        /// <summary>
        /// 主功能备注
        /// </summary>
        public string description { get; set; }
    }
}
