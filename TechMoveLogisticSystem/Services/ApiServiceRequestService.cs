using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public class ApiServiceRequestService : IApiServiceRequestService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiAuthService _apiAuthService;

        public ApiServiceRequestService(
            IHttpClientFactory httpClientFactory,
            IApiAuthService apiAuthService)
        {
            _httpClientFactory = httpClientFactory;
            _apiAuthService = apiAuthService;
        }

        public async Task<List<ServiceRequestReadDto>> GetServiceRequestsAsync()
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var serviceRequests = await client.GetFromJsonAsync<List<ServiceRequestReadDto>>("api/ServiceRequests");

            return serviceRequests ?? new List<ServiceRequestReadDto>();
        }

        public async Task<ServiceRequestReadDto?> GetServiceRequestByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/ServiceRequests/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ServiceRequestReadDto>();
        }

        public async Task<(bool Success, string? ErrorMessage, ServiceRequestReadDto? ServiceRequest)> CreateServiceRequestAsync(ServiceRequestCreateDto requestDto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            await AddJwtTokenAsync(client);

            var response = await client.PostAsJsonAsync("api/ServiceRequests", requestDto);

            if (response.IsSuccessStatusCode)
            {
                var createdRequest = await response.Content.ReadFromJsonAsync<ServiceRequestReadDto>();
                return (true, null, createdRequest);
            }

            var error = await response.Content.ReadAsStringAsync();

            return (false, error, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateServiceRequestAsync(int id, ServiceRequestUpdateDto requestDto)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            await AddJwtTokenAsync(client);

            var response = await client.PutAsJsonAsync($"api/ServiceRequests/{id}", requestDto);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();

            return (false, error);
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteServiceRequestAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            await AddJwtTokenAsync(client);

            var response = await client.DeleteAsync($"api/ServiceRequests/{id}");

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await response.Content.ReadAsStringAsync();

            return (false, error);
        }

        private async Task AddJwtTokenAsync(HttpClient client)
        {
            var token = await _apiAuthService.GetTokenAsync();

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}