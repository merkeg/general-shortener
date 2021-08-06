using System.Net;
using System.Net.Http;
using System.Web;
using FluentAssertions;
using general_shortener.Models;
using general_shortener.Models.Data;
using general_shortener.Models.Entry;
using Xunit;
using Xunit.Extensions.Ordering;

namespace general_shortener_tests.Endpoints
{
    public class GetEntriesEndpoint : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _integrationTestFixture;

        public GetEntriesEndpoint(IntegrationTestFixture integrationTestFixture)
        {
            _integrationTestFixture = integrationTestFixture;
        }
        
        [Theory, Order(1)]
        [InlineData("https://docs.microsoft.com/de-de/dotnet/api/system.tuple?view=net-5.0")]
        [InlineData("https://github.com/merkeg/general-shortener")]
        [InlineData("https://localhost:5001/rWQBl8")]
        [InlineData("https://www.youtube.com/watch?v=U6p8qHs8WGA")]
        [InlineData("https://github.com/merkeg/general-shortener/tree/feature/entry_listing_info")]
        [InlineData("https://www.youtube.com/watch?v=C0TRAb_aHpk")]
        [InlineData("https://stackoverflow.com/questions/23576726/using-readasasynct-to-deserialize-complex-json-object")]
        [InlineData("https://stackoverflow.com/questions/8796618/how-can-i-change-property-names-when-serializing-with-json-net/8796648")]
        [InlineData("https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use")]
        [InlineData("https://localhost:5001/oSyzTQ")]
        public async void FillWithDummyData(string uri)
        {
            await _integrationTestFixture.AuthenticateAsync();

            NewEntryRequestModel requestModel = new ()
            {
                type = EntryType.url,
                value = uri
            };
            HttpResponseMessage message = await _integrationTestFixture.TestClient.PostAsync("/entries", HttpUtils.ConstructFormDataContent(requestModel));
            message.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Theory, Order(2)]
        [InlineData(0, 2, 2, true)]
        [InlineData(2, 2, 2, true)]
        [InlineData(0, 10, 10, true)]
        [InlineData(0, 11, 10, true)]
        [InlineData(5, 15, 5, true)]
        [InlineData(9, 11, 1, true)]
        [InlineData(0, 100, 10, true)]
        [InlineData(100, 100, 0, true)]
        [InlineData(100, 1000, 0, false)]
        [InlineData(100, 101, 0, false)]
        public async void TestLimits(int offset, int limit, int expectedAmount, bool work)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["limit"] = limit.ToString();
            queryString["offset"] = offset.ToString();
            
            HttpResponseMessage message = await _integrationTestFixture.TestClient.GetAsync($"/entries?{queryString.ToString()}");

            if (!work)
            {
                message.StatusCode.Should().Be(HttpStatusCode.BadRequest);
                return;
            }
            
            BaseResponse<EntryInfoResponseModel[]> response =
                    await message.Content.ReadAsAsync<BaseResponse<EntryInfoResponseModel[]>>();
            message.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Result.Length.Should().Be(expectedAmount);

        }
    }
}