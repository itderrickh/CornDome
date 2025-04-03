var template = function (card) {
    var element = document.createElement("div");
    element.classList += "card deck-card";

    element.dataset.ability = card.ability;
    element.dataset.cardImage = card.cardImage;
    element.dataset.cardType = card.cardType;
    element.dataset.landscape = card.landscape;
    element.dataset.name = card.name;
    element.dataset.ability = card.ability;
    element.dataset.cost = card.cost;
    element.dataset.attack = card.attack;
    element.dataset.defense = card.defense;
    element.dataset.id = card.id;

    element.innerHTML = `<img src="${card.cardImage}" alt="${card.name}" style="width: 100%" /><div class="card-overlay remove"></div>`;
    return element;
}

function renderHero() {
    var heroDrop = document.getElementById("deck-hero");
    heroDrop.innerHTML = "";
    if (deck.hero !== null) {
        heroDrop.appendChild(template(deck.hero));
        setupRemoveClicks();
    }
    deckToQuery();
}

function renderLandscapes() {
    var landDrop = document.getElementById("deck-landscapes");
    landDrop.innerHTML = "";
    for (var ls of deck.landscapes) {
        landDrop.appendChild(template(ls));
    }
    setupRemoveClicks();
    deckToQuery();
}

function renderCards() {
    var cardDrop = document.getElementById("deck-cards");
    cardDrop.innerHTML = "";
    for (var cd of deck.cards) {
        cardDrop.appendChild(template(cd));
    }
    setupRemoveClicks();
    deckToQuery();
}

function renderData() {
    var totalStatField = document.getElementById('total-stat');
    var heroesStatField = document.getElementById('heroes-stat');
    var landscapesStatField = document.getElementById('landscapes-stat');
    var creatureStatField = document.getElementById('creature-stat');
    var spellStatField = document.getElementById('spell-stat');
    var buildingStatField = document.getElementById('building-stat');

    heroesStatField.innerHTML = deck.hero !== null ? 1 : 0;
    landscapesStatField.innerHTML = deck.landscapes.length;

    var creatureCount = deck.cards.filter((x) => x.cardType === CardType.Creature).length;
    creatureStatField.innerHTML = creatureCount;
    var spellCount = deck.cards.filter((x) => x.cardType === CardType.Spell).length;
    spellStatField.innerHTML = spellCount;
    var buildingCount = deck.cards.filter((x) => x.cardType === CardType.Building).length;
    buildingStatField.innerHTML = buildingCount;

    totalStatField.innerHTML = creatureCount + spellCount + buildingCount;

    var bluePlainsStatField = document.getElementById('bp-stat');
    var cornfieldStatField = document.getElementById('cf-stat');
    var uselessSwampStatField = document.getElementById('us-stat');
    var nicelandsStatField = document.getElementById('nl-stat');
    var sandylandsStatField = document.getElementById('sl-stat');
    var icylandsStatField = document.getElementById('il-stat');
    var rainbowStatField = document.getElementById('rb-stat');
    var lavaflatsStatField = document.getElementById('lf-stat');

    bluePlainsStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.BluePlains).length;
    cornfieldStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.CornFields).length;
    uselessSwampStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.UselessSwamp).length;
    nicelandsStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.NiceLands).length;
    sandylandsStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.SandyLands).length;
    icylandsStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.IcyLands).length;
    rainbowStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.Rainbow).length;
    lavaflatsStatField.innerHTML = deck.cards.filter((x) => x.landscape === Landscape.LavaFlats).length;
}

function buildDeckIfExistsInQuery() {
    var heroBlock = document.querySelectorAll('#deck-hero .card-container');
    if (heroBlock.length != 0) {
        deck.hero = datasetToCard(heroBlock[0].dataset);
    }

    var landscapeBlocks = document.querySelectorAll('#deck-landscapes .card-container');
    for (let i = 0; i < landscapeBlocks.length; i++) {
        var landscape = landscapeBlocks[i];
        var dataset = landscape.dataset;
        deck.landscapes.push(datasetToCard(dataset));
    }

    var cardBlocks = document.querySelectorAll('#deck-cards .card-container');
    for (let i = 0; i < cardBlocks.length; i++) {
        var card = cardBlocks[i];
        var dataset = card.dataset;
        deck.cards.push(datasetToCard(dataset));
    }

    renderHero();
    renderLandscapes();
    renderCards();
    renderData();
}