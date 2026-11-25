using CornDome.Helpers;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CornDome.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomCardController(Config configuration, ICustomCardRepository customCardRepository, UserRoleHelper userRoleHelper) : Controller
    {
        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var card = await customCardRepository.GetById(id);

            if (card == null)
                return NotFound();

            var isOwner = User.Identity?.IsAuthenticated == true &&
                          int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) &&
                          userId == card.UserId;

            var isAdmin = userRoleHelper.IsCardManager(User);

            var canView = card.Visibility switch
            {
                Visibility.Visible => isOwner || isAdmin || (card.ApprovedBy.HasValue && card.ApprovedBy > 0),
                Visibility.Limited => User.Identity?.IsAuthenticated == true,
                Visibility.Invisible => isOwner || isAdmin,
                _ => false
            };

            if (!canView)
                return NotFound();

            var imageDirectory = configuration.AppData.CustomPath;

            if (string.IsNullOrWhiteSpace(imageDirectory))
                return NotFound();

            var fileName = $"{id:N}.png";
            var imagePath = Path.Combine(imageDirectory, fileName);

            if (!System.IO.File.Exists(imagePath))
                return NotFound();

            return PhysicalFile(imagePath, "image/png");
        }
    }
}
