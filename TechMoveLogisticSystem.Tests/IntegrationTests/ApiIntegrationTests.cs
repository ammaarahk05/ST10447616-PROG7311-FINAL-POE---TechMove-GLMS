using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace TechMoveLogisticSystem.Tests.IntegrationTests
{
    public class ApiIntegrationTests
    {
        private const string ApiBaseUrl = "https://localhost:7120/";

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private HttpClient CreateHttpClient()
        {
            // Allows the test project to call the local HTTPS API during development
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(ApiBaseUrl)
            };
        }

        private async Task<HttpClient> CreateAuthorizedHttpClientAsync()
        {
            var client = CreateHttpClient();

            var loginResponse = await client.PostAsJsonAsync("api/Auth/login", new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin@123"
            });

            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);

            Assert.NotNull(loginResult);
            Assert.False(string.IsNullOrWhiteSpace(loginResult.Token));

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResult.Token);

            return client;
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsJwtToken()
        {
            using var client = CreateHttpClient();

            var response = await client.PostAsJsonAsync("api/Auth/login", new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin@123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
            Assert.Equal("Login successful.", result.Message);
        }

        [Fact]
        public async Task ProtectedClientPost_WithoutToken_ReturnsUnauthorized()
        {
            using var client = CreateHttpClient();

            var newClient = new ClientCreateDto
            {
                Name = "Unauthorized Integration Test Client",
                ContactDetails = "unauthorized-integration@test.com",
                Region = "Test Region"
            };

            var response = await client.PostAsJsonAsync("api/Clients", newClient);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Clients_CreateReadDelete_VerifiesDataIntegrity()
        {
            using var client = await CreateAuthorizedHttpClientAsync();

            var uniqueName = $"Integration Client {Guid.NewGuid()}";

            var createDto = new ClientCreateDto
            {
                Name = uniqueName,
                ContactDetails = "integration-client@test.com",
                Region = "KwaZulu-Natal"
            };

            var createResponse = await client.PostAsJsonAsync("api/Clients", createDto);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdClient = await createResponse.Content.ReadFromJsonAsync<ClientReadDto>(JsonOptions);

            Assert.NotNull(createdClient);
            Assert.True(createdClient.Id > 0);
            Assert.Equal(uniqueName, createdClient.Name);
            Assert.Equal("integration-client@test.com", createdClient.ContactDetails);
            Assert.Equal("KwaZulu-Natal", createdClient.Region);

            var readResponse = await client.GetAsync($"api/Clients/{createdClient.Id}");

            Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

            var readClient = await readResponse.Content.ReadFromJsonAsync<ClientReadDto>(JsonOptions);

            Assert.NotNull(readClient);
            Assert.Equal(createdClient.Id, readClient.Id);
            Assert.Equal(uniqueName, readClient.Name);

            var deleteResponse = await client.DeleteAsync($"api/Clients/{createdClient.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var readAfterDeleteResponse = await client.GetAsync($"api/Clients/{createdClient.Id}");

            Assert.Equal(HttpStatusCode.NotFound, readAfterDeleteResponse.StatusCode);
        }

        [Fact]
        public async Task GetCoreApiEndpoints_ReturnsOk()
        {
            using var client = CreateHttpClient();

            var clientsResponse = await client.GetAsync("api/Clients");
            var contractsResponse = await client.GetAsync("api/Contracts");
            var serviceRequestsResponse = await client.GetAsync("api/ServiceRequests");

            Assert.Equal(HttpStatusCode.OK, clientsResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, contractsResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, serviceRequestsResponse.StatusCode);

            var clientsJson = await clientsResponse.Content.ReadAsStringAsync();
            var contractsJson = await contractsResponse.Content.ReadAsStringAsync();
            var serviceRequestsJson = await serviceRequestsResponse.Content.ReadAsStringAsync();

            Assert.False(string.IsNullOrWhiteSpace(clientsJson));
            Assert.False(string.IsNullOrWhiteSpace(contractsJson));
            Assert.False(string.IsNullOrWhiteSpace(serviceRequestsJson));
        }

        [Fact]
        public async Task ServiceRequests_CreateReadDelete_ForActiveContract_VerifiesDataIntegrity()
        {
            using var client = await CreateAuthorizedHttpClientAsync();

            var activeContractId = await GetContractIdByStatusAsync(client, "Active");

            var uniqueDescription = $"Integration Service Request {Guid.NewGuid()}";

            var createDto = new ServiceRequestCreateDto
            {
                Description = uniqueDescription,
                Cost = 1750,
                Status = "Pending",
                ContractId = activeContractId
            };

            var createResponse = await client.PostAsJsonAsync("api/ServiceRequests", createDto);

            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var createdRequest = await createResponse.Content.ReadFromJsonAsync<ServiceRequestReadDto>(JsonOptions);

            Assert.NotNull(createdRequest);
            Assert.True(createdRequest.Id > 0);
            Assert.Equal(uniqueDescription, createdRequest.Description);
            Assert.Equal(1750, createdRequest.Cost);
            Assert.Equal("Pending", createdRequest.Status);
            Assert.Equal(activeContractId, createdRequest.ContractId);

            var readResponse = await client.GetAsync($"api/ServiceRequests/{createdRequest.Id}");

            Assert.Equal(HttpStatusCode.OK, readResponse.StatusCode);

            var readRequest = await readResponse.Content.ReadFromJsonAsync<ServiceRequestReadDto>(JsonOptions);

            Assert.NotNull(readRequest);
            Assert.Equal(createdRequest.Id, readRequest.Id);
            Assert.Equal(uniqueDescription, readRequest.Description);

            var deleteResponse = await client.DeleteAsync($"api/ServiceRequests/{createdRequest.Id}");

            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

            var readAfterDeleteResponse = await client.GetAsync($"api/ServiceRequests/{createdRequest.Id}");

            Assert.Equal(HttpStatusCode.NotFound, readAfterDeleteResponse.StatusCode);
        }

        [Fact]
        public async Task ServiceRequests_CreateForExpiredContract_ReturnsBadRequest()
        {
            using var client = await CreateAuthorizedHttpClientAsync();

            var expiredContractId = await GetContractIdByStatusAsync(client, "Expired");

            var createDto = new ServiceRequestCreateDto
            {
                Description = "Integration expired contract block test",
                Cost = 500,
                Status = "Pending",
                ContractId = expiredContractId
            };

            var response = await client.PostAsJsonAsync("api/ServiceRequests", createDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();

            Assert.Contains("Expired", errorMessage);
        }

        [Fact]
        public async Task ServiceRequests_CreateForOnHoldContract_ReturnsBadRequest()
        {
            using var client = await CreateAuthorizedHttpClientAsync();

            var onHoldContractId = await GetContractIdByStatusAsync(client, "On Hold");

            var createDto = new ServiceRequestCreateDto
            {
                Description = "Integration on hold contract block test",
                Cost = 500,
                Status = "Pending",
                ContractId = onHoldContractId
            };

            var response = await client.PostAsJsonAsync("api/ServiceRequests", createDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();

            Assert.Contains("On Hold", errorMessage);
        }

        [Fact]
        public async Task ServiceRequests_CreateWithNegativeCost_ReturnsBadRequest()
        {
            using var client = await CreateAuthorizedHttpClientAsync();

            var activeContractId = await GetContractIdByStatusAsync(client, "Active");

            var createDto = new ServiceRequestCreateDto
            {
                Description = "Integration negative cost test",
                Cost = -100,
                Status = "Pending",
                ContractId = activeContractId
            };

            var response = await client.PostAsJsonAsync("api/ServiceRequests", createDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();

            Assert.Contains("Cost cannot be negative", errorMessage);
        }

        [Fact]
        public async Task ServiceRequests_CreateWithBlankDescription_ReturnsBadRequest()
        {
            using var client = await CreateAuthorizedHttpClientAsync();

            var activeContractId = await GetContractIdByStatusAsync(client, "Active");

            var createDto = new ServiceRequestCreateDto
            {
                Description = "",
                Cost = 100,
                Status = "Pending",
                ContractId = activeContractId
            };

            var response = await client.PostAsJsonAsync("api/ServiceRequests", createDto);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var errorMessage = await response.Content.ReadAsStringAsync();

            Assert.Contains("Description is required", errorMessage);
        }

        private async Task<int> GetContractIdByStatusAsync(HttpClient client, string requiredStatus)
        {
            var response = await client.GetAsync("api/Contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts = await response.Content.ReadFromJsonAsync<List<ContractReadDto>>(JsonOptions);

            Assert.NotNull(contracts);

            var contract = contracts.FirstOrDefault(c =>
                string.Equals(c.Status, requiredStatus, StringComparison.OrdinalIgnoreCase));

            Assert.NotNull(contract);

            return contract.Id;
        }

        private class LoginRequestDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        private class LoginResponseDto
        {
            public string Token { get; set; } = string.Empty;
            public DateTime ExpiresAt { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        private class ClientCreateDto
        {
            public string Name { get; set; } = string.Empty;
            public string ContactDetails { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
        }

        private class ClientReadDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string ContactDetails { get; set; } = string.Empty;
            public string Region { get; set; } = string.Empty;
            public int ContractCount { get; set; }
        }

        private class ContractReadDto
        {
            public int Id { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public string ServiceLevel { get; set; } = string.Empty;
            public int ClientId { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public string ClientRegion { get; set; } = string.Empty;
            public string SignedAgreementFileName { get; set; } = string.Empty;
        }

        private class ServiceRequestCreateDto
        {
            public string Description { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public string Status { get; set; } = string.Empty;
            public int ContractId { get; set; }
        }

        private class ServiceRequestReadDto
        {
            public int Id { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal Cost { get; set; }
            public string Status { get; set; } = string.Empty;
            public int ContractId { get; set; }
            public string ContractStatus { get; set; } = string.Empty;
            public string ClientName { get; set; } = string.Empty;
        }
    }
}