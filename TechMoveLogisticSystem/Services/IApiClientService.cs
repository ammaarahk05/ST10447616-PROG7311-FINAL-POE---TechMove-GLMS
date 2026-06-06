using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public interface IApiClientService
    {
        // Gets all clients from the backend API
        Task<List<ClientReadDto>> GetClientsAsync();

        // Gets one client by ID from the backend API
        Task<ClientReadDto?> GetClientByIdAsync(int id);

        // Sends a new client to the backend API
        Task<(bool Success, string? ErrorMessage)> CreateClientAsync(ClientCreateDto clientDto);

        // Sends edited client details to the backend API
        Task<(bool Success, string? ErrorMessage)> UpdateClientAsync(int id, ClientUpdateDto clientDto);

        // Deletes a client through the backend API
        Task<(bool Success, string? ErrorMessage)> DeleteClientAsync(int id);
    }
}