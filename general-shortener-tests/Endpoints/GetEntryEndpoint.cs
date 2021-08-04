using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using MongoDB.Bson;
using Xunit;
using Xunit.Extensions.Ordering;

namespace general_shortener_tests.Endpoints
{
    public class GetEntryEndpoint : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;

        public GetEntryEndpoint(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
        }


        [Fact, Order(1)]
        public async void GetNotExistantEntry()
        {
            HttpResponseMessage message = await _integrationTestFixture.TestClient.GetAsync("/jsndbgjksdng");

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        
    }
}