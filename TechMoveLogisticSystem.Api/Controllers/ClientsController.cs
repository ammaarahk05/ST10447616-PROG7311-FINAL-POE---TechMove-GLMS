using Microsoft.AspNetCore.Mvc;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TechMoveLogisticSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: api/Clients
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClientReadDto>>> GetClients()
        {
            var clients = await _clientService.GetClientsAsync();

            return Ok(clients);
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClientReadDto>> GetClient(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound($"Client with ID {id} was not found.");
            }

            return Ok(client);
        }

        // POST: api/Clients
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ClientReadDto>> CreateClient(ClientCreateDto clientDto)
        {
            var result = await _clientService.CreateClientAsync(clientDto);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(
                nameof(GetClient),
                new { id = result.Client!.Id },
                result.Client);
        }
    }
}