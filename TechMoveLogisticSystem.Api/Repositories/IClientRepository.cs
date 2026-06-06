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

        // Checks if a client name already exists before creating
        Task<bool> ClientNameExistsAsync(string name);

        // Saves a new client to the database
        Task<Client> CreateAsync(Client client);
    }
}