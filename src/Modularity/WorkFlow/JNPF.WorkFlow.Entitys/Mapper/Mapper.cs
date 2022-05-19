using JNPF.Common.Extension;
using JNPF.WorkFlow.Entitys.Dto.FlowBefore;
using JNPF.WorkFlow.Entitys.Dto.FlowEngine;
using JNPF.WorkFlow.Entitys.Dto.FlowLaunch;
using JNPF.WorkFlow.Entitys.Model;
using Mapster;

namespace JNPF.WorkFlow.Entitys.Mapper
{
    class Mapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<FlowEngineEntity, FlowEngineListAllOutput>()
                .Map(dest => dest.formData, src => src.FormTemplateJson);
            config.ForType<FlowEngineEntity, FlowEngineInfoOutput>()
                .Map(dest => dest.formData, src => src.FormTemplateJson)
                .Map(dest => dest.dbLinkId, src => src.DbLinkId.IsEmpty() ? "0" : src.DbLinkId);
            config.ForType<FlowEngineCrInput, FlowEngineEntity>()
                .Map(dest => dest.FormTemplateJson, src => src.formData);
            config.ForType<FlowEngineUpInput, FlowEngineEntity>()
                .Map(dest => dest.FormTemplateJson, src => src.formData);
            config.ForType<FlowEngineEntity, FlowLaunchListOutput>()
               .Map(dest => dest.formData, src => src.FormTemplateJson);
            config.ForType<FlowEngineEntity, FlowBeforeListOutput>()
                .Map(dest => dest.formData, src => src.FlowTemplateJson);
            config.ForType<FlowTemplateJsonModel, TaskNodeModel>()
                .Map(dest => dest.upNodeId, src => src.prevId);
        }
    }
}
