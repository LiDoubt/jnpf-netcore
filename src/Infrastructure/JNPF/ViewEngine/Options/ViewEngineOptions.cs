using JNPF.Dependency;
using JNPF.Reflection;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Reflection;

namespace JNPF.ViewEngine
{
    /// <summary>
    /// 视图引擎编译选项
    /// </summary>
    [SuppressSniffer]
    public class ViewEngineOptions
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ViewEngineOptions()
        {
            ReferencedAssemblies = new HashSet<Assembly>()
            {
                typeof(object).Assembly,
                typeof(ViewEngineModel).Assembly,
                typeof(System.Collections.IList).Assembly,
                typeof(IEnumerable<>).Assembly,
                Reflect.GetAssembly("Microsoft.CSharp"),
                Reflect.GetAssembly("System.Runtime"),
                Reflect.GetAssembly("System.Linq"),
                Reflect.GetAssembly("System.Linq.Expressions")
            };
        }

        /// <summary>
        /// 引用程序集
        /// </summary>
        public HashSet<Assembly> ReferencedAssemblies { get; set; }

        /// <summary>
        /// 元数据引用
        /// </summary>
        public HashSet<MetadataReference> MetadataReferences { get; set; } = new HashSet<MetadataReference>();

        /// <summary>
        /// 模板命名空间
        /// </summary>
        public string TemplateNamespace { get; set; } = "JNPF.ViewEngine";

        /// <summary>
        /// 继承
        /// </summary>
        public string Inherits { get; set; } = "JNPF.ViewEngine.Template.Models";

        /// <summary>
        /// 默认 Using
        /// </summary>
        public HashSet<string> DefaultUsings { get; set; } = new HashSet<string>()
        {
            "System.Linq",
            "System.Collections",
            "System.Collections.Generic"
        };
    }
}