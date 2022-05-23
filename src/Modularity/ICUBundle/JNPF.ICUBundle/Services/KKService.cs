using JNPF.Dependency;
using JNPF.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.ICUBundle.Services
{
    [AllowAnonymous]
    //[ApiDescriptionSettings(Tag = "ICUBundle", Name = "ICU_BUNDLE_3H6H", Order = 7554)]
    [Route("api/icoubndle/[controller]")]
    public class KKService:IDynamicApiController, ITransient
    {

        [HttpGet("kk")]
        public string KK()
        {
            return "666";
        }
    }
}
