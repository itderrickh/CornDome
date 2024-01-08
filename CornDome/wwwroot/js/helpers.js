function datasetToCard(dataset) {
    return {
        cardImage: dataset.cardImage,
        cardType: dataset.cardType,
        landscape: dataset.landscape,
        name: dataset.name,
        ability: dataset.ability,
        cost: dataset.cost,
        attack: dataset.attack,
        defense: dataset.defense,
        id: dataset.id
    };
}

function packString(arr) {
    const counts = {};
    arr.forEach(function (x) { counts[x] = (counts[x] || 0) + 1; });
    var combinedValues = Object.keys(counts).map((element) => `${element}:${counts[element]}`);
    return combinedValues.join(',');
}

function deckToQuery() {
    var heroString = deck.hero != null ? parseInt(deck.hero.id) : -1;
    var landscapeString = packString(deck.landscapes.map((x) => parseInt(x.id)));
    var cardString = packString(deck.cards.map((x) => parseInt(x.id)));
    var data = [heroString, landscapeString, cardString].join(';');

    if (history.pushState) {
        var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '?deck=' + btoa(data);
        window.history.pushState({ path: newurl }, '', newurl);
    }
}