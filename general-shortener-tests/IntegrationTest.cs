using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using general_shortener;
using general_shortener.Models.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;

namespace general_shortener_tests
{
    public class IntegrationTest : IDisposable
    {
        
        internal readonly MongoDbRunner _mongoRunner;
        internal readonly HttpClient TestClient;
        
        public IntegrationTest()
        {
            this._mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true);
            
            IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            
            WebApplicationFactory<Startup> applicationFactory = new WebApplicationFactory<Startup>();
            applicationFactory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureAppConfiguration((webhostBuilder, config) =>
                {
                    webhostBuilder.Configuration = configuration;
                });

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