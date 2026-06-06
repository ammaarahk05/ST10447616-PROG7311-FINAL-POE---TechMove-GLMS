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

        public async Task<(bool Success, string? ErrorMessage)> UpdateClientAsync(int id, ClientUpdateDto clientDto)
        {
            // Basic validation before sending changes to the database
            if (clientDto == null)
            {
                return (false, "Client data is required.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.Name))
            {
                return (false, "Client name is required.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.ContactDetails))
            {
                return (false, "Contact details are required.");
            }

            if (string.IsNullOrWhiteSpace(clientDto.Region))
            {
                return (false, "Region is required.");
            }

            var existingClient = await _clientRepository.GetByIdAsync(id);

            if (existingClient == null)
            {
                return (false, $"Client with ID {id} was not found.");
            }

            var client = new Client
            {
                Id = id,
                Name = clientDto.Name.Trim(),
                ContactDetails = clientDto.ContactDetails.Trim(),
                Region = clientDto.Region.Trim()
            };

            var updated = await _clientRepository.UpdateAsync(id, client);

            return updated
                ? (true, null)
                : (false, "Client could not be updated.");
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteClientAsync(int id)
        {
            // Deletes through the repository so the controller does not touch the database directly
            var deleted = await _clientRepository.DeleteAsync(id);

            return deleted
                ? (true, null)
                : (false, $"Client with ID {id} was not found.");
        }
    }
}