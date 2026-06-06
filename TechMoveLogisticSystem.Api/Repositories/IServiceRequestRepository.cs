using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Repositories
{
    public interface IServiceRequestRepository
    {
        // Gets all service requests as clean API response objects
        Task<List<ServiceRequestReadDto>> GetAllAsync();

        // Gets one service request by ID
        Task<ServiceRequestReadDto?> GetByIdAsync(int id);

        // Gets the parent contract so I can check its status before creating a request
        Task<Contract?> GetContractEntityAsync(int contractId);

        // Updates an existing service request in the database
        Task<bool> UpdateAsync(ServiceRequest serviceRequest);

        // Deletes an existing service request by ID
        Task<bool> DeleteAsync(int id);

        // Gets the real database entity when I need to update or delete data
        Task<ServiceRequest?> GetEntityByIdAsync(int id);

        // Saves a new service request to the database
        Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest);
    }
}