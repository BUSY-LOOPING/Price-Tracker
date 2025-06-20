using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;

namespace PriceTracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new CategoryViewModel();
            var categories = await _categoryService.GetAllAsync();
            vm.Categories = categories.ToList();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteConfirm", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);

            if (result.Status == ServiceResponse<CategoryDto>.ServiceStatus.Deleted)
            {
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else
            {
                TempData["FailMessage"] = result.Messages.Count > 0
                    ? string.Join("; ", result.Messages)
                    : "Failed to delete tag.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult New()
        {
            return View(new NewCategoryViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewCategoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var categoryDto = new CreateCategoryDto
            {
                Name = vm.Name,
                Description = vm.Description
            };


            var response = await _categoryService.CreateAsync(categoryDto);

            if (response.Status == ServiceResponse<CategoryDto>.ServiceStatus.Created)
            {
                TempData["SuccessMessage"] = "Category created successfully!";
                return RedirectToAction("Index");
            }

            TempData["FailMessage"] = response.Messages.Count > 0
                ? string.Join("; ", response.Messages)
                : "There was an error creating the category.";

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var vm = new NewCategoryViewModel
            {

                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description
            };
            return View("New", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NewTagViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("New", vm);
            }

            var categoryDto = new CreateCategoryDto
            {
                Name = vm.Name,
                Description = vm.Description
            };

            var result = await _categoryService.UpdateAsync(id, categoryDto);

            if (result.Status == ServiceResponse<CategoryDto>.ServiceStatus.Updated)
            {
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["FailMessage"] = result.Messages.Count > 0
                ? string.Join("; ", result.Messages)
                : "Failed to update Category.";

            return View("New", vm);
        }

    }
}
