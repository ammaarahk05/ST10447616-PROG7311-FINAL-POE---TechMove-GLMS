using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public interface IApiServiceRequestService
    {
        Task<List<ServiceRequestReadDto>> GetServiceRequestsAsync();

        Task<ServiceRequestReadDto?> GetServiceRequestByIdAsync(int id);

        Task<(bool Success, string? ErrorMessage, ServiceRequestReadDto? ServiceRequest)> CreateServiceRequestAsync(ServiceRequestCreateDto requestDto);

        Task<(bool Success, string? ErrorMessage)> UpdateServiceRequestAsync(int id, ServiceRequestUpdateDto requestDto);

        Task<(bool Success, string? ErrorMessage)> DeleteServiceRequestAsync(int id);
    }
}