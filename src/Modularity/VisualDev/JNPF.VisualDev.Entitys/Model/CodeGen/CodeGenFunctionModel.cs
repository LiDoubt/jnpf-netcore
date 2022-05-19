using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.VisualDev.Entitys.Model.CodeGen
{
    /// <summary>
    /// 代码生成功能模型
    /// </summary>
    public class CodeGenFunctionModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; }
        
        /// <summary>
        /// 是否接口
        /// </summary>
        public bool IsInterface { get; set; }
    }
}
