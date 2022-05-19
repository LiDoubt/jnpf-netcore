using JNPF.Common.Cache;
using JNPF.Data.SqlSugar.Extensions;
using JNPF.JsonSerialization;
using JNPF.UnifyResult;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SqlSugar;
using System;
using System.Collections.Generic;
using Yitter.IdGenerator;

namespace JNPF.API.Tenant
{
    public class Startup 
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 配置sqlsuagr

            List<ConnectionConfig> connectConfigList = new List<ConnectionConfig>();
            var connectionStr = $"{App.Configuration["ConnectionStrings:DefaultConnection"]}";
            var dataBase = $"{App.Configuration["ConnectionStrings:DBName"]}";
            var dbType = (DbType)Enum.Parse(typeof(DbType), App.Configuration["ConnectionStrings:DBType"]);
            var ConfigId = $"{App.Configuration["ConnectionStrings:ConfigId"]}";

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
                    //Console.WriteLine(SqlProfiler.ParameterFormat(sql, pars));
                    App.PrintToMiniProfiler("SqlSugar", "Info", SqlProfiler.ParameterFormat(sql, pars));
                };
            });

            #endregion

            services.AddCorsAccessor();

            services.AddRemoteRequest();

            services.AddConfigurableOptions<CacheOptions>();

            services.AddControllersWithViews()
                    .AddInjectWithUnifyResult<RESTfulResultProvider>()
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

            services.AddEventBridge();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            // 添加状态码拦截中间件
            app.UseUnifyResultStatusCodes();

            app.UseHttpsRedirection(); // 强制https
            app.UseStaticFiles();

            // Serilog请求日志中间件---必须在 UseStaticFiles 和 UseRouting 之间
            //app.UseSerilogRequestLogging();

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

        }
    }
}
