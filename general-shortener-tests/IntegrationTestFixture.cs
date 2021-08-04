using System;
using System.Net.Http;
using System.Net.Http.Headers;
using general_shortener;
using general_shortener.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Mongo2Go;
using MongoDB.Driver;

namespace general_shortener_tests
{
    public class IntegrationTestFixture : WebApplicationFactory<Startup>, IDisposable
    {
        
        internal readonly MongoDbRunner _mongoRunner;
        internal readonly HttpClient TestClient;

        public IntegrationTestFixture()
        {
            this._mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);
            this.TestClient = this.CreateClient();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            

            builder.ConfigureAppConfiguration(config =>
            {
                config.AddConfiguration(new ConfigurationBuilder().AddJsonFile("integrationsettings.json").Build());
            });
            
            builder.ConfigureTestServices(services =>
            {
                
                // TODO: VERY DIRTY IMPLEMENTATION, BUT I AM TOO STUPID TO KNOW ANOTHER WAY

                services.RemoveAll(typeof(IMongoClient));
                services.RemoveAll(typeof(IMongoDatabase));
                
                MongoClient client = new MongoClient(this._mongoRunner.ConnectionString);
                services.AddSingleton<IMongoClient, MongoClient>(s => client);
                services.AddSingleton<IMongoDatabase>(s => client.GetDatabase("general_shortener"));
                
            });
        }
        
        


        public void AuthenticateAsync()
        {
            this.TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "i_am_a_secure_token");
        }

        public void LogoutAsync()
        {
            this.TestClient.DefaultRequestHeaders.Authorization = null;
        }

        public new void Dispose()
        {
            base.Dispose();
            _mongoRunner?.Dispose();
            TestClient?.Dispose();
            
        }
    }
}