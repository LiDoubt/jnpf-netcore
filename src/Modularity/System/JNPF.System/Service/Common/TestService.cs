using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.System.Entitys.Permission;
using JNPF.System.Interfaces.Common;
using JNPF.System.Interfaces.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minio;
using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.System.Service.Common
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [ApiDescriptionSettings(Name = "Test", Order = 306)]
    [Route("api/[controller]")]
    public class TestService : IDynamicApiController, ITransient
    {
        private readonly SqlSugarScope db;
        private readonly ISqlSugarRepository<UserEntity> _sqlSugarRepository;
        public TestService(ISqlSugarRepository<UserEntity> sqlSugarRepository)
        {
            _sqlSugarRepository = sqlSugarRepository;
            db = sqlSugarRepository.Context;
        }

        [HttpGet("")]
        [AllowAnonymous]
        public async Task<dynamic> test1()
        {
            try
            {
                //var str=@'
                UserEntity aa = new UserEntity() { Id = "321321" };
                UserEntity aa1 = new UserEntity() { Id = "3213214" };
                var list = new List<UserEntity>();
                list.Add(aa);
                list.Add(aa1);
                _sqlSugarRepository.Insert(list);
                var xx = App.Assemblies;
                var xxx = App.EffectiveTypes;
                return GetCreateTableSql();
            }
            catch (global::System.Exception)
            {

                throw;
            }
        }


        private void create() {
            UserEntity aa = new UserEntity() { Id = "321321" };
            _sqlSugarRepository.Insert(aa);
        }

        /// <summary>
        /// 创建表脚本勿删
        /// </summary>
        /// <returns></returns>
        private string GetCreateTableSql()
        {
            db.AddConnection(new ConnectionConfig()
            {
                ConfigId = "777",
                DbType = DbType.SqlServer,
                ConnectionString = "Data Source=192.168.0.11;Initial Catalog=netcore_init;User ID=netcore_test;Password=EtMhNTLJsGTeZEaD;MultipleActiveResultSets=true",
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.ChangeDatabase("777");
            var sb = new StringBuilder();
            var tables = db.DbMaintenance.GetTableInfoList(false).OrderBy(x=>x.Name).ToList();
            foreach (var item in tables)
            {
                sb.AppendFormat("insert into {0} select * from {1}.dbo.{0};", item.Name, "${dbName}").AppendLine();
            }
            var sql= sb.ToString();
            return sql;
        }
    }
}
