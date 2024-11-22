document.getElementById("cardTypeFilter").onchange = function () {
    filterFunctions.cardType = this.value;
    filterDataset();
};

document.getElementById("landscapeFilter").onchange = function () {
    filterFunctions.landscape = this.value;
    filterDataset();
};

document.getElementById("costFilter").oninput = function () {
    filterFunctions.cost = this.value !== "" ? parseInt(this.value) : null;
    filterDataset();
};

document.getElementById("attackFilter").oninput = function () {
    filterFunctions.attack = this.value !== "" ? parseInt(this.value) : null;
    filterDataset();
};

document.getElementById("defenseFilter").oninput = function () {
    filterFunctions.defense = this.value !== "" ? parseInt(this.value) : null;
    filterDataset();
};

document.getElementById("abilityFilter").oninput = function () {
    filterFunctions.ability = this.value;
    filterDataset();
};

document.getElementById("nameFilter").oninput = function () {
    filterFunctions.name = this.value;
    filterDataset();
};

document.getElementById("validateButton").onclick = function () {
    function mapToProp(data, prop) {
        return data
            .reduce((res, item) => Object
                .assign(res, {
                    [item[prop]]: 1 + (res[item[prop]] || 0)
                }), Object.create(null))
            ;
    }

    let warnings = [];
    let errors = [];
    let validationSummary = "";

    if (deck.hero === null)
        warnings.push("Deck list does not have a Hero card.")

    if (deck.cards.length < 40) 
        errors.push("Deck does not have the minimum amount of cards (40).")

    if (deck.landscapes.length < 4)
        errors.push("Deck does not have the mimimum amount of Landscapes (4).")

    let collapsedCards = mapToProp(deck.cards, 'name');
    for (let card in collapsedCards) {
        if (collapsedCards[card] > 3) {
            errors.push("A maximum of 3 copies of a card can be in the deck (violation: " + card + ")");
        }
    }

    if (errors.length === 0 && warnings.length === 0) {
        validationSummary = "The deck is valid!";
    } else {
        if (warnings.length > 0) {
            validationSummary += "Warnings:\n";
            for (let warning of warnings) {
                validationSummary += warning + "\n";
            }
            validationSummary += "\n";
        }

        if (errors.length > 0) {
            validationSummary += "Errors:\n";
            for (let err of errors) {
                validationSummary += err + "\n";
            }
            validationSummary += "\n";
        }
    }

    alert(validationSummary);
};

document.getElementById("decklistExport").onclick = function () {
    var landscapes = {};
    for (var landscape of deck.landscapes) {
        if (!landscapes.hasOwnProperty(landscape.name))
            landscapes[landscape.name] = 1;
        else
            landscapes[landscape.name]++;
    }

    var creatures = {};
    for (var creature of deck.cards.filter((x) => x.cardType === "Creature")) {
        if (!creatures.hasOwnProperty(creature.name))
            creatures[creature.name] = 1;
        else
            creatures[creature.name]++;
    }

    var spells = {};
    for (var spell of deck.cards.filter((x) => x.cardType === "Spell")) {
        if (!spells.hasOwnProperty(spell.name))
            spells[spell.name] = 1;
        else
            spells[spell.name]++;
    }

    var buildings = {};
    for (var building of deck.cards.filter((x) => x.cardType === "Building")) {
        if (!buildings.hasOwnProperty(building.name))
            buildings[building.name] = 1;
        else
            buildings[building.name]++;
    }

    var template = "Hero\n";
    if (deck.hero !== null) {
        template += "* " + deck.hero.name + '\n\n';
    } else {
        template += "\n";
    }
    template += `Landscapes\n`

    Object.keys(landscapes).forEach(x => template += `* ${landscapes[x]} - ${x}\n`);
    template += "\nCreatures\n";
    Object.keys(creatures).forEach(x => template += `* ${creatures[x]} - ${x}\n`);
    template += "\nSpells\n";
    Object.keys(spells).forEach(x => template += `* ${spells[x]} - ${x}\n`);
    template += "\nBuildings\n";
    Object.keys(buildings).forEach(x => template += `* ${buildings[x]} - ${x}\n`);
    
    navigator.clipboard.writeText(template);
    alert('Copied to clipboard!');
};

document.getElementById("permalinkExport").onclick = function () {
    var newURL = window.location.protocol + "//" + window.location.host + "/Deck" + window.location.search
    navigator.clipboard.writeText(newURL);
    alert('Copied to clipboard!');
};

document.getElementById("tryoutButton").onclick = function () {
    var newURL = window.location.protocol + "//" + window.location.host + "/Goldfish" + window.location.search;
    window.location.href = newURL;
}

var addButtons = document.querySelectorAll('.add');
addButtons.forEach(x => x.onclick = function () {
    var dataset = x.parentElement.parentElement.dataset;
    var cardElement = {
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

    if (cardElement.cardType === "Hero") {
        deck.hero = cardElement;
        renderHero();
    }
    else if (cardElement.cardType === "Landscape" && deck.landscapes.length < 4) {
        deck.landscapes.push(cardElement);
        renderLandscapes();
    }
    else if (cardElement.cardType !== "Landscape" && cardElement.cardType !== "Hero") {
        deck.cards.push(cardElement);
        renderCards();
    }
    renderData();
});

function setupRemoveClicks() {
    var removeButtons = document.querySelectorAll('.remove');
    removeButtons.forEach(x => x.onclick = function () {
        var dataset = x.parentElement.dataset;

        if (dataset.cardType === "Hero") {
            deck.hero = null;
            renderHero();
        }
        else if (dataset.cardType === "Landscape") {
            var index = deck.landscapes.map(function (e) { return e.id; }).indexOf(dataset.id);
            deck.landscapes.splice(index, 1);
            renderLandscapes();
        }
        else {
            var index = deck.cards.map(function (e) { return e.id; }).indexOf(dataset.id);
            deck.cards.splice(index, 1);
            renderCards();
        }
        renderData();
    });
}