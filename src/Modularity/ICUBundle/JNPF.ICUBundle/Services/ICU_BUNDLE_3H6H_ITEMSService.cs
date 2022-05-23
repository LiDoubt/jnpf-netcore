using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.ICUBundle.Entitys.Entity;
using JNPF.ICUBundle.Entitys.Entity.Dto;
using JNPF.ICUBundle.Entitys.Model;
using JNPF.ICUBundle.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.ICUBundle.Services
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [ApiDescriptionSettings(Tag = "ICUBundle", Name = "ICU_BUNDLE_3H6H_ITEMS", Order = 785)]
    [Route("api/ICUBundle/[controller]")]

    public class ICU_BUNDLE_3H6H_ITEMSService : IICU_BUNDLE_3H6H_ITEMSService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<ICU_BUNDLE_3H6H_ITEMSEntity> icu3h6hITEMS_Repository;
        private readonly SqlSugarScope db;// 核心对象：拥有完整的SqlSugar全部功能
        public ICU_BUNDLE_3H6H_ITEMSService(ISqlSugarRepository<ICU_BUNDLE_3H6H_ITEMSEntity> icu3h6hITEMS_Repository)
        {
            this.icu3h6hITEMS_Repository = icu3h6hITEMS_Repository;
            this.db = icu3h6hITEMS_Repository.Context;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpGet("GetICU_BUNDLE_3H6H_ITEMSAll")]
        public async Task<IList<ICU_BUNDLE_3H6H_ITEMSEntity>> GetICU_BUNDLE_3H6H_ITEMSAll(string a)
        {
            var aa = await icu3h6hITEMS_Repository.Entities.ToListAsync();
            return aa;
        }

        public async Task<IList<ICU_BUNDLE_3H6H_ITEMAndGropuRes>> ICU_BUNDLE_3H6H_ITEMSGROUPEntityAll(string _3H6Huuid)
        {
            var data = await db.Queryable<ICU_BUNDLE_3H6HEntity, ICU_BUNDLE_3H6H_ITEMSEntity, ICU_BUNDLE_3H6H_ITEMSGROUPEntity>((a, b, c) => new JoinQueryInfos(JoinType.Left, a.UUId == b.Bundle3h6huuId, JoinType.Right, b.ItemName == c.ITEMNAME)).Where((a,b,c) => a.UUId == _3H6Huuid).Select((a, b, c) => new ICU_BUNDLE_3H6H_ITEMAndGropuRes
            {
                GROUPNAME = c.GROUPNAME,
                Bundle3h6huuId = b.Bundle3h6huuId,
                ItemName = b.ItemName,
                DataEndTime = b.DataEndTime,
                DataStartTime = b.DataStartTime,
                ItemValue = b.ItemValue,
                OperationTime = b.OperationTime,
                OperationUserId = b.OperationUserId,
                OrderNo = b.OrderNo,
                ICU_BUNDLE_3H6H_ITEMUUId = b.UUId,
                ICU_BUNDLE_3H6H_UUId = a.UUId,
                EndCureTime = a.EndCureTime,
                EndUserId = a.EndUserId,
                PatientId = a.PatientId,
                StartCurdTime = a.StartCurdTime,
                StartUserId = a.StartUserId

            }).ToListAsync();
            return data;
        }


        [HttpPost("UpdateICU_BUNDLE_3H6H_ITEMS")]
        public async Task<bool> UpdateICU_BUNDLE_3H6H_ITEMS(UpdateICU_BUNDLE_3H6H_ITEMSInput h6H_ITEMSInput)
        {
            var sig = await icu3h6hITEMS_Repository.Entities.FirstAsync(r => r.UUId == h6H_ITEMSInput.uuid);
            sig.DataStartTime = DateTime.Parse(h6H_ITEMSInput.starttime);
            sig.DataEndTime = DateTime.Parse(h6H_ITEMSInput.endtime);
            return await db.Updateable(sig).WhereColumns(it => new { it.UUId }).ExecuteCommandAsync()>0;
        }
    }
}
