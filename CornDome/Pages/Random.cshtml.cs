using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages;

[IgnoreAntiforgeryToken]
[AllowAnonymous]
public class RandomModel(ICardRepository cardRepository) : BasePageModel
{
    private readonly ICardRepository _cardRepository = cardRepository;
    public Card QueryCard { get; set; } = null;
    public List<int> TypesToGenerate = [(int)CardTypeEnum.Creature, (int)CardTypeEnum.Spell, (int)CardTypeEnum.Building, (int)CardTypeEnum.Teamwork];

    public void OnGet()
    {
        var cards = _cardRepository.GetAll()
            .Where(x => x.IsCustomCard == false)
            .Where(x => TypesToGenerate.Contains(x.LatestRevision.TypeId)).ToList();
        var randomCard = cards[Random.Shared.Next(cards.Count)];

        var queryId = randomCard.Id;
        QueryCard = _cardRepository.GetCard(queryId);
    }
}