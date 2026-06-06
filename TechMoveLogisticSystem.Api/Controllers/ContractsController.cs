using Microsoft.AspNetCore.Mvc;
using TechMoveLogisticSystem.Api.DTOs;
using TechMoveLogisticSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace TechMoveLogisticSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        // GET: api/contracts
        // Example: api/contracts?status=Active&startDate=2026-01-01&endDate=2026-12-31
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ContractReadDto>>> GetContracts(
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = await _contractService.GetContractsAsync(status, startDate, endDate);

            return Ok(contracts);
        }

        // GET: api/contracts/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ContractReadDto>> GetContract(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound($"Contract with ID {id} was not found.");
            }

            return Ok(contract);
        }

        // POST: api/contracts
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ContractReadDto>> CreateContract(ContractCreateDto contractDto)
        {
            var result = await _contractService.CreateContractAsync(contractDto);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(
                nameof(GetContract),
                new { id = result.Contract!.Id },
                result.Contract);
        }

        // PATCH: api/contracts/5/status
        [Authorize]
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateContractStatus(int id, ContractStatusUpdateDto statusDto)
        {
            var result = await _contractService.UpdateContractStatusAsync(id, statusDto);

            if (!result.Success)
            {
                if (result.ErrorMessage != null && result.ErrorMessage.Contains("not found"))
                {
                    return NotFound(result.ErrorMessage);
                }

                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        // DELETE: api/contracts/5
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var deleted = await _contractService.DeleteContractAsync(id);

            if (!deleted)
            {
                return NotFound($"Contract with ID {id} was not found.");
            }

            return NoContent();
        }

        // POST: api/Contracts/5/agreement
        [Authorize]
        [HttpPost("{id}/agreement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AgreementUploadResultDto>> UploadAgreement(int id, IFormFile file)
        {
            var result = await _contractService.UploadAgreementAsync(id, file);

            if (!result.Success)
            {
                if (result.ErrorMessage != null && result.ErrorMessage.Contains("not found"))
                {
                    return NotFound(result.ErrorMessage);
                }

                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Result);
        }

        // GET: api/Contracts/5/agreement
        [HttpGet("{id}/agreement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var result = await _contractService.GetAgreementAsync(id);

            if (!result.Success)
            {
                if (result.ErrorMessage != null && result.ErrorMessage.Contains("not found"))
                {
                    return NotFound(result.ErrorMessage);
                }

                return BadRequest(result.ErrorMessage);
            }

            // i''ll return the actual PDF file as a download response
            var fileBytes = await System.IO.File.ReadAllBytesAsync(result.File!.FilePath);

            return File(fileBytes, result.File.ContentType, result.File.FileName);
        }
    }
}