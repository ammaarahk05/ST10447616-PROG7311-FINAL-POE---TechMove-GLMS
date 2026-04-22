using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveLogisticSystem.Data;
using TechMoveLogisticSystem.Models;
using TechMoveLogisticSystem.Factories;


namespace TechMoveLogisticSystem.Controllers
{
    public class ContractsController : Controller
    {
        private readonly AppDbContext _context;

        public ContractsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Contracts
        public async Task<IActionResult> Index(string statusFilter, DateTime? startDate, DateTime? endDate)
        {
            // base query- IQueryable for LINQ filtering in DB
            var contracts = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            // 1st filter: Status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                var normalizedStatus = statusFilter.Trim().ToLower();

                contracts = contracts.Where(c =>
                    c.Status.ToLower() == normalizedStatus);
            }

            // 2nd filter: Start Date
            if (startDate.HasValue)
            {
                contracts = contracts.Where(c =>
                    c.StartDate >= startDate.Value);
            }

            // 3rd filtre: End Date
            if (endDate.HasValue)
            {
                contracts = contracts.Where(c =>
                    c.EndDate <= endDate.Value);
            }

            // Execute query
            var result = await contracts.ToListAsync();

            return View(result);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        // POST: Contracts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Contract contract)
        {
            if (contract.SignedAgreementFile != null)
            {
                var extension = Path.GetExtension(contract.SignedAgreementFile.FileName);

                if (extension.ToLower() != ".pdf")
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
                else
                {
                    // Added UUID naming
                    var fileName = Guid.NewGuid().ToString() + extension;

                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await contract.SignedAgreementFile.CopyToAsync(stream);
                    }

                    contract.SignedAgreementFileName = 
                    contract.SignedAgreementFile.FileName;
                    contract.SignedAgreementPath = "/uploads/" + fileName;
                }
            }

            if (ModelState.IsValid)
            {
                IContractFactory factory;

                // simple condition 
                if (contract.ServiceLevel == "International")
                {
                    factory = new InternationalContractFactory();
                }
                else
                {
                    factory = new StandardContractFactory();
                }

                // creates a base contract using factory
                var newContract = factory.CreateContract();

                // copies the user input into the new contract
                newContract.ClientId = contract.ClientId;
                newContract.StartDate = contract.StartDate;
                newContract.EndDate = contract.EndDate;
                newContract.SignedAgreementPath = contract.SignedAgreementPath;
                newContract.SignedAgreementFileName = contract.SignedAgreementFileName;

                _context.Add(newContract);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null) return NotFound();

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);

            return View(contract);
        }

        // POST: Contracts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Contract contract)
        {
            if (id != contract.Id) return NotFound();

            var existingContract = await _context.Contracts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingContract == null) return NotFound();

            if (contract.SignedAgreementFile != null)
            {
                var extension = Path.GetExtension(contract.SignedAgreementFile.FileName);

                if (extension.ToLower() != ".pdf")
                {
                    ModelState.AddModelError("SignedAgreementFile", "Only PDF files are allowed.");
                }
                else
                {
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    var filePath = Path.Combine(uploadFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await contract.SignedAgreementFile.CopyToAsync(stream);
                    }

                    contract.SignedAgreementFileName = 
                    contract.SignedAgreementFile.FileName;
                    contract.SignedAgreementPath = "/uploads/" + fileName;
                }
            }
            else
            {
                contract.SignedAgreementPath = existingContract.SignedAgreementPath;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contract);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(contract.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract != null)
                _context.Contracts.Remove(contract);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }
    }
}