
using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CornDome.Pages
{
    public class CustomCardModel(ICustomCardRepository customCardRepository, UserRoleHelper userRoleHelper) : BasePageModel
    {
        public CustomCard? QueryCard { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            QueryCard = await customCardRepository.GetById(id);

            if (QueryCard == null)
                return NotFound();

            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);

            bool isOwner = userId > 0 && QueryCard.UserId == userId;
            bool isCardManager = userRoleHelper.IsCardManager(User);
            bool isApproved = QueryCard.ApprovedBy > 0;

            bool isPubliclyAccessible =
                isApproved && QueryCard.Visibility != Visibility.Invisible;

            if (!isOwner && !isCardManager && !isPubliclyAccessible)
                return NotFound();

            return Page();
        }
    }
}
