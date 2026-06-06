using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;
using TechMoveLogisticSystem.Api.Repositories;

namespace TechMoveLogisticSystem.Api.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public ServiceRequestService(IServiceRequestRepository serviceRequestRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
        }

        public async Task<List<ServiceRequestReadDto>> GetServiceRequestsAsync()
        {
            return await _serviceRequestRepository.GetAllAsync();
        }

        public async Task<ServiceRequestReadDto?> GetServiceRequestByIdAsync(int id)
        {
            return await _serviceRequestRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage, ServiceRequestReadDto? ServiceRequest)> CreateServiceRequestAsync(ServiceRequestCreateDto requestDto)
        {
            if (requestDto == null)
            {
                return (false, "Service request data is required.", null);
            }

            if (string.IsNullOrWhiteSpace(requestDto.Description))
            {
                return (false, "Description is required.", null);
            }

            if (requestDto.Cost < 0)
            {
                return (false, "Cost cannot be negative.", null);
            }

            var contract = await _serviceRequestRepository.GetContractEntityAsync(requestDto.ContractId);

            if (contract == null)
            {
                return (false, $"Contract with ID {requestDto.ContractId} does not exist.", null);
            }

            // This is the key workflow rule from Part 2
            if (contract.Status == "Expired" || contract.Status == "On Hold")
            {
                return (false, "Service request cannot be created for an Expired or On Hold contract.", null);
            }

            var serviceRequest = new ServiceRequest
            {
                Description = requestDto.Description,
                Cost = requestDto.Cost,
                Status = string.IsNullOrWhiteSpace(requestDto.Status) ? "Pending" : requestDto.Status,
                ContractId = requestDto.ContractId
            };

            var createdRequest = await _serviceRequestRepository.CreateAsync(serviceRequest);

            var readDto = await _serviceRequestRepository.GetByIdAsync(createdRequest.Id);

            return (true, null, readDto);
        }
    }
}