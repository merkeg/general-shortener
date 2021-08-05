using System.Net;
using System.Net.Http;
using FluentAssertions;
using general_shortener.Models.Entry;
using Xunit;
using Xunit.Extensions.Ordering;

namespace general_shortener_tests.Endpoints
{
    public class GetEntryEndpoint : IClassFixture<IntegrationTestFixture>, IClassFixture<NewEntryResponseModel>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;
        private readonly NewEntryResponseModel _responseModel;

        public GetEntryEndpoint(IntegrationTestFixture integrationTestFixture, NewEntryResponseModel responseModel)
        {
            _integrationTestFixture = integrationTestFixture;
            _responseModel = responseModel;
        }


        [Fact, Order(1)]
        public async void GetNotExistantEntry()
        {
            HttpResponseMessage message = await _integrationTestFixture.TestClient.GetAsync("/nonexistant");

            message.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


    }
}