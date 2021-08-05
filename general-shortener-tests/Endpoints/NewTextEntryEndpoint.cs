using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Flurl.Util;
using general_shortener.Models;
using general_shortener.Models.Entry;
using general_shortener_tests.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;
using Xunit;
using Xunit.Extensions.Ordering;

namespace general_shortener_tests.Endpoints
{
    [CollectionDefinition("Create and get an url entry")]
    public class NewTextEntryEndpoint : IClassFixture<IntegrationTestFixture>, IClassFixture<ObjectBearer<BaseResponse<NewEntryResponseModel>>>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;
        private readonly ObjectBearer<BaseResponse<NewEntryResponseModel>> _objectBearer;

        public NewTextEntryEndpoint(IntegrationTestFixture integrationTestFixture, ObjectBearer<BaseResponse<NewEntryResponseModel>> objectBearer)
        {
            _integrationTestFixture = integrationTestFixture;
            _objectBearer = objectBearer;
        }
        

        [Fact, Order(0)]
        public async void CreateNewEntryUnauthorized()
        {
            await _integrationTestFixture.LogoutAsync();
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = "https://www.google.com"
            };
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));


            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        [Fact, Order(0)]
        public async void CreateNewEntryWrongToken()
        {
            await _integrationTestFixture.LogoutAsync();
            _integrationTestFixture.TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "WRONG TOKEN");
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = "https://www.google.com"
            };
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));

            await _integrationTestFixture.LogoutAsync();

            message.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        
        
        [Fact, Order(1)]
        public async void CreateNewEntryUnsupported()
        {
            await _integrationTestFixture.AuthenticateAsync();
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = "https://www.google.com"
            };
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsJsonAsync("/entries", requestModel);


            message.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }
        
        [Fact, Order(2)]
        public async void CreateNewEntryCreated()
        {
            await _integrationTestFixture.AuthenticateAsync();
            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = "https://www.google.com"
            };
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));
            BaseResponse<NewEntryResponseModel> responseModel = await message.Content.ReadAsAsync<BaseResponse<NewEntryResponseModel>>();
            this._objectBearer.Object = responseModel;
            
            
            message.StatusCode.Should().Be(HttpStatusCode.Created);
            responseModel.Should().NotBe(null);
        }
        
        [Theory, Order(3)]
        [InlineData("https://docs.microsoft.com/de-de/dotnet/api/system.tuple?view=net-5.0", true)]
        [InlineData("https://github.com/merkeg/general-shortener", true)]
        [InlineData("https://localhost:5001/rWQBl8", true)]
        [InlineData("https://www.youtube.com/watch?v=U6p8qHs8WGA", true)]
        [InlineData("file:///jdabkjadsgb", false)]
        [InlineData("htpp//www.youtube.com", false)]
        [InlineData("www.youtube.com", false)]
        [InlineData("Wie geht es dir?", false)]
        public async void CreateNewEntryUrlTest(string uri, bool expectedResult)
        {
            await _integrationTestFixture.AuthenticateAsync();

            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = uri
            };
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));
            message.StatusCode.Should().Be(expectedResult ? HttpStatusCode.Created : HttpStatusCode.BadRequest);
            
            
        }


        
        [Theory, Order(4)]
        [InlineData("https://www.google.com/")]
        public async void GetEntry(string uri)
        {
            await _integrationTestFixture.AuthenticateAsync();

            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = uri
            };
            
            HttpResponseMessage request = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));
            BaseResponse<NewEntryResponseModel> responseModel = await request.Content.ReadAsAsync<BaseResponse<NewEntryResponseModel>>();

            HttpResponseMessage message = await _integrationTestFixture.TestClient.GetAsync(responseModel.Result.Url);
            
            message.StatusCode.Should().Be(HttpStatusCode.Redirect);
            message.Headers.Location.ToString().Should().Be(uri);


        }
        
        
        
    }
}