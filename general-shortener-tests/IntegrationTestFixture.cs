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
using Microsoft.Extensions.Hosting;
using Mongo2Go;

namespace general_shortener_tests
{
    public class IntegrationTestFixture : IDisposable
    {
        
        internal readonly MongoDbRunner _mongoRunner;
        internal readonly HttpClient TestClient;
        
        public IntegrationTestFixture()
        {
            this._mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);
            
            
            WebApplicationFactory<Startup> applicationFactory = new WebApplicationFactory<Startup>();
            applicationFactory.WithWebHostBuilder(builder =>
            {
                Console.WriteLine("I AM HEREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE");
                builder.UseEnvironment("Testing");
                builder.UseConfiguration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());

                builder.ConfigureTestServices(services =>
                {
                    services.Configure<MongoDbOptions>(options =>
                    {
                        options.ConnectionString = this._mongoRunner.ConnectionString;
                    });
                });
            });
            
            this.TestClient = applicationFactory.CreateClient();
        }

        public void AuthenticateAsync()
        {
            this.TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "i_am_a_secure_token");
        }

        public void LogoutAsync()
        {
            this.TestClient.DefaultRequestHeaders.Authorization = null;
        }

        public void Dispose()
        {
            _mongoRunner?.Dispose();
            TestClient?.Dispose();
        }
    }
}