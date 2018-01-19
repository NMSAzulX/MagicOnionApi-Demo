using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using MagicOnion;
using MagicOnion.HttpGateway.Swagger;
using MagicOnion.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;

namespace MagicApiTest
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
            //这代码跟咱们之前定义服务的那个代码一个样子
            var service = MagicOnionEngine.BuildServerServiceDefinition(new MagicOnionOptions(true)
            {
                MagicOnionLogger = new MagicOnionLogToGrpcLogger()
            });
            var server = new Server
            {
                Services = { service },
                Ports = { new ServerPort("localhost", 8800, ServerCredentials.Insecure) }
            };

            //这里开始不同，你要把注释生成到xml里给swagger,这里是swagger的用法，看不懂去学swagger
            services.AddSwaggerGen(c =>
            {
                var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Swagger.xml");
                c.IncludeXmlComments(filePath);
            });

            server.Start();

            //这里添加服务
            services.Add(new ServiceDescriptor(typeof(MagicOnionServiceDefinition), service));
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //获取添加了的服务
            var magicOnion = app.ApplicationServices.GetService<MagicOnionServiceDefinition>();
            //使用MagicOnion的Swagger扩展，就是让你的rpc接口也能在swagger页面上显示
            //下面这些东西你可能乍一看就懵逼，但你看到页面的时候就会发现，一个萝卜一个坑。
            //注意：swagger原生用法属性都是大写的，这里是小写。
            app.UseMagicOnionSwagger(magicOnion.MethodHandlers, new SwaggerOptions("MagicOnion.Server", "Swagger Integration Test", "/")
            {
                
                 Info = new Info()
                 {
                     title = "MGrpc",
                     version = "v1",
                     description = "This is the API-Interface for MGrpc",
                     termsOfService = "By NMS",
                     contact = new Contact
                     {

                         name = "LanX",
                         email = "2765968624@qq.com"
                     }
                 },
                //使用Swagger生成的xml，就是你接口的注释
                XmlDocumentPath = PlatformServices.Default.Application.ApplicationBasePath + "Swagger.xml"
            });
            //要想让rpc成为该web服务的接口，流量和协议被统一到你写的这个web项目中来，那么就要用个方法链接你和rpc
            //这个web项目承接你的请求，然后自己去调用rpc获取结果，再返回给你。
            //因此需要下面这句话
            app.UseMagicOnionHttpGateway(magicOnion.MethodHandlers, new Channel("localhost:8800", ChannelCredentials.Insecure));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //以下是swagger的用法，不赘述
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API-v1");
                c.ShowJsonEditor();
                c.ShowRequestHeaders();
            });
            app.UseSwagger();
            app.UseMvc();
        }
    }
}
