
using JNPF.ICUBundle.Entitys.Entity;
using JNPF.ICUBundle.Entitys.Entity.Dto;
using JNPF.ICUBundle.Entitys.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.ICUBundle.Interfaces
{
    public interface IICU_BUNDLE_3H6H_ITEMSService
    {
        Task<IList<ICU_BUNDLE_3H6H_ITEMSEntity>> GetICU_BUNDLE_3H6H_ITEMSAll(string a);


        Task<IList<ICU_BUNDLE_3H6H_ITEMAndGropuRes>> ICU_BUNDLE_3H6H_ITEMSGROUPEntityAll(string _3H6Huuid);

        Task<bool> UpdateICU_BUNDLE_3H6H_ITEMS(UpdateICU_BUNDLE_3H6H_ITEMSInput h6H_ITEMSInpu);


    }
}
