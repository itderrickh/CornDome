const cardMenu = document.getElementById('cardMenu');
const deckMenu = document.getElementById('deckMenu');
const discardMenu = document.getElementById('discardMenu');
const spellzoneMenu = document.getElementById('spellzoneMenu');
const discardSubMenu = document.getElementById('discardSubMenu');
const deckSubMenu = document.getElementById('deckSubMenu');

var deck = JSON.parse(document.getElementById('deck-data').textContent);
var zones = {
    hand: document.getElementById('hand'),
    discard: document.getElementById('discard'),
    deck: document.getElementById('deck'),
    spellZone: document.getElementById('spellzone')
};

var applicationState = {
    hand: [],
    discard: [],
    deck: [],
    field: {
        spellZone: null,
        landscape1: {
            card: null
        },
        building1: {
            data: null
        },
        landscape2: {
            card: null
        },
        building2: {
            data: null
        },
        landscape3: {
            card: null
        },
        building3: {
            data: null
        },
        landscape4: {
            card: null
        },
        building4: {
            data: null
        },
    }
};

function render() {
    var deckSize = document.getElementById('deck-size');

    deckSize.textContent = applicationState.deck.length;
    if (applicationState.deck.length === 0) {
        document.querySelector('#deck .cardslot img').src = "";
    }

    zones.hand.innerHTML = "";
    for (let handCard of applicationState.hand) {
        let template = handCardTemplate(handCard);
        zones.hand.insertAdjacentHTML("beforeend", template);
    }

    if (applicationState.field.spellZone !== null) {
        var spellzone = document.querySelector("#spellzone .cardslot");
        spellzone.innerHTML = "";
        let template = handCardTemplate(applicationState.field.spellZone);
        spellzone.insertAdjacentHTML("beforeend", template);
    } else {
        var spellzone = document.querySelector("#spellzone .cardslot");
        spellzone.innerHTML = "";
    }

    if (applicationState.discard.length > 0) {
        var discardPile = document.querySelector("#discard .cardslot");
        discardPile.innerHTML = "";
        let template = handCardTemplate(applicationState.discard[applicationState.discard.length - 1]);
        discardPile.insertAdjacentHTML("beforeend", template);
    }
}

function drawCard() {
    if (applicationState.deck.length > 0)
        applicationState.hand.push(applicationState.deck.pop());
    render();
    handlers();
}

function viewDeck() {
    const deckList = document.getElementById('deckList');
    deckList.innerHTML = ''; // Clear the current deck list

    if (applicationState.deck.length > 0) {
        applicationState.deck.forEach((card, index) => {
            const listItem = document.createElement('li');
            listItem.className = 'deckCard';
            listItem.innerHTML = handCardTemplate(card);
            listItem.dataset.index = index;

            listItem.addEventListener('contextmenu', (event) => {
                event.preventDefault();
                deckSubMenu.showMenu(event.pageX, event.pageY, event.target);
            });

            deckList.appendChild(listItem);
        });
    } else {
        deckList.innerHTML = "No cards in deck";
    }


    openModal('deckModal');
}

function viewDiscard() {
    const discardList = document.getElementById('discardList');
    discardList.innerHTML = ''; // Clear the current deck list

    if (applicationState.discard.length > 0) {
        applicationState.discard.forEach((card, index) => {
            const listItem = document.createElement('li');
            listItem.className = 'discardCard';
            listItem.innerHTML = handCardTemplate(card);
            listItem.dataset.index = index;

            listItem.addEventListener('contextmenu', (event) => {
                event.preventDefault();
                discardSubMenu.showMenu(event.pageX, event.pageY, event.target);
            });
            discardList.appendChild(listItem);
        });
    } else {
        discardList.innerHTML = "No cards in the discard pile";
    }

    openModal('discardModal');
}

function shuffle(array) {
    let currentIndex = array.length;

    // While there remain elements to shuffle...
    while (currentIndex != 0) {

        // Pick a remaining element...
        let randomIndex = Math.floor(Math.random() * currentIndex);
        currentIndex--;

        // And swap it with the current element.
        [array[currentIndex], array[randomIndex]] = [
            array[randomIndex], array[currentIndex]];
    }
}

function shuffleDeck() {
    shuffle(applicationState.deck);
}

function setMenus() {
    // Card Menu
    const cardMenuItems = [
        { label: 'Discard', action: discardFromHand }
    ];
    cardMenu.setMenuItems(cardMenuItems);

    // Deck Menu
    const deckMenuItems = [
        { label: 'Draw', action: drawCard },
        { label: 'Shuffle', action: shuffleDeck },
        { label: 'View', action: viewDeck },
        { label: 'Mill', action: mill }
    ];
    deckMenu.setMenuItems(deckMenuItems);

    // Discard Menu
    const discardMenuItems = [
        { label: 'View', action: viewDiscard }
    ];
    discardMenu.setMenuItems(discardMenuItems);

    // Spellzone Menu
    const spellzoneMenuItems = [
        { label: 'Discard', action: discardFromSpellZone }
    ];
    spellzoneMenu.setMenuItems(spellzoneMenuItems);

    // Discard Sub Menu
    const discardSubMenuItems = [
        { label: 'To Hand', action: discardToHand },
        { label: 'To Top Deck', action: discardToDeck }
    ];
    discardSubMenu.setMenuItems(discardSubMenuItems);

    // Deck Sub Menu
    const deckSubMenuItems = [
        { label: 'To Hand', action: deckToHand },
        { label: 'To Discard', action: deckToDiscard }
    ];
    deckSubMenu.setMenuItems(deckSubMenuItems);
}

