using JNPF.Dependency;
using System.Collections.Generic;

namespace JNPF.WorkFlow.Entitys.Model
{
    [SuppressSniffer]
    public class FlowEngineImportModel
    {
        public FlowEngineEntity flowEngine { get; set; }

        public List<FlowEngineVisibleEntity> visibleList { get; set; } = new List<FlowEngineVisibleEntity>();
    }
}
