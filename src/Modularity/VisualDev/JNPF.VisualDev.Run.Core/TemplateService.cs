using JNPF.Common.Configuration;
using JNPF.Common.Helper;
using JNPF.Dependency;
using JNPF.ViewEngine;
using JNPF.VisualDev.Entitys;
using JNPF.VisualDev.Entitys.Model.VisualDevModelData;
using JNPF.VisualDev.Run.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace JNPF.VisualDev.Run.Core
{
    /// <summary>
    /// 在线开发运行服务
    /// </summary>
    public class TemplateService : ITemplateService, ITransient
    {
        private readonly IRunService _runService;
       

        /// <summary>
        /// 初始化一个<see cref="TemplateService"/>类型的新实例
        /// </summary>
        public TemplateService(IRunService runService)
        {
            _runService = runService;
            //
        }

        


        
    }
}
