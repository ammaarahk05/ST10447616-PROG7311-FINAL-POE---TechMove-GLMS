using Microsoft.AspNetCore.Mvc;
using TechMoveLogisticSystem.DTOs;
using TechMoveLogisticSystem.Services;

namespace TechMoveLogisticSystem.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IApiClientService _apiClientService;

        public ClientsController(IApiClientService apiClientService)
        {
            _apiClientService = apiClientService;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            // MVC now gets clients from the backend API instead of SQL directly
            var clients = await _apiClientService.GetClientsAsync();

            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Reads one client through the API
            var client = await _apiClientService.GetClientByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateDto client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            // Sends the new client to the API
            var result = await _apiClientService.CreateClientAsync(client);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Client could not be created.");
                return View(client);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Gets existing client from the API before editing
            var client = await _apiClientService.GetClientByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            var updateDto = new ClientUpdateDto
            {
                Name = client.Name,
                ContactDetails = client.ContactDetails,
                Region = client.Region
            };

            // Keeps the client ID available for the edit form post route
            ViewBag.ClientId = id.Value;

            return View(updateDto);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientUpdateDto client)
        {
            if (!ModelState.IsValid)
            {
                return View(client);
            }

            // Sends edited client details to the API
            var result = await _apiClientService.UpdateClientAsync(id, client);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Client could not be updated.");
                return View(client);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Shows client details before confirming delete
            var client = await _apiClientService.GetClientByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Deletes the client through the API
            var result = await _apiClientService.DeleteClientAsync(id);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Client could not be deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}