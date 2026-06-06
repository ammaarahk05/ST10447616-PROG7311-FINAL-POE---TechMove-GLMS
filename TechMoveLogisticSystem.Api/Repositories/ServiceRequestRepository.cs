using Microsoft.EntityFrameworkCore;
using TechMoveLogisticSystem.Api.Data;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly AppDbContext _context;

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceRequestReadDto>> GetAllAsync()
        {
            // I return DTOs here so the API does not expose full EF navigation objects
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                    .ThenInclude(c => c.Client)
                .Select(sr => new ServiceRequestReadDto
                {
                    Id = sr.Id,
                    Description = sr.Description,
                    Cost = sr.Cost,
                    Status = sr.Status ?? string.Empty,
                    ContractId = sr.ContractId,
                    ContractStatus = sr.Contract != null ? sr.Contract.Status : string.Empty,
                    ClientName = sr.Contract != null ? sr.Contract.Client.Name : string.Empty
                })
                .ToListAsync();
        }

        public async Task<ServiceRequestReadDto?> GetByIdAsync(int id)
        {
            // I fetch one service request and shape it into a clean response
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                    .ThenInclude(c => c.Client)
                .Where(sr => sr.Id == id)
                .Select(sr => new ServiceRequestReadDto
                {
                    Id = sr.Id,
                    Description = sr.Description,
                    Cost = sr.Cost,
                    Status = sr.Status ?? string.Empty,
                    ContractId = sr.ContractId,
                    ContractStatus = sr.Contract != null ? sr.Contract.Status : string.Empty,
                    ClientName = sr.Contract != null ? sr.Contract.Client.Name : string.Empty
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ServiceRequest?> GetEntityByIdAsync(int id)
        {
            // Gets the actual database model so it can be edited and saved
            return await _context.ServiceRequests
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Contract?> GetContractEntityAsync(int contractId)
        {
            // I need the actual Contract entity to validate the workflow rule
            return await _context.Contracts.FindAsync(contractId);
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest)
        {
            // I save the new service request after the service layer validates it
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return serviceRequest;
        }

        public async Task<bool> UpdateAsync(ServiceRequest serviceRequest)
        {
            // Updates the existing service request record
            _context.ServiceRequests.Update(serviceRequest);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Finds the service request first so I can delete it safely
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest == null)
            {
                return false;
            }

            _context.ServiceRequests.Remove(serviceRequest);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}