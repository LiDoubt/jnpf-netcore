using JNPF.System.Entitys.Dto._3h6h;
using JNPF.System.Entitys.Entity._3h6h;
using JNPF.System.Entitys.Model._3h6h;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces._3h6h
{
    public interface IICU_BUNDLE_3H6H_ITEMSService
    {
        Task<IList<ICU_BUNDLE_3H6H_ITEMSEntity>> GetICU_BUNDLE_3H6H_ITEMSAll(string a);


        Task<IList<ICU_BUNDLE_3H6H_ITEMAndGropuRes>> ICU_BUNDLE_3H6H_ITEMSGROUPEntityAll(string _3H6Huuid);

        Task<bool> UpdateICU_BUNDLE_3H6H_ITEMS(UpdateICU_BUNDLE_3H6H_ITEMSInput h6H_ITEMSInpu);


    }
}
