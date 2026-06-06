using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public class ApiAuthService : IApiAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiSettings _apiSettings;

        private string? _cachedToken;
        private DateTime _tokenExpiryTime;

        public ApiAuthService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<string> GetTokenAsync()
        {
            // I reuse the token until it is close to expiring
            if (!string.IsNullOrWhiteSpace(_cachedToken) && DateTime.UtcNow < _tokenExpiryTime.AddMinutes(-5))
            {
                return _cachedToken;
            }

            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var loginRequest = new LoginRequestDto
            {
                Username = _apiSettings.Username,
                Password = _apiSettings.Password
            };

            var response = await client.PostAsJsonAsync("api/Auth/login", loginRequest);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("MVC frontend could not authenticate with the backend API.");
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
            {
                throw new InvalidOperationException("Backend API did not return a valid JWT token.");
            }

            _cachedToken = loginResponse.Token;
            _tokenExpiryTime = loginResponse.ExpiresAt;

            return _cachedToken;
        }
    }
}