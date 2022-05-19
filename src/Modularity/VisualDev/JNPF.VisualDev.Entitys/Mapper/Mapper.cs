using JNPF.VisualDev.Entitys.Dto.VisualDev;
using Mapster;

namespace JNPF.VisualDev.Entitys.Mapper
{
    public class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<VisualDevEntity, VisualDevSelectorOutput>()
                .Map(dest => dest.parentId, src => src.Category);
        }
    }
}
