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

        // PUT: api/Clients/5
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClient(int id, ClientUpdateDto clientDto)
        {
            // Sends update request to service layer for validation and saving
            var result = await _clientService.UpdateClientAsync(id, clientDto);

            if (!result.Success)
            {
                if (result.ErrorMessage != null && result.ErrorMessage.Contains("was not found"))
                {
                    return NotFound(result.ErrorMessage);
                }

                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        // DELETE: api/Clients/5
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            // Deletes client through the API
            var result = await _clientService.DeleteClientAsync(id);

            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }

            return NoContent();
        }
    }
}