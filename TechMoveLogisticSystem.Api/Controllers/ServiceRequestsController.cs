using Microsoft.AspNetCore.Mvc;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TechMoveLogisticSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService)
        {
            _serviceRequestService = serviceRequestService;
        }

        // GET: api/ServiceRequests
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ServiceRequestReadDto>>> GetServiceRequests()
        {
            var serviceRequests = await _serviceRequestService.GetServiceRequestsAsync();

            return Ok(serviceRequests);
        }

        // GET: api/ServiceRequests/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceRequestReadDto>> GetServiceRequest(int id)
        {
            var serviceRequest = await _serviceRequestService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound($"Service request with ID {id} was not found.");
            }

            return Ok(serviceRequest);
        }

        // POST: api/ServiceRequests
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ServiceRequestReadDto>> CreateServiceRequest(ServiceRequestCreateDto requestDto)
        {
            var result = await _serviceRequestService.CreateServiceRequestAsync(requestDto);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(
                nameof(GetServiceRequest),
                new { id = result.ServiceRequest!.Id },
                result.ServiceRequest);
        }

        // PUT: api/ServiceRequests/5
        // Protected because updating a service request should require login
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateServiceRequest(int id, ServiceRequestUpdateDto dto)
        {
            // Sends the update to the service layer so backend rules are still enforced
            var result = await _serviceRequestService.UpdateAsync(id, dto);

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

        // DELETE: api/ServiceRequests/5
        // Protected because deleting a service request should also require login
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            // Deletes a service request through the API
            var deleted = await _serviceRequestService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound($"Service request with ID {id} was not found.");
            }

            return NoContent();
        }
    }
}