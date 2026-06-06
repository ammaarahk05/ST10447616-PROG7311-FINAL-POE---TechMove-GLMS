using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Repositories
{
    public interface IClientRepository
    {
        // Gets all clients as clean API response objects
        Task<List<ClientReadDto>> GetAllAsync();

        // Gets one client by ID
        Task<ClientReadDto?> GetByIdAsync(int id);

        // Saves a new client to the database
        Task<Client> CreateAsync(Client client);

        // Checks if a client name already exists before creating
        Task<bool> ClientNameExistsAsync(string name);
        Task<bool> UpdateAsync(int id, Client client);
        Task<bool> DeleteAsync(int id);
    }
}