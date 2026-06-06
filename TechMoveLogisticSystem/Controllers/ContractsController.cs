using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveLogisticSystem.DTOs;
using TechMoveLogisticSystem.Models;
using TechMoveLogisticSystem.Services;

namespace TechMoveLogisticSystem.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IApiContractService _apiContractService;
        private readonly IApiClientService _apiClientService;

        public ContractsController(
            IApiContractService apiContractService,
            IApiClientService apiClientService)
        {
            _apiContractService = apiContractService;
            _apiClientService = apiClientService;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(string? statusFilter, DateTime? startDate, DateTime? endDate)
        {
            // MVC now gets contracts from the backend API instead of SQL directly
            var contractDtos = await _apiContractService.GetContractsAsync(statusFilter, startDate, endDate);

            var contracts = contractDtos
                .Select(MapToContractModel)
                .ToList();

            return View(contracts);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // MVC asks the API for the contract details
            var contractDto = await _apiContractService.GetContractByIdAsync(id.Value);

            if (contractDto == null)
            {
                return NotFound();
            }

            var contract = MapToContractModel(contractDto);

            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            await PopulateClientDropDownAsync();

            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            // These fields are not entered directly by the user on the form
            ModelState.Remove("Client");
            ModelState.Remove("ServiceRequests");
            ModelState.Remove("SignedAgreementPath");
            ModelState.Remove("SignedAgreementFileName");

            if (contract.SignedAgreementFile != null)
            {
                var extension = Path.GetExtension(contract.SignedAgreementFile.FileName).ToLowerInvariant();

                // I validate PDF on the MVC side before sending it to the API
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientDropDownAsync(contract.ClientId);
                return View(contract);
            }

            var createDto = new ContractCreateDto
            {
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = string.IsNullOrWhiteSpace(contract.Status) ? "Draft" : contract.Status,
                ServiceLevel = string.IsNullOrWhiteSpace(contract.ServiceLevel) ? "Standard" : contract.ServiceLevel,
                ClientId = contract.ClientId,
                SignedAgreementFileName = contract.SignedAgreementFile?.FileName ?? string.Empty
            };

            // MVC creates the contract through the backend API
            var createdContract = await _apiContractService.CreateContractAsync(createDto);

            if (createdContract == null)
            {
                ModelState.AddModelError(string.Empty, "The contract could not be created through the backend API.");
                await PopulateClientDropDownAsync(contract.ClientId);
                return View(contract);
            }

            // If a PDF was uploaded, MVC sends it to the API file upload endpoint
            if (contract.SignedAgreementFile != null)
            {
                var uploadResult = await _apiContractService.UploadAgreementAsync(
                    createdContract.Id,
                    contract.SignedAgreementFile);

                if (uploadResult == null)
                {
                    TempData["WarningMessage"] = "Contract was created, but the signed agreement could not be uploaded.";
                }
            }

            TempData["SuccessMessage"] = "Contract created successfully through the backend API.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // MVC gets the existing contract from the API
            var contractDto = await _apiContractService.GetContractByIdAsync(id.Value);

            if (contractDto == null)
            {
                return NotFound();
            }

            var contract = MapToContractModel(contractDto);

            await PopulateClientDropDownAsync(contract.ClientId);

            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return NotFound();
            }

            if (contract.SignedAgreementFile != null)
            {
                var extension = Path.GetExtension(contract.SignedAgreementFile.FileName).ToLowerInvariant();

                // I validate PDF on the MVC side before calling the API
                if (extension != ".pdf")
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientDropDownAsync(contract.ClientId);
                return View(contract);
            }

            // The current backend API supports status updates through PATCH
            var statusUpdated = await _apiContractService.UpdateContractStatusAsync(id, contract.Status);

            if (!statusUpdated)
            {
                ModelState.AddModelError(string.Empty, "The contract status could not be updated through the backend API.");
                await PopulateClientDropDownAsync(contract.ClientId);
                return View(contract);
            }

            // If a new agreement was uploaded, send it to the API
            if (contract.SignedAgreementFile != null)
            {
                var uploadResult = await _apiContractService.UploadAgreementAsync(id, contract.SignedAgreementFile);

                if (uploadResult == null)
                {
                    TempData["WarningMessage"] = "Status was updated, but the signed agreement could not be uploaded.";
                }
            }

            TempData["SuccessMessage"] = "Contract updated successfully through the backend API.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // MVC gets the contract from the API before showing the delete page
            var contractDto = await _apiContractService.GetContractByIdAsync(id.Value);

            if (contractDto == null)
            {
                return NotFound();
            }

            var contract = MapToContractModel(contractDto);

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // MVC deletes through the backend API instead of SQL directly
            var deleted = await _apiContractService.DeleteContractAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Contract deleted successfully through the backend API.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Contracts/DownloadAgreement/5
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            // MVC downloads the PDF through the backend API
            var fileResult = await _apiContractService.DownloadAgreementAsync(id);

            if (fileResult.FileBytes == null ||
                string.IsNullOrWhiteSpace(fileResult.ContentType) ||
                string.IsNullOrWhiteSpace(fileResult.FileName))
            {
                return NotFound("Signed agreement could not be downloaded.");
            }

            return File(fileResult.FileBytes, fileResult.ContentType, fileResult.FileName);
        }

        private async Task PopulateClientDropDownAsync(int? selectedClientId = null)
        {
            // MVC gets clients from the backend API for the dropdown
            var clients = await _apiClientService.GetClientsAsync();

            ViewData["ClientId"] = new SelectList(clients, "Id", "Name", selectedClientId);
        }

        private Contract MapToContractModel(ContractReadDto dto)
        {
            // I map API DTO data back into the existing MVC Contract model so my views can still work
            return new Contract
            {
                Id = dto.Id,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                ServiceLevel = dto.ServiceLevel,
                ClientId = dto.ClientId,
                SignedAgreementFileName = dto.SignedAgreementFileName,

                // This MVC action downloads the file from the API
                SignedAgreementPath = Url.Action(nameof(DownloadAgreement), "Contracts", new { id = dto.Id }) ?? string.Empty,

                Client = new Client
                {
                    Id = dto.ClientId,
                    Name = dto.ClientName,
                    Region = dto.ClientRegion,
                    ContactDetails = string.Empty
                }
            };
        }
    }
}