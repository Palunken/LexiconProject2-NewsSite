using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class SubscriptionTypesController : Controller
    {
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly ILogger<SubscriptionTypesController> _logger;

        public SubscriptionTypesController(ISubscriptionTypeService subscriptionTypeService, ILogger<SubscriptionTypesController> logger)
        {
            _subscriptionTypeService = subscriptionTypeService;
            _logger = logger;
        }
               
        public async Task<IActionResult> Index()
        {
            var subTypes = await _subscriptionTypeService.GetAllSubscriptionTypes();
            var viewModel = new SubscriptionManagementVM
            {
                SubscriptionTypes = subTypes
            };

            return View(viewModel);
        }
                
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("SubscriptionType ID is required.");
            }

            var subType = await _subscriptionTypeService.GetByIdAsync(id.Value);
            if (subType == null)
            {
                return NotFound($"SubscriptionType with ID {id.Value} not found.");
            }

            return View(subType);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new SubscriptionManagementVM();
            return View(viewModel);
        }
               
        [HttpPost]        
        public async Task<IActionResult> Create(SubscriptionManagementVM viewModel)
        {
            if (ModelState.IsValid)
            {
                await _subscriptionTypeService.Create(viewModel.NewSubscriptionType);
                return RedirectToAction(nameof(Index));
            }

            // Reload the list of subscription types
            viewModel.SubscriptionTypes = await _subscriptionTypeService.GetAllSubscriptionTypes();
            return View(viewModel);
        }
                
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("SubscriptionType ID is required.");
            }

            var subType = await _subscriptionTypeService.GetByIdAsync(id.Value);
            if (subType == null)
            {
                return NotFound($"SubscriptionType with ID {id.Value} not found.");
            }
            return View(subType);
        }
                
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TypeName,Description,Price")] SubscriptionType subType)
        {
            if (id != subType.Id)
            {
                return BadRequest($"Mismatched ID. The provided ID {id} does not match SubscriptionType ID {subType.Id}.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _subscriptionTypeService.Edit(subType);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await _subscriptionTypeService.Exists(subType.Id))
                    {
                        return NotFound($"SubscriptionType with ID {subType.Id} does not exist");
                    }
                    else
                    {
                        // Log the exception with additional details
                        _logger.LogError(ex, "Concurrency conflict occurred while updating SubscriptionType with ID {SubscriptionTypeId}", subType.Id);

                        // Re-throw the original exception
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subType);
        }
                       
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _subscriptionTypeService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException)
            {
                return NotFound($"SubscriptionType with ID {id} not found.");
            }
        }

        public async Task<IActionResult> GetDetails(int id)
        {
            var subType = await _subscriptionTypeService.GetByIdAsync(id);
            if (subType == null)
            {
                return NotFound($"SubscriptionType with ID {id} not found.");
            }

            return PartialView("_SubscriptionTypeDetails", subType);
        }
    }
}
