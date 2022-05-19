using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.Extend.Entitys.Dto.Employee
{
    [SuppressSniffer]
    public class ImportDataOutput
    {
        /// <summary>
        /// 导入失败信息
        /// </summary>
        public List<EmployeeListOutput> failResult { get; set; } = new List<EmployeeListOutput>();
        /// <summary>
        /// 失败条数
        /// </summary>
        public int fnum { get; set; }
        /// <summary>
        /// 导入是否成功（0：成功）
        /// </summary>
        public int resultType { get; set; }
        /// <summary>
        /// 成功条数
        /// </summary>
        public int snum { get; set; }
    }
}
