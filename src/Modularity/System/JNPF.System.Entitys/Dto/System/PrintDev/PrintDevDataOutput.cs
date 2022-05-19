using JNPF.Dependency;
using JNPF.System.Entitys.Model.System.PrintDev;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.System.PrintDev
{
    [SuppressSniffer]
    public class PrintDevDataOutput
    {
        /// <summary>
        /// sql数据
        /// </summary>
        public object printData { get; set; }

        /// <summary>
        /// 模板数据
        /// </summary>
        public string printTemplate { get; set; }

        /// <summary>
        /// 审批数据
        /// </summary>
        public List<PrintDevDataModel> flowTaskOperatorRecordList { get; set; } = new List<PrintDevDataModel>();
    }
}
