using System;
using general_shortener.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace general_shortener.Bootstraps
{
    /// <summary>
    /// Bootstrap class for mongodb
    /// </summary>
    public static class MongoDbBootstrap
    {
        /// <summary>
        /// Add mongodb bootstrap
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddMongoDbBootstrap(this IServiceCollection services, IConfiguration configuration)
        {
            MongoDbOptions options = new MongoDbOptions();
            configuration.GetSection(MongoDbOptions.Section).Bind(options);

            if (string.IsNullOrEmpty(options.ConnectionString))
                throw new Exception("No connection string given");
            
            if (string.IsNullOrEmpty(options.DatabaseName))
                throw new Exception("No database name given");
            
            MongoClient client = new MongoClient(options.ConnectionString);
            services.AddSingleton<IMongoClient, MongoClient>(s => client);
            services.AddSingleton<IMongoDatabase>(s => client.GetDatabase(options.DatabaseName));
        }
    }
}