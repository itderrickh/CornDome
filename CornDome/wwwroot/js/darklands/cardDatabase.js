
var filterFunctions = {
    cardType: '',
    overlord: '',
    attack: null,
    defense: null,
    name: '',
    ability: ''
};

function filterDataset(elements) {
    let totalResults = 0;
    let filteredResults = 0;

    for (var ele of elements) {
        var actions = [];

        if (filterFunctions.cardType !== '') {
            actions.push(ele.dataset.cardType === filterFunctions.cardType);
        }

        if (filterFunctions.overlord !== '' && filterFunctions.overlord !== '0') {
            actions.push(ele.dataset.overlord === filterFunctions.overlord);
        }

        if (filterFunctions.attack !== null) {
            actions.push(parseInt(ele.dataset.attack) === filterFunctions.attack);
        }

        if (filterFunctions.defense !== null) {
            actions.push(parseInt(ele.dataset.defense) === filterFunctions.defense);
        }

        if (filterFunctions.name !== '') {
            var name = (ele.dataset.name || '').toLowerCase();
            var filterName = filterFunctions.name.toLowerCase();
            actions.push(name.indexOf(filterName) > -1);
        }

        if (filterFunctions.ability !== '') {
            var ability = (ele.dataset.ability || '').toLowerCase();
            var filterAbility = filterFunctions.ability.toLowerCase();
            actions.push(ability.indexOf(filterAbility) > -1);
        }

        if (actions.every(v => v)) {
            ele.style.display = 'inherit';
            filteredResults++;
        } else {
            ele.style.display = 'none';
        }

        totalResults++;
    }

    const filterFinish = new CustomEvent('filter-complete', {
        bubbles: true,
        detail: {
            filteredCount: filteredResults,
            totalElements: totalResults
        }
    });

    document.dispatchEvent(filterFinish);
}

function applyFilters() {
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
}

// Card type
document.getElementById('cardTypeFilter').onchange = function () {
    filterFunctions.cardType = this.value;
    applyFilters();
};

// Overlord
document.getElementById('overlordFilter').onchange = function () {
    filterFunctions.overlord = this.value;
    applyFilters();
};

// Attack
document.getElementById('attackFilter').oninput = function () {
    filterFunctions.attack = this.value !== '' ? parseInt(this.value) : null;
    applyFilters();
};

// Defense
document.getElementById('defenseFilter').oninput = function () {
    filterFunctions.defense = this.value !== '' ? parseInt(this.value) : null;
    applyFilters();
};

// Name
document.getElementById('nameFilter').oninput = function () {
    filterFunctions.name = this.value;
    applyFilters();
};

// Ability
document.getElementById('abilityFilter').oninput = function () {
    filterFunctions.ability = this.value;
    applyFilters();
};

var sort = {
    by: 'name',
    direction: 'a'
};

function sortDataset() {
    const list = document.getElementById("card-list");
    const items = Array.from(document.querySelectorAll('.card-element'));

    items.sort((a, b) => {
        var aField = a.querySelectorAll('.card-container')[0];
        var bField = b.querySelectorAll('.card-container')[0];
        const intFields = ['cardType', 'overlord', 'attack', 'defense'];
        const dir = sort.direction === 'a' ? 1 : -1;
        const key = sort.by;

        if (intFields.includes(key)) {
            let aVal = parseInt(aField.dataset[key].replace("#", ''), 10);
            let bVal = parseInt(bField.dataset[key].replace("#", ''), 10);

            // Treat NaN as -1
            if (isNaN(aVal)) aVal = -1;
            if (isNaN(bVal)) bVal = -1;

            return (aVal - bVal) * dir;
        } else {
            return aField.dataset[key].localeCompare(bField.dataset[key]) * dir;
        }
    });

    list.innerHTML = "";
    items.forEach(item => list.appendChild(item));
}

document.getElementById("sortOption").onchange = function (event) {
    sort.by = event.target.value;
    sortDataset();
};

document.getElementById("sortAscDesc").onchange = function (event) {
    sort.direction = event.target.value;
    sortDataset();
};