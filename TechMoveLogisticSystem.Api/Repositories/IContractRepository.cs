using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;

namespace TechMoveLogisticSystem.Api.Repositories
{
    public interface IContractRepository
    {
        Task<List<ContractReadDto>> GetAllAsync(string? status, DateTime? startDate, DateTime? endDate);

        Task<ContractReadDto?> GetByIdAsync(int id);

        Task<Contract?> GetEntityByIdAsync(int id);

        Task<bool> ClientExistsAsync(int clientId);

        Task<Contract> CreateAsync(Contract contract);

        Task<bool> DeleteAsync(int id);

        Task SaveChangesAsync();
    }
}