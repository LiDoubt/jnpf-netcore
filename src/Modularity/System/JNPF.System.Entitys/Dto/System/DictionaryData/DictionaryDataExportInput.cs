using JNPF.Dependency;
using JNPF.System.Entitys.Entity.System;
using JNPF.System.Entitys.System;
using System.Collections.Generic;

namespace JNPF.System.Entitys.Dto.System.DictionaryData
{
    [SuppressSniffer]
    public class DictionaryDataExportInput
    {
        public List<DictionaryTypeEntity> list { get; set; } = new List<DictionaryTypeEntity>();
        public List<DictionaryDataEntity> modelList { get; set; } = new List<DictionaryDataEntity>();
    }
}
