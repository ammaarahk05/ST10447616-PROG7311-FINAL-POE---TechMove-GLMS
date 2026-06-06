using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public class ApiContractService : IApiContractService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiAuthService _apiAuthService;

        public ApiContractService(IHttpClientFactory httpClientFactory, IApiAuthService apiAuthService)
        {
            _httpClientFactory = httpClientFactory;
            _apiAuthService = apiAuthService;
        }

        public async Task<List<ContractReadDto>> GetContractsAsync(string? status, DateTime? startDate, DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var queryParts = new List<string>();

            if (!string.IsNullOrWhiteSpace(status))
            {
                queryParts.Add($"status={Uri.EscapeDataString(status)}");
            }

            if (startDate.HasValue)
            {
                queryParts.Add($"startDate={startDate.Value:O}");
            }

            if (endDate.HasValue)
            {
                queryParts.Add($"endDate={endDate.Value:O}");
            }

            var queryString = queryParts.Any()
                ? "?" + string.Join("&", queryParts)
                : string.Empty;

            // MVC reads contract data from the backend API
            var contracts = await client.GetFromJsonAsync<List<ContractReadDto>>($"api/Contracts{queryString}");

            return contracts ?? new List<ContractReadDto>();
        }

        public async Task<ContractReadDto?> GetContractByIdAsync(int id)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/Contracts/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ContractReadDto>();
        }

        public async Task<ContractReadDto?> CreateContractAsync(ContractCreateDto contractDto)
        {
            var client = await CreateAuthorizedClientAsync();

            // Creating contracts is protected, so the JWT token is attached first
            var response = await client.PostAsJsonAsync("api/Contracts", contractDto);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ContractReadDto>();
        }

        public async Task<bool> UpdateContractStatusAsync(int id, string status)
        {
            var client = await CreateAuthorizedClientAsync();

            var statusDto = new ContractStatusUpdateDto
            {
                Status = status
            };

            // MVC calls the API PATCH endpoint instead of changing SQL directly
            var response = await client.PatchAsJsonAsync($"api/Contracts/{id}/status", statusDto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var client = await CreateAuthorizedClientAsync();

            // MVC deletes through the backend API
            var response = await client.DeleteAsync($"api/Contracts/{id}");

            return response.IsSuccessStatusCode;
        }

        public async Task<AgreementUploadResultDto?> UploadAgreementAsync(int contractId, IFormFile file)
        {
            var client = await CreateAuthorizedClientAsync();

            using var formData = new MultipartFormDataContent();

            await using var fileStream = file.OpenReadStream();

            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // The API expects the form field name to be "file"
            formData.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync($"api/Contracts/{contractId}/agreement", formData);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AgreementUploadResultDto>();
        }

        public async Task<(byte[]? FileBytes, string? ContentType, string? FileName)> DownloadAgreementAsync(int contractId)
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            var response = await client.GetAsync($"api/Contracts/{contractId}/agreement");

            if (!response.IsSuccessStatusCode)
            {
                return (null, null, null);
            }

            var fileBytes = await response.Content.ReadAsByteArrayAsync();

            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/pdf";

            var fileName = response.Content.Headers.ContentDisposition?.FileNameStar
                           ?? response.Content.Headers.ContentDisposition?.FileName
                           ?? "signed-agreement.pdf";

            fileName = fileName.Replace("\"", "");

            return (fileBytes, contentType, fileName);
        }

        private async Task<HttpClient> CreateAuthorizedClientAsync()
        {
            var client = _httpClientFactory.CreateClient("TechMoveApi");

            // I attach the JWT token before calling protected API endpoints
            var token = await _apiAuthService.GetTokenAsync();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }
}