using System;
using System.Collections.Generic;
using System.Linq;
using general_shortener.Models.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
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
            var pack = new ConventionPack();
            pack.Add(new UnderscoreCaseElementNameConvention());
            pack.Add(new EnumRepresentationConvention(BsonType.String));

            ConventionRegistry.Register(
                "Conventions",
                pack,
                t => true);
            
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
    
    /// <summary>
    /// String serializer class
    /// https://stackoverflow.com/questions/51856127/how-can-i-save-document-in-mongo-in-camel-case-using-c
    /// </summary>
    public class UnderscoreCaseElementNameConvention : ConventionBase, IMemberMapConvention
    {
        /// <summary>
        /// Function to apply convention
        /// </summary>
        /// <param name="memberMap"></param>
        public void Apply(BsonMemberMap memberMap)
        {
            string name = memberMap.MemberName;
            name = string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            memberMap.SetElementName(name);
        }
    }
}