using System.Net;
using System.Net.Http;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;
using Xunit.Extensions.Ordering;

namespace general_shortener_tests.Endpoints
{
    public class GetEntryEndpoint : IClassFixture<IntegrationTest>
    {
        private readonly IntegrationTest _integrationTest;

        public GetEntryEndpoint(IntegrationTest integrationTest)
        {
            _integrationTest = integrationTest;
        }


        [Fact, Order(1)]
        public async void GetNotExistantEntry()
        {
            HttpResponseMessage message = await _integrationTest.TestClient.GetAsync("/jsndbgjksdng");

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        
    }
}