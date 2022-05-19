using JNPF.System.Entitys.Dto.Permission.Department;
using JNPF.System.Entitys.Dto.Permission.Organize;
using JNPF.System.Entitys.Dto.Permission.OrganizeAdministrator;
using JNPF.System.Entitys.Dto.Permission.User;
using JNPF.System.Entitys.Model.Permission.Organize;
using JNPF.System.Entitys.Model.Permission.User;
using JNPF.System.Entitys.Permission;
using Mapster;

namespace JNPF.System.Entitys.Mapper
{
    public class PermissionMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<UserEntity, UserInfo>()
                .Map(dest => dest.userId, src => src.Id)
                .Map(dest => dest.userAccount, src => src.Account)
                .Map(dest => dest.userName, src => src.RealName)
                .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon)
                .Map(dest => dest.prevLoginTime, src => src.PrevLogTime)
                .Map(dest => dest.prevLoginIPAddress, src => src.PrevLogIP);
            config.ForType<UserEntity, UserInfoOutput>()
                 .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon);
            config.ForType<UserEntity, UserSelectorOutput>()
                .Map(dest => dest.fullName, src => src.RealName + "/" + src.Account)
                .Map(dest => dest.type, src => "user")
                .Map(dest => dest.parentId, src => src.OrganizeId);
            config.ForType<OrganizeEntity, UserSelectorOutput>()
                .Map(dest => dest.type, src => src.Category)
                .Map(dest => dest.icon, src => "icon-ym icon-ym-tree-organization3");
            config.ForType<OrganizeEntity, DepartmentSelectorOutput>()
                 .Map(dest => dest.type, src => src.Category);
            config.ForType<OrganizeAdminIsTratorCrInput, OrganizeAdministratorEntity>()
                .Ignore(dest => dest.UserId);
        }
    }
}
