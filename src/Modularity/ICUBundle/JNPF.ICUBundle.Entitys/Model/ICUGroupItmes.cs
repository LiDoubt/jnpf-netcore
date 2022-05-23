using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.ICUBundle.Entitys.Model
{
    public class ICUGroupItmes
    {
        public string GROUPNAME { get; set; }

        /// <summary>
        /// 起始治疗时间
        /// </summary>
        public DateTime StartCurdTime { get; set; }


        /// <summary>
        /// 终止治疗时间
        /// </summary>
        public DateTime EndCureTime { get; set; }
        public List<ICU_BUNDLE_3H6H_ITEMAndGropuRes> iCU_BUNDLE_3H6H_ITEMAndGropuRes { get; set; }
    }
}
