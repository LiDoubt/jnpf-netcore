using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.Module
{
    /// <summary>
    /// 功能修改输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleUpInput : ModuleCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
