using Microsoft.EntityFrameworkCore;
using TechMoveLogisticSystem.Api.Data;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly AppDbContext _context;

        public ContractRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ContractReadDto>> GetAllAsync(string? status, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(c => c.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.EndDate <= endDate.Value);
            }

            return await query
                .Select(c => new ContractReadDto
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status,
                    ServiceLevel = c.ServiceLevel,
                    ClientId = c.ClientId,
                    ClientName = c.Client.Name,
                    ClientRegion = c.Client.Region,
                    SignedAgreementFileName = c.SignedAgreementFileName,
                    ServiceRequestCount = c.ServiceRequests.Count
                })
                .ToListAsync();
        }

        public async Task<ContractReadDto?> GetByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .Where(c => c.Id == id)
                .Select(c => new ContractReadDto
                {
                    Id = c.Id,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status,
                    ServiceLevel = c.ServiceLevel,
                    ClientId = c.ClientId,
                    ClientName = c.Client.Name,
                    ClientRegion = c.Client.Region,
                    SignedAgreementFileName = c.SignedAgreementFileName,
                    ServiceRequestCount = c.ServiceRequests.Count
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Contract?> GetEntityByIdAsync(int id)
        {
            return await _context.Contracts.FindAsync(id);
        }

        public async Task<bool> ClientExistsAsync(int clientId)
        {
            return await _context.Clients.AnyAsync(c => c.Id == clientId);
        }

        public async Task<Contract> CreateAsync(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}