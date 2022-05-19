using System.Collections.Generic;

namespace JNPF.VisualDev.Entitys.Model.VisualDevModelData
{
    public class OptionsModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<OptionsModel> children { get; set; }
    }
}
