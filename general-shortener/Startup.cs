using System.Text.Json.Serialization;
using general_shortener.Bootstraps;
using general_shortener.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

#pragma warning disable 1591

namespace general_shortener
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;


        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddSwaggerBootstrap(_env);
            services.AddAuthenticationBootstrap();
            services.AddOptionsBindingBootstrap(this.Configuration);
            services.AddMongoDbBootstrap(this.Configuration);

            services.AddSingleton<IDirectoryService, DirectoryService>();
            services.AddSingleton<ITransferService, TransferService>();
            
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
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                
            }

            app.ApplicationServices.GetService<ITransferService>();
            
            // Documentation
            app.UseSwagger();
            app.UseReDoc(c =>
            {
                c.DocumentTitle = "general-shortener";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.RoutePrefix = "docs";
            });

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}