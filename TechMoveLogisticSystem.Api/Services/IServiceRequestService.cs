using TechMoveLogisticSystem.Api.DTOs;

namespace TechMoveLogisticSystem.Api.Services
{
    public interface IServiceRequestService
    {
        // Gets all service requests
        Task<List<ServiceRequestReadDto>> GetServiceRequestsAsync();

        // Gets one service request by ID
        Task<ServiceRequestReadDto?> GetServiceRequestByIdAsync(int id);

        // Creates a service request after checking business rules
        Task<(bool Success, string? ErrorMessage, ServiceRequestReadDto? ServiceRequest)> CreateServiceRequestAsync(ServiceRequestCreateDto requestDto);
    }
}