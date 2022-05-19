using JNPF.Common.Util;
using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.Extend.Entitys.Dto.TableExample
{
    /// <summary>
    /// 表格树形
    /// </summary>
    [SuppressSniffer]
    public class TableExampleTreeListOutput : TreeModel
    {
        public bool loaded { get; set; }
        public bool expanded { get; set; }
        public Dictionary<string, object> ht { get; set; }
        public string text { get; set; }
    }
}
