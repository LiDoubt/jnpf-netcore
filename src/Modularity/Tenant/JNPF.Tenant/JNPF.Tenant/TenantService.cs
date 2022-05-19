using JNPF.Common.Enum;
using JNPF.Common.Extension;
using JNPF.Common.Filter;
using JNPF.DataEncryption;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.LinqBuilder;
using JNPF.Tenant.Entitys;
using JNPF.Tenant.Entitys.Dto;
using JNPF.Tenant.Entitys.Dto.Account;
using JNPF.Tenant.Entitys.Dtos;
using JNPF.Tenant.Entitys.Entity;
using JNPF.Tenant.Entitys.Model;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace JNPF.Tenant
{
    /// <summary>
    /// 租户信息
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    [ApiDescriptionSettings(Tag = "Saas", Name = "Tenant", Order = 600)]
    [Route("api/[controller]")]
    public class TenantService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<TenantEntity> _tenantRepository;
        private readonly ISqlSugarRepository<TenantLogEntity> _tenantLogRepository;
        //private readonly IDataBaseService _dataBaseService;
        private readonly ISqlSugarRepository<AccountEntity> _accountRepository;
        private readonly SqlSugarScope db;

        public TenantService(ISqlSugarRepository<TenantEntity> tenantRepository, 
            ISqlSugarRepository<TenantLogEntity> tenantLogRepository,
            ISqlSugarRepository<AccountEntity> accountRepository)
        {
            _tenantRepository = tenantRepository;
            _tenantLogRepository = tenantLogRepository;
            db = _tenantRepository.Context;
            //_dataBaseService = dataBaseService;
            _accountRepository = accountRepository;
        }

        #region Get
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<dynamic> GetList([FromQuery]TenantListQuery input)
        {
            var queryWhere = LinqExpression.And<TenantEntity>().And(x => x.DeleteMark == null);
            if (!string.IsNullOrEmpty(input.keyword))
                queryWhere = queryWhere.And(m => m.FullName.Contains(input.keyword) || m.EnCode.Contains(input.keyword) || m.CompanyName.Contains(input.keyword));
            if (input.endTime != null && input.startTime != null)
            {
                var start = Ext.GetDateTime(input.startTime.ToString());
                var end = Ext.GetDateTime(input.endTime.ToString());
                start = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, 0);
                end = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59, 999);
                queryWhere = queryWhere.And(x => SqlFunc.Between(x.CreatorTime, start, end));
            }
            var list = await _tenantRepository.Entities.Where(queryWhere).OrderBy(x => x.CreatorTime, OrderByType.Desc).ToPagedListAsync(input.currentPage, input.pageSize);
            var pageList = new SqlSugarPagedList<TenantListOutput>()
            {
                list = list.list.Adapt<List<TenantListOutput>>(),
                pagination = list.pagination
            };
            return PageResult<TenantListOutput>.SqlSugarPageResult(pageList);
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<dynamic> GetInfo(string id)
        {
            var output = (await _tenantRepository.FirstOrDefaultAsync(x=>x.Id==id&&x.DeleteMark==null)).Adapt<TenantInfoOutput>();
            return output;
        }
        /// <summary>
        /// 获取租户数据库
        /// </summary>
        /// <param name="encode"></param>
        /// <returns></returns>
        [HttpGet("DbName/{encode}")]
        public async Task<dynamic> GetDbContent(string encode)
        {
            var data = await _tenantRepository.FirstOrDefaultAsync(x=>x.EnCode==encode&&x.DeleteMark==null);
            if (data == null)
                throw JNPFException.Oh(ErrorCode.Zh10000);
            if (DateTime.Now > data.ExpiresTime)
                throw JNPFException.Oh(ErrorCode.Zh10001);
            TenantLogEntity tenantLogEntity = new TenantLogEntity()
            {
                Id = YitIdHelper.NextId().ToString(),
                TenantId = data.Id,
                LoginAccount = encode,
                LoginIPAddress = data.IPAddress,
                LoginIPAddressName = data.IPAddressName,
                LoginSourceWebsite = data.SourceWebsite,
                LoginTime= DateTime.Now
            };
            await _tenantLogRepository.InsertAsync(tenantLogEntity);
            var output = new TenantDbContentOutput();
            output.dbName = data.DbName;
            return output;
        }
        #endregion

        #region Post
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task Create_Api([FromBody] TenantCrInput input)
        {
            if (await _tenantRepository.AnyAsync(x=>x.EnCode==input.enCode&&x.DeleteMark==null))
                throw JNPFException.Oh(ErrorCode.Zh10002);
            var entity = new TenantEntity()
            {
                FullName = input.fullName,
                EnCode = input.enCode,
                CompanyName = input.companyName,
                DbName= "jnpf_tenant_" + Ext.GetTimeStamp,
                ExpiresTime = input.expiresTime == 0 ? DateTime.Now.AddMonths(1) : Ext.GetDateTime(input.expiresTime.ToString()),
                Description = input.description
            };
            var isOk = await Create(entity);
            if(isOk<1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task Update([FromBody] TenantUpInput input, string id)
        {
            if (await _tenantRepository.AnyAsync(x =>x.Id!=id&& x.EnCode == input.enCode && x.DeleteMark == null))
                throw JNPFException.Oh(ErrorCode.Zh10002);
            var entity = new TenantEntity()
            {
                FullName = input.fullName,
                EnCode = input.enCode,
                CompanyName = input.companyName,
                ExpiresTime = input.expiresTime == 0 ? DateTime.Now : Ext.GetDateTime(input.expiresTime.ToString()),
                Description = input.description,
                Id = id
            };
            var isOk = await _tenantRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1000);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete_Api([FromBody] TenantDeleteQuery input, string id)
        {
            var entity = await _tenantRepository.FirstOrDefaultAsync(x=>x.Id==id&&x.DeleteMark==null);
            if (entity == null)
                throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await Delete(entity, (int)input.isClear);
            if (isOk < 1)
                throw JNPFException.Oh(ErrorCode.COM1002);
        }
        /// <summary>
        /// 创建租户数据库
        /// </summary>
        /// <param name="dbName">数据库名</param>
        /// <returns></returns>
        [HttpPost("DbName/{dbName}")]
        public async Task CreateDb(string dbName)
        {
            await CreateDB(dbName);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="id">主键值</param>
        /// <returns></returns>
        [HttpPost("/Login")]
        public async Task<dynamic> Login([FromBody] AccountLoginInput input)
        {
            var entity = await _accountRepository.FirstOrDefaultAsync(x => x.Account == input.account && x.DeleteMark == null);
            if (!MD5Encryption.Encrypt(input.password).Equals(entity.Password))
                throw JNPFException.Oh(ErrorCode.D1000);
            return entity.Adapt<TenantLoginOutput>();
        }
        #endregion

        #region PrivateMethod
        private async Task<int> Create(TenantEntity entity)
        {
            try
            {
                db.BeginTran();
                Task.Run(() => { CreateDB(entity.DbName); });
                var isOk= await _tenantRepository.Context.Insertable(entity).CallEntityMethod(m => m.Creator()).ExecuteCommandAsync();
                db.CommitTran();
                return isOk;
            }
            catch (Exception)
            {
                db.RollbackTran();
                return 0;
            }
        }

        // <summary>
        /// 创建租户数据库
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private async Task CreateDB(string dbName)
        {
            try
            {
                var dbinitName = App.Configuration["JNPF_DB_init:DBName"];
                var dbinitConnString = App.Configuration["JNPF_DB_init:ConnectionString"];
                var dbinitType = App.Configuration["JNPF_DB_init:ConnectionType"];
                db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = "7",
                    DbType = ToDbType(dbinitType),
                    ConnectionString = dbinitConnString,
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });

                var initDB = db.GetConnection("7");

                #region 创建数据库
                var CrDataBaseSql = "CREATE DATABASE " + dbName;
                await initDB.Ado.ExecuteCommandAsync(CrDataBaseSql);
                #endregion
                #region 初始化数据库
                var strSql = DBTableSql(dbinitType);
                var modelList = initDB.Ado.SqlQuery<DynamicDbTableModel>(strSql);
                var list = modelList.Adapt<List<DbTableModel>>();
                StringBuilder sb = new StringBuilder();
                if (dbinitType.ToLower().Equals("mysql"))
                {
                    foreach (var item in list)
                    {
                        sb.AppendFormat(@"create table {0}.{1} as select * from  {2};", dbName, item.table, item.table, dbinitName);
                        sb.AppendFormat(@"ALTER table {2}.{0} add PRIMARY KEY({1})", item.table, item.primaryKey,dbName);
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        sb.AppendFormat(@"select * into {0}.dbo.{1} from {2}.dbo.{3};", dbName, item.table, dbinitName, item.table);
                        sb.AppendFormat(@"ALTER TABLE {2}.[dbo].[{0}] ADD CONSTRAINT [PK__{0}] PRIMARY KEY CLUSTERED ([{1}])
                        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];", item.table, item.primaryKey,dbName);
                    }
                }
                await initDB.Ado.ExecuteCommandAsync(sb.ToString());
                #endregion
            }
            catch (Exception e)
            {

                throw;
            }
        }

        /// <summary>
        /// 删除租户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        private async Task<int> Delete(TenantEntity entity, int flag)
        {
            if (flag==1)
            {
               await DeleteDB(entity);
            }
            return await _tenantRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
        }

        /// <summary>
        /// 删除租户库
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task DeleteDB(TenantEntity entity)
        {
            var dbinitName = App.Configuration["JNPF_DB_init:DBName"];
            var dbinitConnString = App.Configuration["JNPF_DB_init:ConnectionString"];
            var dbinitType = App.Configuration["JNPF_DB_init:ConnectionType"];
            db.AddConnection(new ConnectionConfig()
            {
                ConfigId = "7",
                DbType =ToDbType(dbinitType),
                ConnectionString = dbinitConnString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });

            var initDB = db.GetConnection("7");
            var exists = (await initDB.Ado.ExecuteCommandAsync(string.Format("select count(*) from sys.databases where name='{0}'", entity.DbName))).ToInt();
            if (exists == 1)
            {
                await initDB.Ado.ExecuteCommandAsync(string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", entity.DbName));
                await initDB.Ado.ExecuteCommandAsync(string.Format("DROP DATABASE {0}", entity.DbName));
            }
        }

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private DbType ToDbType(string dbType)
        {
            switch (dbType)
            {
                case "SqlServer":
                    return DbType.SqlServer;
                case "MySql":
                    return DbType.MySql;
                case "Oracle":
                    return DbType.Oracle;
                case "DM":
                    return DbType.Dm;
                case "KingBase":
                    return DbType.Kdbndp;
                case "PostgreSql":
                    return DbType.PostgreSQL;
                default:
                    throw JNPFException.Oh(ErrorCode.D1505);
            }
        }

        /// <summary>
        /// 数据库表SQL
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private string DBTableSql(string dbType)
        {
            StringBuilder sb = new StringBuilder();
            switch (dbType)
            {
                case "SqlServer":
                    sb.Append(@"DECLARE @TABLEINFO TABLE ( NAME VARCHAR(50) , SUMROWS VARCHAR(11) , RESERVED VARCHAR(50) , DATA VARCHAR(50) , INDEX_SIZE VARCHAR(50) , UNUSED VARCHAR(50) , PK VARCHAR(50) ) DECLARE @TABLENAME TABLE ( NAME VARCHAR(50) ) DECLARE @NAME VARCHAR(50) DECLARE @PK VARCHAR(50) INSERT INTO @TABLENAME ( NAME ) SELECT O.NAME FROM SYSOBJECTS O , SYSINDEXES I WHERE O.ID = I.ID AND O.XTYPE = 'U' AND I.INDID < 2 ORDER BY I.ROWS DESC , O.NAME WHILE EXISTS ( SELECT 1 FROM @TABLENAME ) BEGIN SELECT TOP 1 @NAME = NAME FROM @TABLENAME DELETE @TABLENAME WHERE NAME = @NAME DECLARE @OBJECTID INT SET @OBJECTID = OBJECT_ID(@NAME) SELECT @PK = COL_NAME(@OBJECTID, COLID) FROM SYSOBJECTS AS O INNER JOIN SYSINDEXES AS I ON I.NAME = O.NAME INNER JOIN SYSINDEXKEYS AS K ON K.INDID = I.INDID WHERE O.XTYPE = 'PK' AND PARENT_OBJ = @OBJECTID AND K.ID = @OBJECTID INSERT INTO @TABLEINFO ( NAME , SUMROWS , RESERVED , DATA , INDEX_SIZE , UNUSED ) EXEC SYS.SP_SPACEUSED @NAME UPDATE @TABLEINFO SET PK = @PK WHERE NAME = @NAME END SELECT F.NAME AS F_TABLE,ISNULL(P.TDESCRIPTION,F.NAME) AS F_TABLENAME, F.RESERVED AS F_SIZE, RTRIM(F.SUMROWS) AS F_SUM, F.PK AS F_PRIMARYKEY FROM @TABLEINFO F LEFT JOIN ( SELECT NAME = CASE WHEN A.COLORDER = 1 THEN D.NAME ELSE '' END , TDESCRIPTION = CASE WHEN A.COLORDER = 1 THEN ISNULL(F.VALUE, '') ELSE '' END FROM SYSCOLUMNS A LEFT JOIN SYSTYPES B ON A.XUSERTYPE = B.XUSERTYPE INNER JOIN SYSOBJECTS D ON A.ID = D.ID AND D.XTYPE = 'U' AND D.NAME <> 'DTPROPERTIES' LEFT JOIN SYS.EXTENDED_PROPERTIES F ON D.ID = F.MAJOR_ID WHERE A.COLORDER = 1 AND F.MINOR_ID = 0 ) P ON F.NAME = P.NAME WHERE 1 = 1 ORDER BY F_TABLE");
                    break;
                case "Oracle":
                    sb.Append(@"SELECT DISTINCT COL.TABLE_NAME AS F_TABLE,TAB.COMMENTS AS F_TABLENAME,0 AS F_SIZE,NVL(T.NUM_ROWS,0)AS F_SUM,COLUMN_NAME AS F_PRIMARYKEY FROM USER_CONS_COLUMNS COL INNER JOIN USER_CONSTRAINTS CON ON CON.CONSTRAINT_NAME=COL.CONSTRAINT_NAME INNER JOIN USER_TAB_COMMENTS TAB ON TAB.TABLE_NAME=COL.TABLE_NAME INNER JOIN USER_TABLES T ON T.TABLE_NAME=COL.TABLE_NAME WHERE CON.CONSTRAINT_TYPE NOT IN('C','R')ORDER BY COL.TABLE_NAME");
                    break;
                case "MySql":
                    sb.Append(@"SELECT T1.*,(SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.`COLUMNS`WHERE TABLE_SCHEMA=DATABASE()AND TABLE_NAME=T1.F_TABLE AND COLUMN_KEY='PRI')F_PRIMARYKEY FROM(SELECT TABLE_NAME F_TABLE,0 F_SIZE,TABLE_ROWS F_SUM,(SELECT IF(LENGTH(TRIM(TABLE_COMMENT))<1,TABLE_NAME,TABLE_COMMENT))F_TABLENAME FROM INFORMATION_SCHEMA.`TABLES`WHERE TABLE_SCHEMA=DATABASE())T1 ORDER BY T1.F_TABLE");
                    break;
                default:
                    throw new Exception("不支持");
            }
            return sb.ToString();
        }
        #endregion
    }
}
