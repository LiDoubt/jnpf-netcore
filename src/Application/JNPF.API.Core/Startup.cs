using JNPF.Common.Cache;
using JNPF.Common.Core.Filter;
using JNPF.Data.SqlSugar.Extensions;
using JNPF.JsonSerialization;
using JNPF.Message.Extensions;
using JNPF.TaskScheduler.Interfaces.TaskScheduler;
using JNPF.UnifyResult;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OnceMi.AspNetCore.OSS;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Serilog;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using Yitter.IdGenerator;

namespace JNPF.API.Core
{
    /// <summary>
    /// 
    /// </summary>
    [AppStartup(9)]
    public class Startup : AppStartup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            SqlSugarConfigure(services);

            services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);

            services.AddCorsAccessor();

            services.AddRemoteRequest();

            services.AddConfigurableOptions<CacheOptions>();

            services.AddControllersWithViews()
                    .AddMvcFilter<RequestActionFilter>()
                    .AddInjectWithUnifyResult<RESTfulResultProvider>()
                     //.AddJsonOptions(options =>
                     //{
                     //    //options.JsonSerializerOptions.Converters.AddDateFormatString("yyyy-MM-dd HH:mm:ss");
                     //    //格式化日期时间格式
                     //    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
                     //});
                     .AddNewtonsoftJson(options =>
                     {
                         //options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                         //默认命名规则
                         options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                         //设置时区为 UTC
                         options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                         //格式化json输出的日期格式
                         options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                         //格式化json输出的日期格式为时间戳
                         options.SerializerSettings.Converters.Add(new NewtonsoftDateTimeJsonConverter());
                         //空值处理
                         //options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                     });

            services.AddViewEngine();

            #region minio

            services.AddOSSService(option => 
            {
                option.Provider = OSSProvider.Minio;
                option.Endpoint = "192.168.0.60:9000";
                option.AccessKey = "minioadmin";
                option.SecretKey = "minioadmin";
                option.IsEnableHttps = false;
                option.IsEnableCache = true;
            });

            #endregion

            #region 微信
            services.AddSenparcGlobalServices(App.Configuration)//Senparc.CO2NET 全局注册
                        .AddSenparcWeixinServices(App.Configuration);//Senparc.Weixin 注册（如果使用Senparc.Weixin SDK则添加）
            services.AddSession();
            services.AddMemoryCache();//使用本地缓存必须添加
            services.AddSenparcWeixinServices(App.Configuration);
            #endregion

            services.AddEventBridge();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="senparcSetting"></param>
        /// <param name="senparcWeixinSetting"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //  NGINX 反向代理获取真实IP
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // 添加状态码拦截中间件
            app.UseUnifyResultStatusCodes();

            app.UseHttpsRedirection(); // 强制https
            app.UseStaticFiles();

            // Serilog请求日志中间件---必须在 UseStaticFiles 和 UseRouting 之间
            app.UseSerilogRequestLogging();

            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(20),
                ReceiveBufferSize = 4 * 1024
            });
            app.UseMiddleware<WebSocketHandlerMiddleware>();

            app.UseRouting();

            app.UseCorsAccessor();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            // 设置雪花id的workerId，确保每个实例workerId都应不同
            var workerId = ushort.Parse(App.Configuration["SnowId:WorkerId"] ?? "1");
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions { WorkerId = workerId, WorkerIdBitLength = 16 });

            #region 微信

            IRegisterService register = RegisterService.Start(senparcSetting.Value).UseSenparcGlobal();//启动 CO2NET 全局注册，必须！
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);//微信全局注册,必须！

            #endregion

            // 开启自启动定时任务
            App.GetService<ITimeTaskService>().StartTimerJob();
        }

        /// <summary>
        /// 配置SqlSugar
        /// </summary>
        /// <param name="services"></param>
        private void SqlSugarConfigure(IServiceCollection services)
        {
            #region 配置sqlsuagr

            List<ConnectionConfig> connectConfigList = new List<ConnectionConfig>();
            var connectionStr = $"{App.Configuration["ConnectionStrings:DefaultConnection"]}";
            var dataBase = $"{App.Configuration["ConnectionStrings:DBName"]}";
            var dbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]);
            var ConfigId = $"{App.Configuration["ConnectionStrings:ConfigId"]}";
            var iocDbType = (IocDbType)Enum.Parse(typeof(IocDbType), App.Configuration["ConnectionStrings:DBType"]);

            SugarIocServices.AddSqlSugar(new IocConfig()
            {
                ConnectionString = string.Format(connectionStr, dataBase),
                DbType = iocDbType,
                ConfigId= ConfigId,
                IsAutoCloseConnection = true//自动释放
            });

            connectConfigList.Add(new ConnectionConfig
            {
                ConnectionString = string.Format(connectionStr, dataBase),
                DbType = dbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConfigId = ConfigId,
                ConfigureExternalServices = new ConfigureExternalServicesExtenisons()
                {
                    EntityNameServiceType = typeof(SugarTable)//这个不管是不是自定义都要写，主要是用来获取所有实体
                }
            });

            services.AddSqlSugar(connectConfigList.ToArray(), db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    if (sql.StartsWith("SELECT"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    if (sql.StartsWith("UPDATE") || sql.StartsWith("INSERT"))
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    if (sql.StartsWith("DELETE"))
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                    }
                    //在控制台输出sql语句
                    Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                    //App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
                };
            });

            #endregion
        }
    }
}
