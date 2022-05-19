using JNPF.Dependency;

namespace JNPF.System.Entitys.Dto.System.ModuleButton
{
    /// <summary>
    /// 功能按钮修改输入
    /// </summary>
    [SuppressSniffer]
    public class ModuleButtonUpInput : ModuleButtonCrInput
    {
        /// <summary>
        /// id
        /// </summary>
        public string id { get; set; }
    }
}
