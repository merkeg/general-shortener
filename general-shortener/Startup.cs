using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
#pragma warning disable 1591

namespace general_shortener
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
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
            });

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });

            var xmlCommentPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            var documentationPath = Path.Combine(AppContext.BaseDirectory, "Resources\\documentation.md");
            var documentation = File.ReadAllText(documentationPath);
            
            services.AddSwaggerGen(c => {
                
                c.IncludeXmlComments(xmlCommentPath);
                c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "general-shortener",
                Version = "v1",
                Description = documentation,
                Contact = new OpenApiContact()
                {
                    Name = "Egor Merk",
                    Email = "contact@merkeg.de",
                    Url = new Uri("https://twitter.com/merkegor")
                }
            });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }
            
            // Documentation
            app.UseSwagger();
            app.UseReDoc(c =>
            {
                c.DocumentTitle = "general-shortener";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.RoutePrefix = "docs";
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}