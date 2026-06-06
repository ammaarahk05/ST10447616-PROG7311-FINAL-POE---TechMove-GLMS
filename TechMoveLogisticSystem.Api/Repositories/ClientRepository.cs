using Microsoft.EntityFrameworkCore;
using TechMoveLogisticSystem.Api.Data;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;

        public ClientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientReadDto>> GetAllAsync()
        {
            //return DTOs so the API does not expose full EF navigation objects
            return await _context.Clients
                .Include(c => c.Contracts)
                .Select(c => new ClientReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ContactDetails = c.ContactDetails,
                    Region = c.Region,
                    ContractCount = c.Contracts.Count
                })
                .ToListAsync();
        }

        public async Task<ClientReadDto?> GetByIdAsync(int id)
        {
            // fetch one client and include a useful contract count
            return await _context.Clients
                .Include(c => c.Contracts)
                .Where(c => c.Id == id)
                .Select(c => new ClientReadDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ContactDetails = c.ContactDetails,
                    Region = c.Region,
                    ContractCount = c.Contracts.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ClientNameExistsAsync(string name)
        {
            // it'd prevent duplicate client names for cleaner data
            return await _context.Clients
                .AnyAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<Client> CreateAsync(Client client)
        {
            // saves the new client after the service layer validates it
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return client;
        }

        public async Task<bool> UpdateAsync(int id, Client updatedClient)
        {
            // Finds the existing client record first
            var existingClient = await _context.Clients.FindAsync(id);

            if (existingClient == null)
            {
                return false;
            }

            // Updates only the fields the user can edit
            existingClient.Name = updatedClient.Name;
            existingClient.ContactDetails = updatedClient.ContactDetails;
            existingClient.Region = updatedClient.Region;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Finds the client before deleting
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return false;
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}