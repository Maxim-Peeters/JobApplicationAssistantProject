using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
            try
            {
                _logger.LogInformation("Making GET request to {Endpoint}", endpoint);

                var response = await _client.GetAsync(endpoint);
                _logger.LogInformation("Received response with status code: {StatusCode}", response.StatusCode);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Received content: {Content}", content);

                return JsonSerializer.Deserialize<T>(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed for endpoint {Endpoint}. Status: {Status}",
                    endpoint, ex.StatusCode);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize response from {Endpoint}", endpoint);
                throw;
            }
        }

        public async Task<T> GetByIdAsync<T>(string endpoint, int id)
        {
            var response = await _client.GetAsync($"{endpoint}/by-id/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }

        public async Task<T> PostAsync<T, TRequest>(string endpoint, TRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var data = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{endpoint}/create/", data);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content);
        }

        public async Task PutAsync<TRequest>(string endpoint, int id, TRequest request)
        {
            var json = JsonSerializer.Serialize(request);
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