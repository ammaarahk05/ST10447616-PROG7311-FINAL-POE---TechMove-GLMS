using Microsoft.AspNetCore.Http;
using TechMoveLogisticSystem.Api.DTOs;

namespace TechMoveLogisticSystem.Api.Services
{
    public interface IContractService
    {
        Task<List<ContractReadDto>> GetContractsAsync(string? status, DateTime? startDate, DateTime? endDate);

        Task<ContractReadDto?> GetContractByIdAsync(int id);

        Task<(bool Success, string? ErrorMessage, ContractReadDto? Contract)> CreateContractAsync(ContractCreateDto contractDto);

        // Uploads a signed PDF agreement for a contract
        Task<(bool Success, string? ErrorMessage, AgreementUploadResultDto? Result)> UploadAgreementAsync(int contractId, IFormFile file);

        // Gets the saved agreement file information for download
        Task<(bool Success, string? ErrorMessage, StoredFileDto? File)> GetAgreementAsync(int contractId);

        Task<(bool Success, string? ErrorMessage)> UpdateContractStatusAsync(int id, ContractStatusUpdateDto statusDto);

        Task<bool> DeleteContractAsync(int id);
    }
}