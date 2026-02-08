using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages;

[IgnoreAntiforgeryToken]
[AllowAnonymous]
public class RandomModel(ICardRepository cardRepository, Config config) : PageModel
{
    private readonly ICardRepository _cardRepository = cardRepository;
    public Card QueryCard { get; set; } = null;
    public string BaseUrl { get; set; } = config.BaseUrl;
    public List<int> TypesToGenerate = [(int)CardTypeEnum.Creature, (int)CardTypeEnum.Spell, (int)CardTypeEnum.Building, (int)CardTypeEnum.Teamwork];

    public void OnGet()
    {
        var cards = _cardRepository.GetAll()
            .Where(x => TypesToGenerate.Contains(x.LatestRevision.TypeId)).ToList();
        var randomCard = cards[Random.Shared.Next(cards.Count)];

        var queryId = randomCard.Id;
        QueryCard = _cardRepository.GetCard(queryId);
    }
}