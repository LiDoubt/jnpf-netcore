using JNPF.Dependency;
using System.ComponentModel.DataAnnotations;

namespace JNPF.System.Entitys.Dto.System.ComFields
{
    /// <summary>
    /// 
    /// </summary>
    [SuppressSniffer]
    public class ComFieldsUpInput : ComFieldsCrInput
    {
        /// <summary>
        /// id
        /// </summary>

        [Required(ErrorMessage = "id不能为空")]
        public string id { get; set; }
    }
}
