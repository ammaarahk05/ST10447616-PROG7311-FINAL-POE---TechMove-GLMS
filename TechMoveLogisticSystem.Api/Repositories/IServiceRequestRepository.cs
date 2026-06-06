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

        // Saves a new service request to the database
        Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest);
    }
}