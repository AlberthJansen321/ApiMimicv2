using ApiMimicv2.DataBase;
using ApiMimicv2.V1.Repositories;
using ApiMimicv2.V1.Repositories.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ApiMimicv2.Herlpers;
using Microsoft.OpenApi.Models;
using ApiMimicv2.Herlpers.Swagger;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ApiMimicv2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {

            #region AutoMapper-Config

            var config = new MapperConfiguration(cfg => cfg.AddProfile(new DTOMapperProfile()));
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            #endregion


            services.AddDbContext<MimicContext>(opt =>
            {

                opt.UseSqlite("Data Source=Database\\Mimic.db");

            });

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddScoped<IPalavraRepository, PalavraRepository>();

            services.AddApiVersioning(cfg =>
            {
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.ReportApiVersions = true;
               // cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.ResolveConflictingActions( conflito => conflito.First());
                cfg.SwaggerDoc("v1.0", new OpenApiInfo()
                {
                    Title = "MimicAPI - V1.0",
                    Version = "V1.0"
                });
                cfg.SwaggerDoc("v1.1", new OpenApiInfo()
                {
                    Title = "MimicAPI - V1.1",
                    Version = "V1.1"
                });
                cfg.SwaggerDoc("v2.0", new OpenApiInfo()
                {
                    Title = "MimicAPI - V2.0",
                    Version = "V2.0"
                });

                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var NomeArquivo = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var ArquivodeComentario = Path.Combine(CaminhoProjeto,NomeArquivo);
                cfg.IncludeXmlComments(ArquivodeComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                    // would mean this action is unversioned and should be included everywhere
                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });
                
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);

        }
#pragma warning disable CS1591 // O comentário XML ausente não foi encontrado para o tipo ou membro visível publicamente
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CS1591 // O comentário XML ausente não foi encontrado para o tipo ou membro visível publicamente
        {
          
         
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(cfg => {

                    cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "MimicAPI - V2.0");
                    cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "MimicAPI - V1.1");
                    cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MimicAPI - V1.0");
                    cfg.RoutePrefix = string.Empty;

                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApiVersioning();

         //   app.UseAuthorization();

            app.UseRouting();

            app.UseStatusCodePages();

            app.UseMvc();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
