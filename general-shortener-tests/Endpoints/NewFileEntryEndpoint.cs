using System.Net;
using System.Net.Http;
using FluentAssertions;
using general_shortener.Models;
using general_shortener.Models.Entry;
using Xunit;

namespace general_shortener_tests.Endpoints
{
    public class NewFileEntryEndpoint: IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;

        public NewFileEntryEndpoint(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
        }

        [Theory]
        [InlineData("Resources/Files/TextDocument.txt")]
        [InlineData("Resources/Files/image.png")]
        [InlineData("Resources/Files/video.mp4")]
        public async void PostNewFileEntryEndpoint(string fileName)
        {
            await this._integrationTestFixture.AuthenticateAsync();
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.file,
            };
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel, fileName));


            message.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory]
        [InlineData("Resources/Files/TextDocument.txt", true)]
        [InlineData("Resources/Files/image.png", false)]
        [InlineData("Resources/Files/video.mp4", false)]
        public async void PostAndGetNewFileEntry(string fileName, bool withDownloadHeader)
        {
            await this._integrationTestFixture.AuthenticateAsync();
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.file,
            };
            
            HttpResponseMessage response = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel, fileName));
            BaseResponse<NewEntryResponseModel> responseModel = await response.Content.ReadAsAsync<BaseResponse<NewEntryResponseModel>>();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.GetAsync(responseModel.Result.Url);

            if (withDownloadHeader)
            {
                message.Content.Headers.ContentDisposition.Should().NotBeNull();
            }
            else
            {
                message.Content.Headers.ContentDisposition.Should().BeNull();
            }

            message.StatusCode.Should().Be(HttpStatusCode.OK);


        }

        [Theory]
        [InlineData("Resources/Files/TextDocument.txt")]
        [InlineData("Resources/Files/image.png")]
        [InlineData("Resources/Files/video.mp4")]
        public async void DeleteAfterCreateAndTest(string fileName)
        {
            await this._integrationTestFixture.AuthenticateAsync();
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.file,
            };
            HttpResponseMessage response = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel, fileName));
            BaseResponse<NewEntryResponseModel> responseModel = await response.Content.ReadAsAsync<BaseResponse<NewEntryResponseModel>>();
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            HttpResponseMessage deletionResponse =
                await _integrationTestFixture.TestClient.GetAsync(responseModel.Result.DeletionUrl);
            
            deletionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            HttpResponseMessage testResponse =
                await _integrationTestFixture.TestClient.GetAsync(responseModel.Result.Url);

            testResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

        }

    }
}