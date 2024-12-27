using System.Net.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoreBot.Services
{
    public interface IApiService
    {
        Task<T> GetAllAsync<T>(string endpoint);
        Task<T> GetByIdAsync<T>(string endpoint, int id);
        Task<T> PostAsync<T, TRequest>(string endpoint, TRequest request);
        Task PutAsync<TRequest>(string endpoint, int id, TRequest request);
        Task DeleteAsync(string endpoint, int id);
    }

    public class ApiService : IApiService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;
        private readonly ILogger<ApiService> _logger;

        public ApiService(IConfiguration configuration, ILogger<ApiService> logger)
        {
            _logger = logger;

            _baseUrl = "http://localhost:5243/";
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseUrl);
            _logger.LogInformation("Initializing ApiService with base URL: {BaseUrl}", _baseUrl);
        }

        public async Task<T> GetAllAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogDebug($"GET {endpoint} response: {content}");
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<T> GetByIdAsync<T>(string endpoint, int id)
        {
            var response = await _client.GetAsync($"{endpoint}/by-id/{id}");
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<T> PostAsync<T, TRequest>(string endpoint, TRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{endpoint}/create", data);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task PutAsync<TRequest>(string endpoint, int id, TRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{endpoint}/update/{id}", data);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string endpoint, int id)
        {
            var response = await _client.DeleteAsync($"{endpoint}/remove/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
