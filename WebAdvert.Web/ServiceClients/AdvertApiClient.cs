using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebAdvert.Api.Domain.Models;
using WebAdvert.Web.Models.Adverts;
using ConfirmAdvertModel = WebAdvert.Web.Models.Adverts.ConfirmAdvertModel;

namespace WebAdvert.Web.ServiceClients
{
    public class AdvertApiClient : IAdvertApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AdvertApiClient(IConfiguration configuration, HttpClient httpClient, IMapper mapper)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _mapper = mapper;


            var createUrl = _configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
            _httpClient.BaseAddress = new Uri(createUrl);
        }

        public async Task<string> CreateAsync(CreateAdvertModel model)
        {
            var advertModel = _mapper.Map<AdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(advertModel);
            var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/create", new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false);
            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseJson;
        }

        public async Task<bool> ConfirmAsync(ConfirmAdvertModel model)
        {
            var confirmAdvertModel = _mapper.Map<Api.Domain.Models.ConfirmAdvertModel>(model);
            var jsonModel = JsonConvert.SerializeObject(confirmAdvertModel);
            var response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/confirm", new StringContent(jsonModel, Encoding.UTF8, "application/json")).ConfigureAwait(false);

            return response.StatusCode == HttpStatusCode.OK;
        }
    }
}
