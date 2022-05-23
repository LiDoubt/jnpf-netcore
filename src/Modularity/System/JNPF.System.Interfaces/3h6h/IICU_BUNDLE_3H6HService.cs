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
    public interface IICU_BUNDLE_3H6HService
    {

        Task<IList<ICU_BUNDLE_3H6HEntity>> GetICU_BUNDLE_3H6HEntity(string patientId);

        Task<bool> UpdateICU_BUNDLE_3H6H(string uuid, string startuserid);
    }
}
