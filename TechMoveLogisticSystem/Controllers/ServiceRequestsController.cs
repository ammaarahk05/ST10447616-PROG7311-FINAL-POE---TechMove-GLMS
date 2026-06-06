using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveLogisticSystem.DTOs;
using TechMoveLogisticSystem.Models;
using TechMoveLogisticSystem.Services;
using TechMoveLogisticSystem.Strategies;

namespace TechMoveLogisticSystem.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IApiServiceRequestService _apiServiceRequestService;
        private readonly IApiContractService _apiContractService;
        private readonly CurrencyService _currencyService;

        public ServiceRequestsController(
            IApiServiceRequestService apiServiceRequestService,
            IApiContractService apiContractService,
            CurrencyService currencyService)
        {
            _apiServiceRequestService = apiServiceRequestService;
            _apiContractService = apiContractService;
            _currencyService = currencyService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            // MVC now gets service requests from the backend API instead of SQL directly
            var serviceRequestDtos = await _apiServiceRequestService.GetServiceRequestsAsync();

            // Gets contract data from API so the MVC table can still show service level
            var contracts = await _apiContractService.GetContractsAsync(null, null, null);

            var requests = serviceRequestDtos
                .Select(dto => MapToServiceRequestModel(dto, contracts))
                .ToList();

            var rate = await _currencyService.GetUsdToZarRateAsync();

            var viewModel = requests.Select(r =>
            {
                var paymentContext = new PaymentContext();

                if (r.Contract != null && r.Contract.ServiceLevel == "International")
                {
                    paymentContext.SetStrategy(new InternationalPaymentStrategy());
                }
                else
                {
                    paymentContext.SetStrategy(new MonthlyPaymentStrategy());
                }

                var calculated = paymentContext.ExecutePayment((double)r.Cost);

                return new
                {
                    Request = r,
                    Calculated = calculated,
                    EstimatedUsd = rate > 0 ? r.Cost / rate : 0
                };
            }).ToList();

            ViewBag.ProcessedRequests = viewModel;

            return View(requests);
        }

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequestDto = await _apiServiceRequestService.GetServiceRequestByIdAsync(id.Value);

            if (serviceRequestDto == null)
            {
                return NotFound();
            }

            var contracts = await _apiContractService.GetContractsAsync(null, null, null);

            var serviceRequest = MapToServiceRequestModel(serviceRequestDto, contracts);

            var rate = await _currencyService.GetUsdToZarRateAsync();

            ViewBag.Rate = rate;

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create()
        {
            await PopulateContractDropDownAsync();

            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequest serviceRequest)
        {
            ModelState.Remove("Contract");

            if (string.IsNullOrWhiteSpace(serviceRequest.Description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }

            if (serviceRequest.Cost <= 0)
            {
                ModelState.AddModelError("Cost", "Cost must be a positive value.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateContractDropDownAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            decimal rate;

            try
            {
                // User enters USD in MVC, then MVC converts to ZAR before sending to API
                rate = await _currencyService.GetUsdToZarRateAsync();
                serviceRequest.Cost = _currencyService.ConvertUsdToZar(serviceRequest.Cost, rate);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Currency conversion failed. Please try again later.");
                await PopulateContractDropDownAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var createDto = new ServiceRequestCreateDto
            {
                Description = serviceRequest.Description,
                Cost = serviceRequest.Cost,
                Status = string.IsNullOrWhiteSpace(serviceRequest.Status) ? "Pending" : serviceRequest.Status,
                ContractId = serviceRequest.ContractId
            };

            // Sends the service request to the backend API
            var result = await _apiServiceRequestService.CreateServiceRequestAsync(createDto);

            if (!result.Success)
            {
                AddApiErrorToModelState(result.ErrorMessage);
                await PopulateContractDropDownAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            TempData["SuccessMessage"] = "Service request created successfully through the backend API.";

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequestDto = await _apiServiceRequestService.GetServiceRequestByIdAsync(id.Value);

            if (serviceRequestDto == null)
            {
                return NotFound();
            }

            var rate = await _currencyService.GetUsdToZarRateAsync();

            var serviceRequest = new ServiceRequest
            {
                Id = serviceRequestDto.Id,
                Description = serviceRequestDto.Description,
                Cost = rate > 0 ? Math.Round(serviceRequestDto.Cost / rate, 2) : serviceRequestDto.Cost,
                Status = serviceRequestDto.Status,
                ContractId = serviceRequestDto.ContractId
            };

            await PopulateContractDropDownAsync(serviceRequest.ContractId);

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Contract");

            if (string.IsNullOrWhiteSpace(serviceRequest.Description))
            {
                ModelState.AddModelError("Description", "Description is required.");
            }

            if (serviceRequest.Cost <= 0)
            {
                ModelState.AddModelError("Cost", "Cost must be a positive value.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateContractDropDownAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            try
            {
                // User edits USD in MVC, then MVC converts to ZAR before sending to API
                var rate = await _currencyService.GetUsdToZarRateAsync();
                serviceRequest.Cost = _currencyService.ConvertUsdToZar(serviceRequest.Cost, rate);
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Currency conversion failed. Please try again later.");
                await PopulateContractDropDownAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            var updateDto = new ServiceRequestUpdateDto
            {
                Description = serviceRequest.Description,
                Cost = serviceRequest.Cost,
                Status = serviceRequest.Status,
                ContractId = serviceRequest.ContractId
            };

            // Sends the update through the backend API
            var result = await _apiServiceRequestService.UpdateServiceRequestAsync(id, updateDto);

            if (!result.Success)
            {
                AddApiErrorToModelState(result.ErrorMessage);
                await PopulateContractDropDownAsync(serviceRequest.ContractId);
                return View(serviceRequest);
            }

            TempData["SuccessMessage"] = "Service request updated successfully through the backend API.";

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequestDto = await _apiServiceRequestService.GetServiceRequestByIdAsync(id.Value);

            if (serviceRequestDto == null)
            {
                return NotFound();
            }

            var contracts = await _apiContractService.GetContractsAsync(null, null, null);

            var serviceRequest = MapToServiceRequestModel(serviceRequestDto, contracts);

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Deletes through the backend API instead of SQL directly
            var result = await _apiServiceRequestService.DeleteServiceRequestAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Service request could not be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateContractDropDownAsync(int? selectedContractId = null)
        {
            // MVC gets contracts from the backend API for the dropdown
            var contracts = await _apiContractService.GetContractsAsync(null, null, null);

            ViewData["ContractId"] = new SelectList(
                contracts.Select(c => new
                {
                    Id = c.Id,
                    Display = $"Contract {c.Id} - {c.ServiceLevel} - {c.ClientName} - {c.Status}"
                }),
                "Id",
                "Display",
                selectedContractId
            );
        }

        private ServiceRequest MapToServiceRequestModel(
            ServiceRequestReadDto dto,
            List<ContractReadDto> contracts)
        {
            var matchingContract = contracts.FirstOrDefault(c => c.Id == dto.ContractId);

            return new ServiceRequest
            {
                Id = dto.Id,
                Description = dto.Description,
                Cost = dto.Cost,
                Status = dto.Status,
                ContractId = dto.ContractId,
                Contract = new Contract
                {
                    Id = dto.ContractId,
                    Status = dto.ContractStatus,
                    ServiceLevel = matchingContract?.ServiceLevel ?? "Standard",
                    Client = new Client
                    {
                        Name = dto.ClientName,
                        ContactDetails = string.Empty,
                        Region = matchingContract?.ClientRegion ?? string.Empty
                    }
                }
            };
        }

        private void AddApiErrorToModelState(string? errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                ModelState.AddModelError(string.Empty, "The API rejected the request.");
                return;
            }

            if (errorMessage.Contains("Expired") || errorMessage.Contains("On Hold"))
            {
                ModelState.AddModelError("ContractId", "BLOCKED! Cannot create or assign a Service Request for an Expired or On Hold contract.");
            }
            else if (errorMessage.Contains("Description"))
            {
                ModelState.AddModelError("Description", errorMessage);
            }
            else if (errorMessage.Contains("Cost"))
            {
                ModelState.AddModelError("Cost", errorMessage);
            }
            else
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
        }
    }
}