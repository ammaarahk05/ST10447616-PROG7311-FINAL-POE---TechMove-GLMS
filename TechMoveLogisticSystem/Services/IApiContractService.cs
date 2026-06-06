using TechMoveLogisticSystem.DTOs;

namespace TechMoveLogisticSystem.Services
{
    public interface IApiContractService
    {
        // These methods allow MVC to work with contracts through the backend API
        Task<List<ContractReadDto>> GetContractsAsync(string? status, DateTime? startDate, DateTime? endDate);

        Task<ContractReadDto?> GetContractByIdAsync(int id);

        Task<ContractReadDto?> CreateContractAsync(ContractCreateDto contractDto);

        Task<bool> UpdateContractStatusAsync(int id, string status);

        Task<bool> DeleteContractAsync(int id);

        // These methods allow MVC to upload and download signed agreements through the API
        Task<AgreementUploadResultDto?> UploadAgreementAsync(int contractId, IFormFile file);

        Task<(byte[]? FileBytes, string? ContentType, string? FileName)> DownloadAgreementAsync(int contractId);
    }
}