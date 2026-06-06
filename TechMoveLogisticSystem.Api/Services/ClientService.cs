using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Models;
using TechMoveLogisticSystem.Api.Repositories;

namespace TechMoveLogisticSystem.Api.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<List<ClientReadDto>> GetClientsAsync()
        {
            return await _clientRepository.GetAllAsync();
        }

        public async Task<ClientReadDto?> GetClientByIdAsync(int id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        public async Task<(bool Success, string? ErrorMessage, ClientReadDto? Client)> CreateClientAsync(ClientCreateDto clientDto)
        {
            if (clientDto == null)
            {
                return (false, "Client data is required.", null);
            }

            if (string.IsNullOrWhiteSpace(clientDto.Name))
            {
                return (false, "Client name is required.", null);
            }

            if (string.IsNullOrWhiteSpace(clientDto.ContactDetails))
            {
                return (false, "Contact details are required.", null);
            }

            if (string.IsNullOrWhiteSpace(clientDto.Region))
            {
                return (false, "Region is required.", null);
            }

            var nameExists = await _clientRepository.ClientNameExistsAsync(clientDto.Name);

            if (nameExists)
            {
                return (false, "A client with this name already exists.", null);
            }

            var client = new Client
            {
                Name = clientDto.Name,
                ContactDetails = clientDto.ContactDetails,
                Region = clientDto.Region
            };

            var createdClient = await _clientRepository.CreateAsync(client);

            var readDto = await _clientRepository.GetByIdAsync(createdClient.Id);

            return (true, null, readDto);
        }
    }
}