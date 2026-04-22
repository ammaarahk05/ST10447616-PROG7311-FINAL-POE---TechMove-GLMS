using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveLogisticSystem.Data;
using TechMoveLogisticSystem.Models;
using TechMoveLogisticSystem.Services;
using TechMoveLogisticSystem.Strategies;

namespace TechMoveLogisticSystem.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly CurrencyService _currencyService;

        // injects the DbContext and CurrencyService
        public ServiceRequestsController(AppDbContext context, CurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index()
        {
            var requests = await _context.ServiceRequests
    .Include(s => s.Contract)
    .ToListAsync();

            var rate = await _currencyService.GetUsdToZarRateAsync();

            var viewModel = requests.Select(r =>
            {
                var paymentContext = new PaymentContext();

                if (r.Contract.ServiceLevel == "International")
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
                    EstimatedUsd = r.Cost / rate
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

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            var rate = await _currencyService.GetUsdToZarRateAsync();
            ViewBag.Rate = rate;
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .ToList();

            ViewData["ContractId"] = new SelectList(
                contracts.Select(c => new
                {
                    Id = c.Id,
                    Display = $"Contract {c.Id} - {c.ServiceLevel} ({c.Client.Name})"
                }),
                "Id",
                "Display"
            );
            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Cost,Status,ContractId")] ServiceRequest serviceRequest)
        {
            // workflow validation

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "The selected contract does not exist.");
            }
            else
            {
                var status = contract.Status?.Trim().ToLower();

                if (status == "expired" || status == "on hold")
                {
                    ModelState.AddModelError("ContractId",
                        "BLOCKED! Cannot create a Service Request for an Expired or On Hold contract.");
                }
            }

            // stops early if invalid and avoids unnecessary API calls)
            if (!ModelState.IsValid)
            {
                ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);
                return View(serviceRequest);
            }

            // CURRENCY CONVERSION
            try
            {
                // gets the live USD to ZAR rate from exchangerate-api.com 
                decimal rate = await _currencyService.GetUsdToZarRateAsync();

                // convertc user input (USD) into ZAR
                serviceRequest.Cost = _currencyService.ConvertUsdToZar(serviceRequest.Cost, rate);

            }

            catch
            {
                // If API fails, it shows the user-friendly error
                ModelState.AddModelError("", "Currency conversion failed. Please try again later.");

                ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);
                return View(serviceRequest);
            }

            //saves to the database
            _context.Add(serviceRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            decimal rate = await _currencyService.GetUsdToZarRateAsync();

            // convert stored ZAR back to USD for display
            serviceRequest.Cost = Math.Round(serviceRequest.Cost / rate, 2);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            var contracts = _context.Contracts
                .Include(c => c.Client)
                .ToList();

            ViewData["ContractId"] = new SelectList(
                contracts.Select(c => new
                {
                    Id = c.Id,
                    Display = $"Contract {c.Id} - {c.ServiceLevel} ({c.Client.Name})"
                }),
                "Id",
                "Display",
                serviceRequest.ContractId
            );

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Cost,Status,ContractId")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.Id == serviceRequest.ContractId);

            if (contract == null)
            {
                ModelState.AddModelError("ContractId", "Selected contract does not exist.");
            }
            else
            {
                var status = contract.Status?.Trim().ToLower();

                if (status == "expired" || status == "on hold")
                {
                    ModelState.AddModelError("ContractId",
                        "BLOCKED! Cannot assign Service Request to an Expired or On Hold contract.");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);
                return View(serviceRequest);
            }

            try
            {
                decimal rate = await _currencyService.GetUsdToZarRateAsync();

                // convert edited USD to ZAR
                serviceRequest.Cost = _currencyService.ConvertUsdToZar(serviceRequest.Cost, rate);

                _context.Update(serviceRequest);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError("", "Currency conversion failed. Please try again later.");

                ViewData["ContractId"] = new SelectList(_context.Contracts, "Id", "Id", serviceRequest.ContractId);
                return View(serviceRequest);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);

            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.Id == id);
        }
    }
}