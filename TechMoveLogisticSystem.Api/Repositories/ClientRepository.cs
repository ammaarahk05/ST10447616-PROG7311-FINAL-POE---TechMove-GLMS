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
    }
}