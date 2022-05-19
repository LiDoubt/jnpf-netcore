using JNPF.Common.Filter;
using JNPF.System.Entitys.Model.System.DataBase;
using JNPF.System.Entitys.System;
using JNPF.VisualDev.Entitys.Dto.VisualDevModelData;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using SqlSugar;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace JNPF.System.Interfaces.System
{
    /// <summary>
    /// 数据管理
    /// 版 本：V3.2
    /// 版 权：引迈信息技术有限公司（https://www.jnpfsoft.com）
    /// 作 者：JNPF开发平台组
    /// 日 期：2021-06-01 
    /// </summary>
    public interface IDataBaseService
    {
        /// <summary>
        /// 表列表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <returns></returns>
        List<DbTableModel> GetList(DbLinkEntity link);

        /// <summary>
        /// 表字段
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        List<DbTableFieldModel> GetFieldList(DbLinkEntity link, string table);

        /// <summary>
        /// 表字段
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        List<DbTableFieldModel> GetFieldListByNoAsync(DbLinkEntity link, string table);

        /// <summary>
        /// 表数据
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        DataTable GetData(DbLinkEntity link, string table);

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        Task<int> ExecuteSql(DbLinkEntity link, string strSql);

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="table">表名</param>
        bool Delete(DbLinkEntity link, string table);

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        Task<bool> Create(DbLinkEntity link, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList);

        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="link">数据连接</param>
        /// <param name="oldTable">旧表</param>
        /// <param name="tableModel">表对象</param>
        /// <param name="tableFieldList">字段对象</param>
        Task<bool> Update(DbLinkEntity link, string oldTable, DbTableModel tableModel, List<DbTableFieldModel> tableFieldList);

        /// <summary>
        /// 根据链接获取数据
        /// </summary>
        /// <returns></returns>
        DataTable GetInterFaceData(DbLinkEntity link, string strSql,params SugarParameter[] parameters);

        /// <summary>
        /// 根据链接获取分页数据
        /// </summary>
        /// <returns></returns>
        PageResult<Dictionary<string, object>> GetInterFaceData(DbLinkEntity link, string strSql, VisualDevModelListQueryInput pageInput, ColumnDesignModel columnDesign, string menuId);

        /// <summary>
        /// 验证数据库连接
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        bool IsConnection(DbLinkEntity link);

        /// <summary>
        /// 同步数据
        /// </summary>
        /// <param name="link"></param>
        /// <param name="dt"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        bool SyncData(DbLinkEntity link, DataTable dt, string table);

        /// <summary>
        /// 数据库表SQL
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        string DBTableSql(string dbType);

        /// <summary>
        /// 转换数据库类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        SqlSugar.DbType ToDbType(string dbType);

        /// <summary>
        /// 转换连接字符串
        /// </summary>
        /// <param name="dbLinkEntity"></param>
        /// <returns></returns>
        string ToConnectionString(DbLinkEntity dbLinkEntity);

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="link"></param>
        /// <param name="table"></param>
        /// <returns></returns>
        bool IsAnyTable(DbLinkEntity link, string table);

        /// <summary>
        /// 同步表操作
        /// </summary>
        /// <param name="linkFrom"></param>
        /// <param name="linkTo"></param>
        /// <param name="table"></param>
        /// <param name="type"></param>
        void SyncTable(DbLinkEntity linkFrom, DbLinkEntity linkTo, string table, int type);

        /// <summary>
        /// 添加链接
        /// </summary>
        /// <param name="link"></param>
        void AddConnection(DbLinkEntity link);

        /// <summary>
        /// 根据链接获取数据(打印模板)
        /// </summary>
        /// <returns></returns>
        DataTable GetPrintDevData(DbLinkEntity link, string strSql, List<SugarParameter> sugarParameters = null);
    }
}
