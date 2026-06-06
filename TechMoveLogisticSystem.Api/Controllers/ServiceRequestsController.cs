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
    }
}