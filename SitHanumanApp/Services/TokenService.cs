using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using SitHanumanApp.Models;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using ZXing.Aztec.Internal;

namespace SitHanumanApp.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private LoginResult? _token;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient 
            {
                    BaseAddress = new Uri(configuration["ApiBaseUrl"])
            };
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            try
            {
                var loginData = new { username, password };
                var response = await _httpClient.PostJsonAsync("/api/token/", loginData);

                if (response.IsSuccessStatusCode)
                {
                    _token = await response.Content.ReadFromJsonAsync<LoginResult>();
                    return _token;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Errore nella risposta: {errorContent}");
                    throw new Exception("Login failed: Invalid username or password.");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Network error: Unable to reach the server.", e);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during login.", e);
            }
        }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {

                var accessToken = _token?.Access;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    if (IsTokenExpired(accessToken))
                    {
                        var newAccessToken = await RefreshTokenAsync();
                        if (newAccessToken != null)
                        {
                            return newAccessToken;
                        }
                        else
                        {
                            throw new Exception("Unable to refresh access token.");
                        }
                    }
                    else
                    {
                        return accessToken;
                    }
                }
                else
                {
                    throw new Exception("Access token not found.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while retrieving the access token.", e);
            }
        }

        private bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JsonWebTokenHandler();
                var jsonToken = handler.ReadJsonWebToken(token);
                var expirationDate = jsonToken.ValidTo;
                return expirationDate < DateTime.UtcNow;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while checking token expiration.", e);
            }
        }

        private async Task<string> RefreshTokenAsync()
        {
            try
            {
                var refreshToken = _token?.Refresh;

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshData = new { refresh = refreshToken };
                    var response = await _httpClient.PostJsonAsync("/api/token/refresh/", refreshData);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                        if (result != null)
                        {
                            _token = result;
                            return result.Access;
                        }
                        else
                        {
                            throw new Exception("Refresh token failed: Invalid response.");
                        }
                    }
                    else
                    {
                        throw new Exception("Refresh token failed: Invalid refresh token.");
                    }
                }
                else
                {
                    throw new Exception("Refresh token not found.");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Network error: Unable to reach the server for refresh token.", e);
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while refreshing the token.", e);
            }
        }
    }

    public static class HttpClientExtension
    {
        public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client,
                                                                       string requestUri,
                                                                       T value)
        {
            // Serializzazione usando System.Text.Json
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true // Imposta su true per una formattazione leggibile
            };
            var data = JsonSerializer.Serialize(value, options);
            var content = new StringContent(data, Encoding.UTF8, "application/json");

            // Log della richiesta (opzionale)
            Debug.WriteLine(client.BaseAddress + requestUri);

            // Invio della richiesta
            return await client.PostAsync(requestUri, content).ConfigureAwait(false);
        }
    }
}
