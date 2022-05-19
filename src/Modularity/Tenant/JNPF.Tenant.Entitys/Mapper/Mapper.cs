using JNPF.Tenant.Entitys.Dto;
using JNPF.Tenant.Entitys.Entity;
using JNPF.Tenant.Entitys.Model;
using Mapster;

namespace JNPF.Tenant.Entitys.Mapper
{
    class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DynamicDbTableModel, DbTableModel>()
                .Map(dest => dest.table, src => src.F_TABLE)
                .Map(dest => dest.tableName, src => src.F_TABLENAME)
                .Map(dest => dest.size, src => src.F_SIZE)
                .Map(dest => dest.sum, src => int.Parse(src.F_SUM))
                .Map(dest => dest.primaryKey, src => src.F_PRIMARYKEY);

            config.ForType<AccountEntity,TenantLoginOutput>()
                .Map(dest => dest.token, src => src.Id);
        }
    }
}
