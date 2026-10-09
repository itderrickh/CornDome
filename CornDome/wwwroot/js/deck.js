function showStats() {
    document.getElementById("showStats").style.display = "none";
    document.getElementById("statsSection").style.display = "block";
    document.getElementById("actionsSection").style.display = "block";
}

function deckToQuery(deck) {
    var heroString = deck.hero != null ? parseInt(deck.hero) : "";
    var landscapeString = packString(deck.landscapes.map((x) => parseInt(x)));
    var cardString = packString(deck.cards.map((x) => parseInt(x)));
    if (heroString || landscapeString || cardString) {
        var data = [heroString, landscapeString, cardString].join(';');
        return data;
    }

    return "";
}

document.getElementById("saveDatabaseButton").onclick = async function () {
    var deck = {
        'hero': null,
        'landscapes': [],
        'cards': []
    }
    document.querySelectorAll(".deck-card").forEach(function(card) {
        if (card.dataset.type === 'hero') {
            deck.hero = card.dataset.cardId;
        }
        if (card.dataset.type === 'landscape') {
            deck.landscapes.push(card.dataset.cardId);
        }
        if (card.dataset.type === 'card') {
            deck.cards.push(card.dataset.cardId);
        }
    });

    const errorBox = document.getElementById('apiError');
    const params = new URLSearchParams(window.location.search);
    const deckId = params.get('id');

    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    ).value;

    const visibility = parseInt(document.getElementById("deckVisibility").value, 10);
    const description = document.getElementById("deckDescription").value;
    const iconId = document.getElementById("deckIconCardId").value;
    const deckName = document.getElementById("deckName").value

    const response = await fetch('/api/Deck', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': token
        },
        body: JSON.stringify({
            deckContent: deckToQuery(deck),
            visibility: visibility,
            description: description,
            iconId: iconId,
            deckId: deckId,
            deckName: deckName
        })
    });

    const result = await response.json();

    if (!response.ok) {
        const title = document.getElementById('apiErrorTitle');
        const list = document.getElementById('apiErrorList');

        title.textContent = result.title || 'Something went wrong';

        list.innerHTML = '';

        if (result.errors) {
            for (const [field, messages] of Object.entries(result.errors)) {
                for (const msg of messages) {
                    const li = document.createElement('li');

                    const strong = document.createElement('strong');
                    strong.textContent = field + ': ';

                    li.appendChild(strong);
                    li.appendChild(document.createTextNode(msg));

                    list.appendChild(li);
                }
            }
        }

        errorBox.classList.remove('d-none');
    }
    else {
        alert(result.message);
        const modalElement = document.getElementById('saveModal');
        const modal = bootstrap.Modal.getInstance(modalElement);

        modal.hide();
                
        errorBox.classList.add('d-none');
    }
}