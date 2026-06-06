using Microsoft.AspNetCore.Http;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;
using TechMoveLogisticSystem.Api.Repositories;

namespace TechMoveLogisticSystem.Api.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IFileStorageService _fileStorageService;

        private readonly string[] _allowedStatuses =
        {
            "Draft",
            "Active",
            "Expired",
            "On Hold",
            "Approved",
            "Declined"
        };

        public ContractService(
    IContractRepository contractRepository,
    IFileStorageService fileStorageService)
        {
            _contractRepository = contractRepository;
            _fileStorageService = fileStorageService;
        }
            public async Task<(bool Success, string? ErrorMessage, AgreementUploadResultDto? Result)> UploadAgreementAsync(int contractId, IFormFile file)
        {
            var contract = await _contractRepository.GetEntityByIdAsync(contractId);

            if (contract == null)
            {
                return (false, $"Contract with ID {contractId} was not found.", null);
            }

            try
            {
                // I save the PDF using the file storage service so the controller stays clean
                var savedFileName = await _fileStorageService.SaveAgreementPdfAsync(file);

                contract.SignedAgreementFileName = file.FileName;
                contract.SignedAgreementPath = savedFileName;

                await _contractRepository.SaveChangesAsync();

                var result = new AgreementUploadResultDto
                {
                    ContractId = contract.Id,
                    OriginalFileName = file.FileName,
                    SavedFileName = savedFileName,
                    Message = "Signed agreement uploaded successfully."
                };

                return (true, null, result);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message, null);
            }
        }

        public async Task<(bool Success, string? ErrorMessage, StoredFileDto? File)> GetAgreementAsync(int contractId)
        {
            var contract = await _contractRepository.GetEntityByIdAsync(contractId);

            if (contract == null)
            {
                return (false, $"Contract with ID {contractId} was not found.", null);
            }

            if (string.IsNullOrWhiteSpace(contract.SignedAgreementPath))
            {
                return (false, "No signed agreement has been uploaded for this contract.", null);
            }

            var filePath = _fileStorageService.GetAgreementFilePath(contract.SignedAgreementPath);

            if (!System.IO.File.Exists(filePath))
            {
                return (false, "The signed agreement file could not be found on the server.", null);
            }

            var file = new StoredFileDto
            {
                FilePath = filePath,
                FileName = contract.SignedAgreementFileName ?? "SignedAgreement.pdf",
                ContentType = "application/pdf"
            };

            return (true, null, file);
        
        }

        public async Task<List<ContractReadDto>> GetContractsAsync(string? status, DateTime? startDate, DateTime? endDate)
        {
            return await _contractRepository.GetAllAsync(status, startDate, endDate);
        }

        public async Task<ContractReadDto?> GetContractByIdAsync(int id)
        {
            return await _contractRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage, ContractReadDto? Contract)> CreateContractAsync(ContractCreateDto contractDto)
        {
            if (contractDto == null)
            {
                return (false, "Contract data is required.", null);
            }

            if (contractDto.EndDate <= contractDto.StartDate)
            {
                return (false, "End date must be after start date.", null);
            }

            if (!_allowedStatuses.Contains(contractDto.Status))
            {
                return (false, "Invalid status value.", null);
            }

            var clientExists = await _contractRepository.ClientExistsAsync(contractDto.ClientId);

            if (!clientExists)
            {
                return (false, $"Client with ID {contractDto.ClientId} does not exist.", null);
            }

            var contract = new Contract
            {
                StartDate = contractDto.StartDate,
                EndDate = contractDto.EndDate,
                Status = contractDto.Status,
                ServiceLevel = contractDto.ServiceLevel,
                ClientId = contractDto.ClientId,
                SignedAgreementFileName = contractDto.SignedAgreementFileName
            };

            var createdContract = await _contractRepository.CreateAsync(contract);

            var readDto = await _contractRepository.GetByIdAsync(createdContract.Id);

            return (true, null, readDto);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateContractStatusAsync(int id, ContractStatusUpdateDto statusDto)
        {
            if (statusDto == null || string.IsNullOrWhiteSpace(statusDto.Status))
            {
                return (false, "Status is required.");
            }

            if (!_allowedStatuses.Contains(statusDto.Status))
            {
                return (false, "Invalid status value.");
            }

            var contract = await _contractRepository.GetEntityByIdAsync(id);

            if (contract == null)
            {
                return (false, $"Contract with ID {id} was not found.");
            }

            contract.Status = statusDto.Status;
            await _contractRepository.SaveChangesAsync();

            return (true, null);
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            return await _contractRepository.DeleteAsync(id);
        }
    }
}