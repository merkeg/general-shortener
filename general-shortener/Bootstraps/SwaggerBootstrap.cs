using System;
using System.IO;
using System.Reflection;
using general_shortener.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace general_shortener.Bootstraps
{
    /// <summary>
    /// Bootstrapper class for swagger
    /// </summary>
    public static class SwaggerBootstrap
    {
        /// <summary>
        /// Add Swagger configuration to the service init
        /// </summary>
        /// <param name="services"></param>
        public static void AddSwaggerBootstrap(this IServiceCollection services)
        {
            var xmlCommentPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            var documentationPath = Path.Combine(AppContext.BaseDirectory, "Resources\\documentation.md");
            var documentation = File.ReadAllText(documentationPath);
            
            services.AddSwaggerGen(c =>
            {
                
                
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
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Api key that is using the Bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.OperationFilter<AuthOperationFilter>();
                
            });
        }
    }
}