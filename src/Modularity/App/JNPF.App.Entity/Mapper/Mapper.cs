using JNPF.Apps.Entitys.Dto;
using JNPF.System.Entitys.Permission;
using Mapster;

namespace JNPF.Apps.Entitys.Mapper
{
    class Mapper: IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<UserEntity, AppUserOutput>()
                .Map(dest => dest.userId, src => src.Id)
                .Map(dest => dest.userAccount, src => src.Account)
                .Map(dest => dest.userName, src => src.RealName)
                .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon);
            config.ForType<UserEntity, AppUserInfoOutput>()
                .Map(dest => dest.headIcon, src => "/api/File/Image/userAvatar/" + src.HeadIcon);
        }
    }
}
