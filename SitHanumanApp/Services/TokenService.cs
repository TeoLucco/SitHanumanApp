using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Maui.Storage;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SitHanumanApp.Services
{
    public class TokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

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
                var response = await _httpClient.PostAsJsonAsync("/api/token/", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                    return result;
                }
                else
                {
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
                var accessToken = Preferences.Get("accessToken", string.Empty);

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
                var refreshToken = Preferences.Get("refreshToken", string.Empty);

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var refreshData = new { refresh = refreshToken };
                    var response = await _httpClient.PostAsJsonAsync("/api/token/refresh/", refreshData);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                        if (result != null)
                        {
                            Preferences.Set("accessToken", result.AccessToken);
                            Preferences.Set("refreshToken", result.RefreshToken);
                            return result.AccessToken;
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

        public async Task<HttpResponseMessage> GetProtectedResourceAsync()
        {
            try
            {
                var accessToken = await GetAccessTokenAsync();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await _httpClient.GetAsync("/api/protected/");
                    return response;
                }
                else
                {
                    throw new Exception("Access token is invalid or not found.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while accessing the protected resource.", e);
            }
        }
    }
}
