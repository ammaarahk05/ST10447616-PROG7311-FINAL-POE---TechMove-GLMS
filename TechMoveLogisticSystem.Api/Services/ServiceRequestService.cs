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

            // This keeps the Part 2 workflow rule inside the backend API
            var contractStatus = contract.Status?.Trim().ToLower();

            if (contractStatus == "expired" || contractStatus == "on hold")
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

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(int id, ServiceRequestUpdateDto dto)
        {
            if (dto == null)
            {
                return (false, "Service request data is required.");
            }

            // Gets the real database model, not the read DTO
            var existingRequest = await _serviceRequestRepository.GetEntityByIdAsync(id);

            if (existingRequest == null)
            {
                return (false, $"Service request with ID {id} was not found.");
            }

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                return (false, "Description is required.");
            }

            if (dto.Cost < 0)
            {
                return (false, "Cost cannot be negative.");
            }

            var contract = await _serviceRequestRepository.GetContractEntityAsync(dto.ContractId);

            if (contract == null)
            {
                return (false, $"Contract with ID {dto.ContractId} does not exist.");
            }

            // This prevents assigning requests to blocked contracts
            var contractStatus = contract.Status?.Trim().ToLower();

            if (contractStatus == "expired" || contractStatus == "on hold")
            {
                return (false, "Service request cannot be assigned to an Expired or On Hold contract.");
            }

            existingRequest.Description = dto.Description;
            existingRequest.Cost = dto.Cost;
            existingRequest.Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status;
            existingRequest.ContractId = dto.ContractId;

            var updated = await _serviceRequestRepository.UpdateAsync(existingRequest);

            return updated
                ? (true, null)
                : (false, "Service request could not be updated.");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // Deletes through the repository so the controller stays clean
            return await _serviceRequestRepository.DeleteAsync(id);
        }
    }
}