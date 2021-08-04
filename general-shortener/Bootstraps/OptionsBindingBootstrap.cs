using general_shortener.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace general_shortener.Bootstraps
{
    /// <summary>
    /// Options configuration bootstrap class
    /// </summary>
    public static class OptionsBindingBootstrap
    {
        /// <summary>
        /// Add Options configuration to the service init
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddOptionsBindingBootstrap(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbOptions>(configuration.GetSection(MongoDbOptions.Section));
            services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.Section));
        }
    }
}