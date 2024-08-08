using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SitHanumanApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly TokenService _tokenService;

        public ApiService(IConfiguration configuration, JsonSerializerOptions jsonOptions, TokenService tokenService)
        {
            _jsonOptions = jsonOptions;
            _tokenService = tokenService;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration["ApiBaseUrl"])
            };
        }

        public async Task<T?> GetAsync<T>(string requestUri)
        {
            var accessToken = await _tokenService.GetAccessTokenAsync();
            if (accessToken != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(responseData, _jsonOptions);
            }
            else
            {
                throw new HttpRequestException($"Request failed with status code: {response.StatusCode}");
            }
        }

        public async Task<T?> PostAsync<T>(string requestUri, object? content)
        {
            try
            {
                var accessToken = await _tokenService.GetAccessTokenAsync();
                if (accessToken != null)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                StringContent? httpContent = null;
                if (content != null)
                {
                    var jsonContent = JsonSerializer.Serialize(content, _jsonOptions);
                    httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.PostJsonAsync(requestUri, httpContent);

                var responseData = await response.Content.ReadAsStringAsync();
                var jsonData = JsonSerializer.Deserialize<T>(responseData, _jsonOptions);
                return jsonData;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }
        }
    }
}
