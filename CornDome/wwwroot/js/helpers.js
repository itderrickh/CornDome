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

function getFormattedDateTime() {
    const currentDate = new Date();
    const months = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
    const day = String(currentDate.getDate()).padStart(2, '0');
    const month = months[currentDate.getMonth()];
    const year = currentDate.getFullYear();

    let hours = currentDate.getHours();
    const minutes = String(currentDate.getMinutes()).padStart(2, '0');
    const ampm = hours >= 12 ? 'PM' : 'AM';

    // Convert hours from 24-hour format to 12-hour format
    hours = hours % 12;
    if (hours === 0) hours = 12;  // Adjust for midnight case (0 becomes 12)

    return `${month}/${day}/${year} ${hours}:${minutes} ${ampm}`;
}