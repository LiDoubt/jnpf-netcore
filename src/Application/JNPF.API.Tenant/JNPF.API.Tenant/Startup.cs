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
            #region ����sqlsuagr

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
                    EntityNameServiceType = typeof(SugarTable)//��������ǲ����Զ��嶼Ҫд����Ҫ��������ȡ����ʵ��
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
                    //�ڿ���̨���sql���
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
                         //Ĭ����������
                         options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                         //����ʱ��Ϊ UTC
                         options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                         //��ʽ��json��������ڸ�ʽ
                         options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                         //��ʽ��json��������ڸ�ʽΪʱ���
                         options.SerializerSettings.Converters.Add(new NewtonsoftDateTimeJsonConverter());
                         //��ֵ����
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

            // ���״̬�������м��
            app.UseUnifyResultStatusCodes();

            app.UseHttpsRedirection(); // ǿ��https
            app.UseStaticFiles();

            // Serilog������־�м��---������ UseStaticFiles �� UseRouting ֮��
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

            // ����ѩ��id��workerId��ȷ��ÿ��ʵ��workerId��Ӧ��ͬ
            var workerId = ushort.Parse(App.Configuration["SnowId:WorkerId"] ?? "1");
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions { WorkerId = workerId, WorkerIdBitLength = 16 });

        }
    }
}
