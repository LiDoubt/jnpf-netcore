using JNPF.System.Entitys.Dto.System.Database;
using JNPF.System.Entitys.Dto.System.DbBackup;
using JNPF.System.Entitys.Dto.System.Province;
using JNPF.System.Entitys.Model.System.DataBase;
using JNPF.System.Entitys.System;
using Mapster;
using SqlSugar;

namespace JNPF.System.Entitys.Mapper
{
    public class SystemMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DbBackupEntity, DbBackupListOutput>()
                .Map(dest => dest.fileUrl, src => src.FilePath);
            config.ForType<DbColumnInfo, DbTableFieldModel>()
                .Map(dest => dest.field, src => src.DbColumnName)
                .Map(dest => dest.fieldName, src => src.ColumnDescription)
                .Map(dest => dest.dataLength, src => src.Length.ToString())
                .Map(dest => dest.identity, src => src.IsIdentity ? "1" : "")
                .Map(dest => dest.primaryKey, src => src.IsPrimarykey ? 1 : 0)
                .Map(dest => dest.allowNull, src => src.IsNullable ? 1 : 0)
                .Map(dest => dest.defaults, src => src.DefaultValue);
            config.ForType<DbTableFieldModel, DbColumnInfo>()
                .Map(dest => dest.DbColumnName, src => src.field)
                .Map(dest => dest.ColumnDescription, src => src.fieldName)
                .Map(dest => dest.Length, src => int.Parse(src.dataLength))
                .Map(dest => dest.IsIdentity, src => src.identity == "1")
                .Map(dest => dest.IsPrimarykey, src => src.primaryKey == 1)
                .Map(dest => dest.IsNullable, src => src.allowNull == 1)
                .Map(dest => dest.DefaultValue, src => src.defaults);
            config.ForType<DynamicDbTableModel, DbTableModel>()
                .Map(dest => dest.table, src => src.F_TABLE)
                .Map(dest => dest.tableName, src => src.F_TABLENAME)
                .Map(dest => dest.size, src => src.F_SIZE)
                .Map(dest => dest.sum, src => int.Parse(src.F_SUM))
                .Map(dest => dest.primaryKey, src => src.F_PRIMARYKEY);
            config.ForType<DbTableInfo, DatabaseTableListOutput>()
                .Map(dest => dest.table, src => src.Name)
                .Map(dest => dest.tableName, src => src.Description);
            config.ForType<DbTableInfo, DbTableModel>()
                .Map(dest => dest.table, src => src.Name)
                .Map(dest => dest.tableName, src => src.Description);
            config.ForType<DbTableInfo, TableInfo>()
                .Map(dest => dest.table, src => src.Name)
                .Map(dest => dest.tableName, src => src.Description);
            config.ForType<DbColumnInfo, TableFieldList>()
                .Map(dest => dest.field, src => src.DbColumnName)
                .Map(dest => dest.fieldName, src => src.ColumnDescription)
                .Map(dest => dest.dataLength, src => src.Length.ToString())
                .Map(dest => dest.primaryKey, src => src.IsPrimarykey ? 1 : 0)
                .Map(dest => dest.allowNull, src => src.IsNullable ? 1 : 0);
        }
    }
}
