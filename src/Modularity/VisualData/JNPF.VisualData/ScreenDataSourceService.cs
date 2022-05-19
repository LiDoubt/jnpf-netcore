using JNPF.Common.Enum;
using JNPF.Dependency;
using JNPF.DynamicApiController;
using JNPF.FriendlyException;
using JNPF.VisualData.Entity;
using JNPF.VisualData.Entitys.Dto.ScreenDataSource;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JNPF.VisualData
{
    /// <summary>
    /// 业务实现：大屏
    /// </summary>
    [ApiDescriptionSettings(Tag = "BladeVisual", Name = "db", Order = 160)]
    [Route("api/blade-visual/[controller]")]
    public class ScreenDataSourceService : IDynamicApiController, ITransient
    {
        private readonly ISqlSugarRepository<VisualDBEntity> _visualDBRepository;

        /// <summary>
        /// 初始化一个<see cref="ScreenDataSourceService"/>类型的新实例
        /// </summary>
        public ScreenDataSourceService(ISqlSugarRepository<VisualDBEntity> visualDBRepository)
        {
            _visualDBRepository = visualDBRepository;
        }

        #region Get

        /// <summary>
        /// 分页
        /// </summary>
        /// <returns></returns>
        [HttpGet("list")]
        public async Task<dynamic> GetList([FromQuery] ScreenDataSourceListQueryInput input)
        {
            var data = await _visualDBRepository.Entities.Select(v => new { id = v.Id, name = v.Name, driverClass = v.DriverClass, password = v.Password, remark = v.Remark, url = v.Url, username = v.UserName, isDeleted = v.IsDeleted }).MergeTable().Select<ScreenDataSourceListOutput>().Where(v => v.isDeleted.Equals(0)).ToPagedListAsync(input.current, input.size);
            return new { current = data.pagination.PageIndex, pages = data.pagination.Total / data.pagination.PageSize, records = data.list, size = data.pagination.PageSize, total = data.pagination.Total };
        }

        /// <summary>
        /// 详情
        /// </summary>
        /// <returns></returns>
        [HttpGet("detail")]
        public async Task<dynamic> GetInfo(string id)
        {
            var entity = await _visualDBRepository.SingleAsync(v => v.Id == id && v.IsDeleted == "0");
            var data = entity.Adapt<ScreenDataSourceInfoOutput>();
            return data;
        }

        /// <summary>
        /// 下拉数据源
        /// </summary>
        /// <returns></returns>
        [HttpGet("db-list")]
        public async Task<dynamic> GetDBList()
        {
            var data = await _visualDBRepository.Entities.Select(v => new { id = v.Id, name = v.Name, driverClass = v.DriverClass }).MergeTable().Select<ScreenDataSourceSeleectOutput>().ToListAsync();
            return data;
        }

        #endregion

        #region Post

        /// <summary>
        /// 新增与修改
        /// </summary>
        /// <returns></returns>
        [HttpPost("submit")]
        public async Task Submit([FromBody] ScreenDataSourceUpInput input)
        {
            var entity = input.Adapt<VisualDBEntity>();
            var isOk = 0;
            if (input.id == null)
                isOk = await _visualDBRepository.Context.Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
            else
                isOk = await _visualDBRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("save")]
        public async Task Create([FromBody] ScreenDataSourceCrInput input)
        {
            var entity = input.Adapt<VisualDBEntity>();
            var isOk = await _visualDBRepository.Context.Insertable(entity).IgnoreColumns(ignoreNullColumn: true).CallEntityMethod(m => m.Create()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1000);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("update")]
        public async Task Update([FromBody] ScreenDataSourceUpInput input)
        {
            var entity = input.Adapt<VisualDBEntity>();
            var isOk = await _visualDBRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.LastModify()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1001);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task Delete(string ids)
        {
            var entity = await _visualDBRepository.SingleAsync(v => v.Id == ids && v.IsDeleted == "0");
            _ = entity ?? throw JNPFException.Oh(ErrorCode.COM1005);
            var isOk = await _visualDBRepository.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).CallEntityMethod(m => m.Delete()).ExecuteCommandAsync();
            if (!(isOk > 0)) throw JNPFException.Oh(ErrorCode.COM1002);
        }

        /// <summary>
        /// 数据源测试连接
        /// </summary>
        /// <returns></returns>
        [HttpPost("db-test")]
        public dynamic Test([FromBody] ScreenDataSourceUpInput input)
        {
            try
            {
                var db = _visualDBRepository.Context;
                db.AddConnection(new ConnectionConfig()
                {
                    ConfigId = input.id,
                    DbType = ToDbTytpe(input.driverClass),
                    ConnectionString = ToConnectionString(input.driverClass, input.url, input.name, input.password),
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true
                });
                db.ChangeDatabase(input.id);


                db.Open();
                db.Close();
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                throw JNPFException.Oh(ErrorCode.D1507);
            }
        }

        /// <summary>
        /// 动态执行SQL
        /// </summary>
        /// <returns></returns>
        [HttpPost("dynamic-query")]
        public async Task<dynamic> Query([FromBody] ScreenDataSourceDynamicQueryInput input)
        {
            var entity = await _visualDBRepository.SingleAsync(v => v.Id == input.id && v.IsDeleted == "0");
            var db = _visualDBRepository.Context;
            db.AddConnection(new ConnectionConfig()
            {
                ConfigId = input.id,
                DbType = ToDbTytpe(entity.DriverClass),
                ConnectionString = ToConnectionString(entity.DriverClass, entity.Url, entity.UserName, entity.Password),
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.ChangeDatabase(input.id);
            var table = await db.Ado.GetDataTableAsync(input.sql);
            return table;
        }

        #endregion


        #region PrivateMethod

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        private SqlSugar.DbType ToDbTytpe(string dbType)
        {
            switch (dbType)
            {
                case "org.postgresql.Driver":
                    return SqlSugar.DbType.PostgreSQL;
                case "com.mysql.cj.jdbc.Driver":
                    return SqlSugar.DbType.MySql;
                case "oracle.jdbc.OracleDriver":
                    return SqlSugar.DbType.Oracle;
                case "com.microsoft.sqlserver.jdbc.SQLServerDriver":
                    return SqlSugar.DbType.SqlServer;
                default:
                    throw JNPFException.Oh(ErrorCode.D1505);
            }
        }

        /// <summary>
        /// 转换连接字符串
        /// </summary>
        /// <returns></returns>
        private string ToConnectionString(string driverClass, string url, string name, string password)
        {
            switch (driverClass)
            {
                case "org.postgresql.Driver":
                    return string.Format(url, name, password);
                case "com.mysql.cj.jdbc.Driver":
                    return string.Format(url, name, password);
                case "oracle.jdbc.OracleDriver":
                    return string.Format(url, name, password);
                case "com.microsoft.sqlserver.jdbc.SQLServerDriver":
                    return string.Format(url, name, password);
                default:
                    throw JNPFException.Oh(ErrorCode.D1505);
            }
        }

        #endregion
    }
}
