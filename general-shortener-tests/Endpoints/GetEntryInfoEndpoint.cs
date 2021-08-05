using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using general_shortener.Models;
using general_shortener.Models.Data;
using general_shortener.Models.Entry;
using Xunit;

namespace general_shortener_tests.Endpoints
{
    public class GetEntryInfoEndpoint : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;

        public GetEntryInfoEndpoint(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
        }

        [Theory]
        [InlineData("https://docs.microsoft.com/de-de/dotnet/api/system.tuple?view=net-5.0", EntryType.url)]
        [InlineData("https://github.com/merkeg/general-shortener", EntryType.url)]
        [InlineData("https://localhost:5001/rWQBl8", EntryType.url)]
        [InlineData("https://www.youtube.com/watch?v=U6p8qHs8WGA", EntryType.url)]
        [InlineData("Das ist ein Text", EntryType.text)]
        [InlineData("asdasdasdasdasd", EntryType.text)]
        [InlineData("This does not help me", EntryType.text)]
        public async void CreateAndCheckInfo(string value, EntryType type)
        {
            await _integrationTestFixture.AuthenticateAsync();

            NewEntryRequestModel requestModel = new ()
            {
                type = type,
                value = value
            };
            HttpResponseMessage creationResponse = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));
            creationResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            BaseResponse<NewEntryResponseModel> creationResponseModel = await creationResponse.Content.ReadAsAsync<BaseResponse<NewEntryResponseModel>>();

            string slug = new Uri(creationResponseModel.Result.Url).Segments.Last();
            string deletionCode = new Uri(creationResponseModel.Result.DeletionUrl).Segments.Last();
            
            HttpResponseMessage infoResponse = await _integrationTestFixture.TestClient.GetAsync($"/entries/{slug}");
            infoResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            BaseResponse<EntryInfoResponseModel> infoResponseModel = await infoResponse.Content.ReadAsAsync<BaseResponse<EntryInfoResponseModel>>();

            infoResponseModel.Result.Slug.Should().Be(slug);
            infoResponseModel.Result.DeletionCode.Should().Be(deletionCode);
            infoResponseModel.Result.Type.Should().Be(type);
            infoResponseModel.Result.Value.Should().Be(value);
        }
    }
}