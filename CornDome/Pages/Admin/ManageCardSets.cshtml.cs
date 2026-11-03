using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class ManageCardSetsModel(ICardRepository cardRepository) : BasePageModel
    {
        public IEnumerable<CardSet> Sets { get; set; } = [];

        [BindProperty]
        [Required(ErrorMessage = "A set name is required.")]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        public int SetId { get; set; }

        public void OnGet()
        {
            LoadSets();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                LoadSets();
                return Page();
            }

            var result = cardRepository.AddSet(new CardSet
            {
                Description = Description.Trim()
            });

            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Set added successfully." : "Failed to add set.";

            return RedirectToPage();
        }

        public IActionResult OnPostUpdate()
        {
            if (SetId <= 0 || string.IsNullOrWhiteSpace(Description))
            {
                TempData["ErrorMessage"] = "A valid set name is required.";
                return RedirectToPage();
            }

            var result = cardRepository.UpdateSet(new CardSet
            {
                Id = SetId,
                Description = Description.Trim()
            });

            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Set updated successfully." : "Failed to update set.";

            return RedirectToPage();
        }

        public IActionResult OnPostDelete()
        {
            if (SetId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid set.";
                return RedirectToPage();
            }

            var result = cardRepository.DeleteSet(SetId);

            TempData[result ? "SuccessMessage" : "ErrorMessage"] =
                result ? "Set deleted successfully." : "Failed to delete set.";

            return RedirectToPage();
        }

        private void LoadSets()
        {
            Sets = cardRepository.GetAllSets()
                .OrderBy(x => x.Description)
                .ToList();
        }
    }
}