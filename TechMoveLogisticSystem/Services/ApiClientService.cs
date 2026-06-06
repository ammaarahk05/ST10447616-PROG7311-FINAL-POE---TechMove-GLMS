using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public class ApiClientService : IApiClientService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiAuthService _apiAuthService;

        public ApiClientService(
            IHttpClientFactory httpClientFactory,
            IApiAuthService apiAuthService)
        {
            _httpClientFactory = httpClientFactory;
            _apiAuthService = apiAuthService;
        }

        private async Task<HttpClient> CreateAuthorizedClientAsync()
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            // Gets the JWT token so MVC can access protected API endpoints
            var token = await _apiAuthService.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        public async Task<List<ClientReadDto>> GetClientsAsync()
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            // GET endpoints can still be read without needing login
            var clients = await client.GetFromJsonAsync<List<ClientReadDto>>("api/Clients");

            return clients ?? new List<ClientReadDto>();
        }

        public async Task<ClientReadDto?> GetClientByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/Clients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ClientReadDto>();
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateClientAsync(ClientCreateDto clientDto)
        {
            var client = await CreateAuthorizedClientAsync();

            // POST is protected, so this uses the JWT token
            var response = await client.PostAsJsonAsync("api/Clients", clientDto);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateClientAsync(int id, ClientUpdateDto clientDto)
        {
            var client = await CreateAuthorizedClientAsync();

            // PUT is protected, so this uses the JWT token
            var response = await client.PutAsJsonAsync($"api/Clients/{id}", clientDto);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteClientAsync(int id)
        {
            var client = await CreateAuthorizedClientAsync();

            // DELETE is protected, so this uses the JWT token
            var response = await client.DeleteAsync($"api/Clients/{id}");

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();
            return (false, error);
        }
    }
}