function discardToHand(target) {
    var locationInDiscard = target.parentElement.parentElement.dataset.index;
    var card = applicationState.discard.splice(locationInDiscard, 1)[0];

    document.dispatchEvent(new CustomEvent('to-hand', { detail: card }));
    viewDiscard();
}

function discardToDeck(target) {
    var locationInDiscard = target.parentElement.parentElement.dataset.index;
    var card = applicationState.discard.splice(locationInDiscard, 1)[0];

    applicationState.deck.push(card);
    viewDiscard();

    render();
    handlers();
}

function deckToHand(target) {
    var locationInDeck = target.parentElement.parentElement.dataset.index;
    var card = applicationState.deck.splice(locationInDeck, 1)[0];

    document.dispatchEvent(new CustomEvent('to-hand', { detail: card }));
    viewDeck();
}

function deckToDiscard(target) {
    var locationInDeck = target.parentElement.parentElement.dataset.index;
    var card = applicationState.deck.splice(locationInDeck, 1)[0];

    document.dispatchEvent(new CustomEvent('discard-card', { detail: card }));
    viewDeck();
}

function handlers() {
    const elements = document.querySelectorAll("#hand .hand-card");

    for (let handCard of elements) {
        handCard.removeEventListener("dragstart", dragstartHandler);
        handCard.addEventListener("dragstart", dragstartHandler);
    }

    elements.forEach((card) => {
        card.addEventListener('contextmenu', (event) => {
            event.preventDefault();  // Disable default browser context menu
            cardMenu.showMenu(event.pageX, event.pageY, event.target);
        });
    });

    // Context Menus
    setMenus();

    zones.deck.addEventListener('contextmenu', (event) => {
        event.preventDefault();
        deckMenu.showMenu(event.pageX, event.pageY, event.target);
    });

    zones.discard.addEventListener('contextmenu', (event) => {
        event.preventDefault();
        discardMenu.showMenu(event.pageX, event.pageY, event.target);
    });

    zones.spellZone.addEventListener('contextmenu', (event) => {
        event.preventDefault();
        spellzoneMenu.showMenu(event.pageX, event.pageY, event.target);
    });

    zones.spellZone.removeEventListener("drop", spellzoneDrop);
    zones.spellZone.addEventListener('drop', spellzoneDrop);
}

function discardFromSpellZone() {
    let card = applicationState.field.spellZone;
    applicationState.field.spellZone = null;

    document.dispatchEvent(new CustomEvent('discard-card', { detail: card }));

    render();
    handlers();
}

function mill() {
    if (applicationState.deck.length > 0) {
        let card = applicationState.deck.pop();
        document.dispatchEvent(new CustomEvent('discard-card', { detail: card }));
    }

    render();
    handlers();
}

function discardFromHand(target) {
    var id = parseInt(target.parentElement.dataset.id, 10);
    var locationInHand = applicationState.hand.map(function(x) { return x.Id; }).indexOf(id);
    var card = applicationState.hand.splice(locationInHand, 1);

    document.dispatchEvent(new CustomEvent('discard-card', { detail: card[0] }));

    render();
    handlers();
}

function handCardTemplate(card) {
    var data = encodeURIComponent(JSON.stringify(card));
    return `
                                <div draggable="true"
                                    class="hand-card"
                                    data-id="${card.Id}"
                                    data-card="${data}">
                                            <img class="deck-card-image w-100" alt="${card.LatestRevision.Name}" src="${baseUrl}${card.LatestRevision.GetRegularImage}" />
                                    </div>
                            `;
}

function dragstartHandler(ev) {
    // Add the target element's id to the data transfer object
    ev.dataTransfer.setData("text/plain", ev.target.parentElement.dataset.card);
}

function spellzoneDrop(ev) {
    ev.preventDefault();

    try {
        const rawData = ev.dataTransfer.getData('text');
        const card = JSON.parse(decodeURIComponent(rawData));

        if (applicationState.field.spellZone !== null) {
            document.dispatchEvent(new CustomEvent('discard-card', { detail: applicationState.field.spellZone }));
        }

        var locationInHand = applicationState.hand.map(function(x) { return x.Id; }).indexOf(card['Id']);
        applicationState.hand.splice(locationInHand, 1);

        applicationState.field.spellZone = card;
        render();
        handlers();
    }
    catch (ex) {
        console.error('Object is not droppable: ' + ex);
    }
}

document.addEventListener('landscape-card-drop', (event) => {
    const cardData = event.detail.cardData;
    var locationInHand = applicationState.hand.map(function(x) { return x.Id; }).indexOf(cardData['Id']);
    applicationState.hand.splice(locationInHand, 1);
    render();
    handlers();
});

document.addEventListener('building-card-drop', (event) => {
    const cardData = event.detail.cardData;
    var locationInHand = applicationState.hand.map(function(x) { return x.Id; }).indexOf(cardData['Id']);
    applicationState.hand.splice(locationInHand, 1);
    render();
    handlers();
});

document.addEventListener('discard-card', (event) => {
    const cardData = event.detail;
    applicationState.discard.push(cardData);

    render();
    handlers();
});

document.addEventListener('to-hand', (event) => {
    const cardData = event.detail;
    applicationState.hand.push(cardData);

    render();
    handlers();
});

window.addEventListener("DOMContentLoaded", () => {
    applicationState.deck = deck;

    shuffle(applicationState.deck);
    for (var i = 0; i < 5; i++) {
        applicationState.hand.push(applicationState.deck.pop());
    }

    render();
    handlers();
});