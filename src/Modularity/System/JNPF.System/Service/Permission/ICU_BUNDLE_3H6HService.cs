using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Entity._3h6h;
using JNPF.System.Entitys.Model._3h6h;
using JNPF.System.Interfaces._3h6h;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Service.Permission
{
    /// <summary>
    ///  
    /// </summary>
    [AllowAnonymous]
    //[ApiDescriptionSettings(Tag = "ICUBundle", Name = "ICU_BUNDLE_3H6H", Order = 7554)]
    [Route("api/icoubndle/[controller]")]
    public class ICU_BUNDLE_3H6HService : IICU_BUNDLE_3H6HService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ICU_BUNDLE_3H6HEntity> icu3h6hRepository;
        private readonly IICU_BUNDLE_3H6H_ITEMSService icu3h6hITEMS_service;

        public ICU_BUNDLE_3H6HService(ISqlSugarRepository<ICU_BUNDLE_3H6HEntity> icu3h6hRepository
            , IICU_BUNDLE_3H6H_ITEMSService icu3h6hITEMS_service
            )
        {
            this.icu3h6hRepository = icu3h6hRepository;
            this.icu3h6hITEMS_service = icu3h6hITEMS_service;
        }


        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="patientId"></param>
        /// <returns></returns>
        [HttpGet("GetICU_BUNDLE_3H6HEntity")]
        public async Task<IList<ICU_BUNDLE_3H6HEntity>> GetICU_BUNDLE_3H6HEntity(string patientId)
        {

            var res = await icu3h6hRepository.Entities.ToListAsync();
            var a = await icu3h6hITEMS_service.GetICU_BUNDLE_3H6H_ITEMSAll("");
            return res;
        }

        [HttpGet("GetICU_BUNDLE_3H6H_ITEMSAll")]
        public async Task<IList<ICUGroupItmes>> GetICU_BUNDLE_3H6H_ITEMSAll(string uuid)
        {
            if (string.IsNullOrWhiteSpace(uuid))
                return null;
            var data = await icu3h6hITEMS_service.ICU_BUNDLE_3H6H_ITEMSGROUPEntityAll(uuid);
            var res = data.GroupBy(r => r.GROUPNAME).Select(a => new ICUGroupItmes
            {
                GROUPNAME = a.Key,
                StartCurdTime = a.FirstOrDefault().StartCurdTime,
                EndCureTime = a.FirstOrDefault().EndCureTime,
                iCU_BUNDLE_3H6H_ITEMAndGropuRes = data.Where(c => c.GROUPNAME == a.Key).ToList()
            }).ToList();
            return res;
        }


        [HttpGet("UpdateICU_BUNDLE_3H6H")]
        public async Task<bool> UpdateICU_BUNDLE_3H6H(string uuid, string startuserid)
        {
            var sig = await icu3h6hRepository.SingleAsync(r => r.UUId == uuid);
            sig.StartUserId = startuserid;
            sig.StartCurdTime = DateTime.Now;
            return await icu3h6hRepository.Context.Updateable(sig).WhereColumns(r => r.UUId).ExecuteCommandAsync() > 0;
        }
    }
}
