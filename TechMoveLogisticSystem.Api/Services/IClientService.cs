using TechMoveLogisticSystem.Api.DTOs;

namespace TechMoveLogisticSystem.Api.Services
{
    public interface IClientService
    {
        // Gets all clients for the API and MVC frontend
        Task<List<ClientReadDto>> GetClientsAsync();

        // Gets one client by ID
        Task<ClientReadDto?> GetClientByIdAsync(int id);

        // Creates a new client after validation
        Task<(bool Success, string? ErrorMessage, ClientReadDto? Client)> CreateClientAsync(ClientCreateDto clientDto);

        Task<(bool Success, string? ErrorMessage)> UpdateClientAsync(int id, ClientUpdateDto clientDto);
        Task<(bool Success, string? ErrorMessage)> DeleteClientAsync(int id);
    }
}