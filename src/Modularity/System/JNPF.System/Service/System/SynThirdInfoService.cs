using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Util;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.Expand.Thirdparty;
using JNPF.Expand.Thirdparty.DingDing;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using JNPF.System.Entitys.Dto.Permission.Organize;
using JNPF.System.Entitys.Dto.System.SynThirdInfo;
using JNPF.System.Entitys.Dto.System.SysConfig;
using JNPF.System.Entitys.Permission;
using JNPF.System.Entitys.System;
using JNPF.System.Interfaces.System;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.System.Core.Service.SynThirdInfo
{
    /// <summary>
    /// 第三方同步
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "System", Name = "SynThirdInfo", Order = 210)]
    [Route("api/system/[controller]")]
    public class SynThirdInfoService: ISynThirdInfoService, IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<SynThirdInfoEntity> _synThirdInfoRepository;
        private readonly ISysConfigService _sysConfigService;
        private readonly ISqlSugarRepository<OrganizeEntity> _organizeRepository;
        private readonly ISqlSugarRepository<UserEntity> _userRepository;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="synThirdInfoRepository"></param>
        /// <param name="sysConfigService"></param>
        /// <param name="organizeRepository"></param>
        /// <param name="userRepository"></param>
        public SynThirdInfoService(ISqlSugarRepository<SynThirdInfoEntity> synThirdInfoRepository, ISysConfigService sysConfigService, ISqlSugarRepository<OrganizeEntity> organizeRepository, ISqlSugarRepository<UserEntity> userRepository)
        {
            _synThirdInfoRepository = synThirdInfoRepository;
            _sysConfigService = sysConfigService;
            _organizeRepository = organizeRepository;
            _userRepository = userRepository;
        }

        #region Get
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="thirdType">请求参数</param>
        /// <returns></returns>
        [HttpGet("getSynThirdTotal/{thirdType}")]
        public async Task<dynamic> GetList(int thirdType)
        {
            var whereLambda = LinqExpression.And<SynThirdInfoEntity>();
            whereLambda = whereLambda.And(x => x.ThirdType == thirdType);
            return await GetListByThirdType(whereLambda,"用户", "组织");
        }

        /// <summary>
        /// 钉钉同步组织
        /// </summary>
        /// <returns></returns>
        [HttpGet("synAllOrganizeSysToDing")]
        public async Task<dynamic> synAllOrganizeSysToDing()
        {
            var flag = await SynData(2, 1);
            var whereLambda = LinqExpression.And<SynThirdInfoEntity>();
            whereLambda = whereLambda.And(x => x.ThirdType == 2 && x.DataType < 3);
            return await GetListByThirdType(whereLambda, "组织", "组织");
        }

        /// <summary>
        /// 企业微信同步组织
        /// </summary>
        /// <returns></returns>
        [HttpGet("synAllOrganizeSysToQy")]
        public async Task<dynamic> synAllOrganizeSysToQy()
        {
            var flag = await SynData(1, 1);
            var whereLambda = LinqExpression.And<SynThirdInfoEntity>();
            whereLambda = whereLambda.And(x => x.ThirdType == 1 && x.DataType < 3);
            return await GetListByThirdType(whereLambda, "组织", "组织");
        }

        /// <summary>
        /// 钉钉同步用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("synAllUserSysToDing")]
        public async Task<dynamic> synAllUserSysToDing()
        {
            var flag = await SynData(2, 3);
            var whereLambda = LinqExpression.And<SynThirdInfoEntity>();
            whereLambda = whereLambda.And(x => x.ThirdType == 2 && x.DataType == 3);
            return await GetListByThirdType(whereLambda, "用户", "用户");
        }

        /// <summary>
        /// 企业微信同步用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("synAllUserSysToQy")]
        public async Task<dynamic> synAllUserSysToQy()
        {
            var flag = await SynData(1, 3);
            var whereLambda = LinqExpression.And<SynThirdInfoEntity>();
            whereLambda = whereLambda.And(x => x.ThirdType == 1 && x.DataType == 3);
            return await GetListByThirdType(whereLambda, "用户", "用户");
        }
        #endregion

        #region Method
        /// <summary>
        /// 获取同步数据
        /// </summary>
        /// <param name="whereLambda"></param>
        /// <param name="synType1"></param>
        /// <param name="synType2"></param>
        /// <returns></returns>
        private async Task<dynamic> GetListByThirdType(Expression<Func<SynThirdInfoEntity, bool>> whereLambda,string synType1,string synType2)
        {
            var synThirdInfoList = await _synThirdInfoRepository.Entities.Where(whereLambda).ToListAsync();
            var userList = await _userRepository.Entities.Where(x => x.DeleteMark == null).ToListAsync();
            var orgList = await _organizeRepository.Entities.Where(x => x.DeleteMark == null).ToListAsync();
            if (synType1.Equals(synType2))
            {
                var output = new SynThirdInfoConutOutput()
                {
                    synType = synType1,
                    recordTotal = synType1.Equals("组织") ? orgList.Count : userList.Count,
                    synDate= synThirdInfoList.Select(x => x.LastModifyTime).ToList().Max().IsEmpty() ? synThirdInfoList.Select(x => x.CreatorTime).ToList().Max() : synThirdInfoList.Select(x => x.LastModifyTime).ToList().Max(),
                    synFailCount = synThirdInfoList.FindAll(x => x.SynState.Equals("2")).Count,
                    synSuccessCount = synThirdInfoList.FindAll(x => x.SynState.Equals("1")).Count,
                    unSynCount = synThirdInfoList.FindAll(x => x.SynState.Equals("0")).Count,
                };
                return output;


            }
            else
            {
                var output = new List<SynThirdInfoConutOutput>();
                var synUserList = synThirdInfoList.FindAll(x => x.DataType == 3);
                var synOrgList = synThirdInfoList.FindAll(x => x.DataType < 3);
                output.Add(new SynThirdInfoConutOutput()
                {
                    synType = synType2,
                    recordTotal = synType2.Equals("组织") ? orgList.Count : userList.Count,
                    synDate = synOrgList.Select(x => x.LastModifyTime).ToList().Max().IsEmpty() ? synOrgList.Select(x => x.CreatorTime).ToList().Max() : synOrgList.Select(x => x.LastModifyTime).ToList().Max(),
                    synFailCount = synOrgList.FindAll(x => x.SynState.Equals("2")).Count,
                    synSuccessCount = synOrgList.FindAll(x => x.SynState.Equals("1")).Count,
                    unSynCount = synOrgList.FindAll(x => x.SynState.Equals("0")).Count,
                });
                output.Add(new SynThirdInfoConutOutput() {
                    synType = synType1,
                    recordTotal = synType1.Equals("组织") ? orgList.Count : userList.Count,
                    synDate = synUserList.Select(x => x.LastModifyTime).ToList().Max().IsEmpty() ? synUserList.Select(x => x.CreatorTime).ToList().Max() : synUserList.Select(x => x.LastModifyTime).ToList().Max(),
                    synFailCount = synUserList.FindAll(x => x.SynState.Equals("2")).Count,
                    synSuccessCount = synUserList.FindAll(x => x.SynState.Equals("1")).Count,
                    unSynCount = synUserList.FindAll(x => x.SynState.Equals("0")).Count,
                });
                return output;
            }
        }

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private async Task<int> SynData(int thirdType, int dataType)
        {
            try
            {
                var sysConfig = await _sysConfigService.GetInfo();
                var synThirdInfo = await _synThirdInfoRepository.Entities.Where(x=>x.ThirdType==thirdType).ToListAsync();
                var orgList = (await _organizeRepository.Entities.Where(x=>x.DeleteMark==null).ToListAsync()).Adapt<List<OrganizeListOutput>>().ToTree("-1");
                var userList =await _userRepository.Entities.Where(x=>x.DeleteMark==null).ToListAsync();
                if (dataType == 3)
                {
                    await SynUser(thirdType, dataType, sysConfig, userList);
                }
                else
                {
                    await SynDep(thirdType, dataType, sysConfig, orgList);
                }
                return 1;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        /// <summary>
        /// 删除第三方数据
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <param name="sysConfig"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [NonAction]
        public async Task DelSynData(int thirdType, int dataType, SysConfigOutput sysConfig, string id)
        {
            try
            {
                string msg = "";
                var synInfo = await _synThirdInfoRepository.FirstOrDefaultAsync(x => x.ThirdType == thirdType && x.DataType == dataType && x.SysObjId == id);
                if (synInfo.IsNullOrEmpty() || synInfo.ThirdObjId.IsNullOrEmpty())
                    throw JNPFException.Oh(ErrorCode.D9004);
                if (thirdType == 1)
                {
                    var weChat = new WeChat(sysConfig.qyhCorpId, sysConfig.qyhCorpSecret);
                    if (dataType == 3)
                    {
                        weChat.DeleteMember(synInfo.ThirdObjId);
                    }
                    else
                    {
                        weChat.DeleteDepartment(synInfo.ThirdObjId.ToInt(), ref msg);
                    }
                }
                else
                {
                    var ding = new Ding(sysConfig.dingSynAppKey, sysConfig.dingSynAppSecret);
                    if (dataType == 3)
                    {
                        ding.DeleteUser(new DingUserModel() { Userid = synInfo.ThirdObjId }, ref msg);
                    }
                    else
                    {
                        ding.DeleteDep(new DingDepModel() { DeptId = synInfo.ThirdObjId.ToInt() }, ref msg);
                    }
                }
                await _synThirdInfoRepository.DeleteAsync(synInfo);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>
        /// 判断是否存在同步数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private bool IsExistThirdObjId(string id, int thirdType, int dataType)
        {
            var falg = _synThirdInfoRepository.Any(x => x.ThirdType == thirdType && x.DataType == dataType && x.SysObjId.Equals(id)&&!SqlFunc.IsNullOrEmpty(x.ThirdObjId));
            return !falg;
        }

        /// <summary>
        /// 保存同步数据
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <param name="sysObjId"></param>
        /// <param name="thirdObjId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private async Task Save(int thirdType, int dataType, string sysObjId, string thirdObjId, string msg)
        {
            var entity = await _synThirdInfoRepository.FirstOrDefaultAsync(x => x.SysObjId == sysObjId && x.ThirdType == thirdType);
            if (entity==null)
            {
                entity = new SynThirdInfoEntity();
                entity.Id = YitIdHelper.NextId().ToString();
                entity.ThirdType = thirdType;
                entity.DataType = dataType;
                entity.SysObjId = sysObjId;
                entity.ThirdObjId = thirdObjId;
                entity.SynState = thirdObjId.IsNullOrEmpty() ? "2" : "1";
                entity.Description = msg;
                var newDic = await _synThirdInfoRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteReturnEntityAsync();
                _ = newDic ?? throw JNPFException.Oh(ErrorCode.D9005);
            }
            else
            {
                entity.ThirdObjId = thirdObjId;
                entity.SynState = thirdObjId.IsEmpty() ? "2" : "1";
                entity.Description = msg;
                var isOk = await _synThirdInfoRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
                if (isOk < 0)
                    throw JNPFException.Oh(ErrorCode.D9006);
            }
        }

        /// <summary>
        /// 获取第三方部门
        /// </summary>
        /// <param name="organizeId"></param>
        /// <param name="thirdType"></param>
        /// <param name="thirdDepList"></param>
        private void GetThirdDep(string organizeId, int thirdType, List<int> thirdDepList)
        {
            var info = _synThirdInfoRepository.FirstOrDefault(x => x.SysObjId == organizeId && x.ThirdType == thirdType);
            if (info.IsNotEmptyOrNull() && info.ThirdObjId.IsNotEmptyOrNull())
            {
                thirdDepList.Add(Convert.ToInt32(info.ThirdObjId));
            }
        }

        /// <summary>
        /// 根据系统主键获取第三方主键
        /// </summary>
        /// <param name="ids">系统主键</param>
        /// <param name="thirdType">第三方类型</param>
        /// <param name="dataType">数据类型</param>
        /// <returns></returns>
        [NonAction]
        public async Task<List<string>> GetThirdIdList(List<string> ids,int thirdType,int dataType)
        {
            return await _synThirdInfoRepository.Entities.Where(x => x.ThirdType == thirdType
            && x.DataType == dataType && !SqlFunc.IsNullOrEmpty(x.ThirdObjId)).
            In(x => x.SysObjId, ids.ToArray()).Select(x=>x.ThirdObjId).ToListAsync();
        }

        #region 部门同步
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thirdType">第三方类型</param>
        /// <param name="dataType">组织类型</param>
        /// <param name="sysConfig">系统配置</param>
        /// <param name="orgList">组织</param>
        /// <returns></returns>
        [NonAction]
        public async Task SynDep(int thirdType, int dataType, SysConfigOutput sysConfig, List<OrganizeListOutput> orgList)
        {
            try
            {
                if (thirdType == 1)
                {
                    var weChat = new WeChat(sysConfig.qyhCorpId, sysConfig.qyhCorpSecret);
                    foreach (var item in orgList)
                    {
                        await WeChatDep(item, weChat, thirdType, dataType);
                    }
                }
                else
                {
                    var ding = new Ding(sysConfig.dingSynAppKey, sysConfig.dingSynAppSecret);
                    foreach (var item in orgList)
                    {
                        dataType = item.category.Equals("company") ? 1 : 2;
                        await DingDep(item, ding, thirdType, dataType);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task WeChatDep(OrganizeListOutput org, WeChat weChatQYHelper, int thirdType, int dataType)
        {

            long parentid = 0;
            if (org.parentId.Equals("-1"))
            {
                parentid = 1;
            }
            else
            {
                var entity = await _synThirdInfoRepository.FirstOrDefaultAsync(x => x.SysObjId==org.parentId && x.ThirdType == thirdType);
                if (entity!=null && entity.SynState == "1")
                {
                    parentid = Convert.ToInt32(entity.ThirdObjId);
                }
            }
            var thirdObjId = "";
            var msg = "";
            if (IsExistThirdObjId(org.id, thirdType, dataType))
            {
                thirdObjId = weChatQYHelper.CreateDepartment(org.fullName, parentid, 1,ref msg).ToString();
                if (thirdObjId.Equals("0"))
                {
                    thirdObjId = "";
                }
            }
            else
            {
                var synEntity = await _synThirdInfoRepository.FirstOrDefaultAsync(x => org.id == x.SysObjId && thirdType == x.ThirdType);
                if (synEntity.IsNotEmptyOrNull())
                {
                    thirdObjId = synEntity.ThirdObjId;
                    var id = Convert.ToInt32(thirdObjId);
                    var flag = weChatQYHelper.UpdateDepartment(id, org.fullName, (int)parentid, 1, ref msg);
                    thirdObjId = flag ? thirdObjId : "";
                }
            }
            await Save(thirdType, dataType, org.id, thirdObjId, msg);
            if (org.hasChildren)
            {
                foreach (var item in org.children)
                {
                    var orgChild = item.Adapt<OrganizeListOutput>();
                    dataType = orgChild.category.Equals("company") ? 1 : 2;
                    await WeChatDep(orgChild, weChatQYHelper, thirdType, dataType);
                }
            }
        }

        private async Task DingDep(OrganizeListOutput org, Ding dingHelper, int thirdType, int dataType)
        {
            var dingDep = new DingDepModel();
            dingDep.Name = org.fullName;
            if (org.parentId.Equals("-1"))
            {
                dingDep.ParentId = 1;
            }
            else
            {
                var entity = await _synThirdInfoRepository.FirstOrDefaultAsync(x => x.SysObjId == org.parentId && x.ThirdType == thirdType);
                if (entity!=null && entity.SynState == "1")
                {
                    dingDep.ParentId = Convert.ToInt32(entity.ThirdObjId);
                }
            }
            var thirdObjId = "";
            var msg = "";
            if (IsExistThirdObjId(org.id, thirdType, dataType))
            {
                thirdObjId = dingHelper.CreateDep(dingDep,ref msg);
            }
            else
            {
                var synEntity = await _synThirdInfoRepository.FirstOrDefaultAsync(x => org.id == x.SysObjId && thirdType == x.ThirdType);
                if (synEntity.IsNotEmptyOrNull())
                {
                    thirdObjId = synEntity.ThirdObjId;
                    dingDep.DeptId = Convert.ToInt32(thirdObjId);
                    var flag = dingHelper.UpdateDep(dingDep, ref msg);
                    thirdObjId = flag ? thirdObjId : "";
                }
                
            }
            await Save(thirdType, dataType, org.id, thirdObjId, msg);
            if (org.hasChildren)
            {
                foreach (var item in org.children)
                {
                    var orgChild = item.Adapt<OrganizeListOutput>();
                    dataType = orgChild.category.Equals("company") ? 1 : 2;
                    await DingDep(orgChild, dingHelper, thirdType, dataType);
                }
            }
        }
        #endregion

        #region 用户同步
        /// <summary>
        /// 
        /// </summary>
        /// <param name="thirdType"></param>
        /// <param name="dataType"></param>
        /// <param name="sysConfig"></param>
        /// <param name="userList"></param>
        /// <returns></returns>
        [NonAction]
        public async Task SynUser(int thirdType, int dataType, SysConfigOutput sysConfig, List<UserEntity> userList)
        {
            if (thirdType == 1)
            {
                var weChat = new WeChat(sysConfig.qyhCorpId, sysConfig.qyhCorpSecret);
                foreach (var item in userList)
                {
                    await WeChatUser(item, weChat, thirdType, dataType);
                }
            }
            else
            {
                var ding = new Ding(sysConfig.dingSynAppKey, sysConfig.dingSynAppSecret);
                foreach (var item in userList)
                {
                    await DingUser(item, ding, thirdType, dataType);
                }
            }
        }

        private async Task WeChatUser(UserEntity user, WeChat weChatQYHelper, int thirdType, int dataType)
        {
            var qyUser = new QYMemberModel();
            List<int> depList = new List<int>();
            GetThirdDep(user.OrganizeId, thirdType, depList);
            qyUser.userid = user.Id;
            qyUser.name = user.RealName;
            qyUser.mobile = user.MobilePhone;
            qyUser.email = user.Email;
            qyUser.department = depList.Select(x => (long)x).ToArray();
            var thirdObjId = "";
            var msg = "";
            if (IsExistThirdObjId(user.Id, thirdType, dataType))
            {
                var flag = weChatQYHelper.CreateMember(qyUser,ref msg);
                thirdObjId = flag ? user.Id : "";
            }
            else
            {
                var flag = weChatQYHelper.UpdateMember(qyUser, ref msg);
                thirdObjId = flag ? user.Id : "";
            }
            await Save(thirdType, dataType, user.Id, thirdObjId, msg);
        }

        private async Task DingUser(UserEntity user, Ding dingHelper, int thirdType, int dataType)
        {
            var dingUser = new DingUserModel();
            List<int> depList = new List<int>();
            GetThirdDep(user.OrganizeId, thirdType, depList);
            dingUser.Name = user.RealName;
            dingUser.Mobile = user.MobilePhone;
            dingUser.Email = user.Email;
            dingUser.DeptIdList = string.Join(",", depList);
            var thirdObjId = "";
            var msg = "";
            if (IsExistThirdObjId(user.Id, thirdType, dataType))
            {
                var userId = dingHelper.CreateUser(dingUser, ref msg);
                thirdObjId = userId;
            }
            else
            {
                thirdObjId = _synThirdInfoRepository.FirstOrDefault(x => x.SysObjId == user.Id && x.ThirdType == thirdType).ThirdObjId;
                dingUser.Userid = thirdObjId;
                var flag = dingHelper.UpdateUser(dingUser, ref msg);
                thirdObjId = flag ? thirdObjId : "";
            }
            await Save(thirdType, dataType, user.Id, thirdObjId, msg);
        }
        #endregion

        #endregion
    }
}
