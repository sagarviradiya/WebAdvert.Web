using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WebAdvert.Models;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;

        public AdvertApiClient(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            var createUrl = _configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");
            _httpClient.BaseAddress = new Uri(createUrl);
            _httpClient.DefaultRequestHeaders.Add("Content-type", "application/json");
        }

        public async Task<AdvertResponse> Create(AdvertModel model)
        {
            var advertApiModel = new AdvertModel(); //TODO:AutoMapper

            var jsonModel = JsonSerializer.Serialize(advertApiModel);
            var response = await _httpClient.PutAsync(_httpClient.BaseAddress, new StringContent(jsonModel));
            var responseJson = await response.Content.ReadAsStringAsync();
            var createAdvertResponse = JsonSerializer.Deserialize<CreateAdvertResponse>(responseJson);
            var advertResponse = new AdvertResponse(); //TODO:AutoMapper;
            return advertResponse;
        }
    }
}
