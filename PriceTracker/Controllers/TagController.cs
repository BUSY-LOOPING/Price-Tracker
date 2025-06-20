using Microsoft.AspNetCore.Mvc;
using PriceTracker.Interfaces;
using PriceTracker.Models;
using PriceTracker.Models.ViewModels;

namespace PriceTracker.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new TagViewModel();
            var tags = await _tagService.GetAllAsync();
            vm.Tags = tags.ToList();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }
            return PartialView("_DeleteConfirm", tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tagService.DeleteAsync(id);

            if (result.Status == ServiceResponse<TagDto>.ServiceStatus.Deleted)
            {
                TempData["SuccessMessage"] = "Tag deleted successfully.";
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
            return View(new NewTagViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewTagViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var tagDto = new CreateTagDto
            {
                Name = vm.Name,
                Description = vm.Description
            };


            var response = await _tagService.CreateAsync(tagDto);

            if (response.Status == ServiceResponse<TagDto>.ServiceStatus.Created)
            {
                TempData["SuccessMessage"] = "Tag created successfully!";
                return RedirectToAction("Index");
            }

            TempData["FailMessage"] = response.Messages.Count > 0
                ? string.Join("; ", response.Messages)
                : "There was an error creating the tag.";

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            var vm = new NewTagViewModel
            {

                TagId = tag.TagId,
                Name = tag.Name,
                Description = tag.Description
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

            var tagDto = new CreateTagDto
            {
                Name = vm.Name,
                Description = vm.Description
            };

            var result = await _tagService.UpdateAsync(id, tagDto);

            if (result.Status == ServiceResponse<TagDto>.ServiceStatus.Updated)
            {
                TempData["SuccessMessage"] = "Tag updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["FailMessage"] = result.Messages.Count > 0
                ? string.Join("; ", result.Messages)
                : "Failed to update Tag.";

            return View("New", vm);
        }

    }
}
