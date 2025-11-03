using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class AddModel(ICardRepository cardRepository, Config config) : PageModel
    {
        [BindProperty]
        public AddCard AddCard { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var card = new Card() { IsCustomCard = AddCard.IsCustomCard };
            var revision = new CardRevision()
            {
                Ability = AddCard.Ability,
                Attack = AddCard.Attack,
                Defense = AddCard.Defense,
                SetId = AddCard.SetId,
                TypeId = AddCard.TypeId,
                LandscapeId = AddCard.LandscapeId,
                Cost = AddCard.Cost,
                Name = AddCard.Name
            };
            var cardImage = new CardImage()
            {
                CardImageTypeId = 2,
                ImageUrl = $"upload/{AddCard.Name.Replace(" ", "_")}{Path.GetExtension(AddCard.Image.FileName)}"
            };

            using (var stream = System.IO.File.Create($"{config.AppData.UploadPath}/{AddCard.Name.Replace(" ", "_")}{Path.GetExtension(AddCard.Image.FileName)}"))
            {
                await AddCard.Image.CopyToAsync(stream);
            }

            cardRepository.AddCard(card, revision, cardImage);
            return RedirectToPage("/CardManage/Index");
        }
    }

    public class AddCard
    {
        public bool IsCustomCard { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Ability { get; set; }
        public int? SetId { get; set; }
        public int? LandscapeId { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
