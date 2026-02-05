var filterFunctions = {
    cardType: '',
    landscape: [],
    attack: null,
    defense: null,
    cost: null,
    name: '',
    ability: '',
    set: null,
    customCards: false,
    burnToken: false,
    freezeToken: false,
    attackToken: false
};

function filterDataset(elements) {
    let totalResults = 0;
    let filteredResults = 0;
    for (var ele of elements) {
        var actions = [];
        if (filterFunctions.cardType !== "")
            actions.push(ele.dataset.cardType === filterFunctions.cardType)
        if (filterFunctions.landscape.length > 0)
            actions.push(filterFunctions.landscape.indexOf(ele.dataset.landscape) > -1);
        if (filterFunctions.cost !== null)
            actions.push(parseInt(ele.dataset.cost) === filterFunctions.cost);
        if (filterFunctions.attack !== null)
            actions.push(parseInt(ele.dataset.attack) === filterFunctions.attack);
        if (filterFunctions.defense !== null)
            actions.push(parseInt(ele.dataset.defense) === filterFunctions.defense);
        if (filterFunctions.name !== '') {
            var name = ele.dataset.name.toLowerCase();
            var filterName = filterFunctions.name.toLowerCase()
            actions.push(name.indexOf(filterName) > -1);
        }
        if (filterFunctions.ability !== '') {
            var ability = ele.dataset.ability.toLowerCase();
            var filterAbility = filterFunctions.ability.toLowerCase();
            actions.push(ability.indexOf(filterAbility) > -1);
        }
        if (filterFunctions.set !== null && filterFunctions.set !== '') {
            var set = ele.dataset.set.toLowerCase();
            var filterSet = filterFunctions.set.toLowerCase()
            actions.push(set === filterSet);
        }

        if (filterFunctions.burnToken) {
            var ability = ele.dataset.ability.toLowerCase();
            actions.push(ability.indexOf("burn") > -1 || ability.indexOf("burning") > -1);
        }
        if (filterFunctions.freezeToken) {
            var ability = ele.dataset.ability.toLowerCase();
            actions.push(ability.indexOf("freeze") > -1 || ability.indexOf("frozen") > -1);
        }
        if (filterFunctions.attackToken) {
            var ability = ele.dataset.ability.toLowerCase();
            actions.push(ability.indexOf("[attack]") > -1);
        }


        if (!filterFunctions.customCards) {
            actions.push(ele.dataset.custom !== "True");
        }

        if (actions.every(v => v)) {
            ele.style.display = "inherit";
            filteredResults++;
            totalResults++;
        } else {
            ele.style.display = "none";
            totalResults++;
        }
    }

    const filterFinish = new CustomEvent("filter-complete", {
        bubbles: true,
        detail: {
            filteredCount: filteredResults,
            totalElements: totalResults
        }
    });

    document.dispatchEvent(filterFinish);
}

document.getElementById("cardTypeFilter").onchange = function () {
    filterFunctions.cardType = this.value;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

var landscapeCheckboxes = document.querySelectorAll(".landscape-checkbox");
landscapeCheckboxes.forEach(x => {
    x.onchange = function () {
        if (this.checked) {
            filterFunctions.landscape.push(this.value);
        } else {
            var indexToRemove = filterFunctions.landscape.indexOf(this.value);
            filterFunctions.landscape.splice(indexToRemove, 1);
        }

        var cards = document.querySelectorAll('.card-container');
        filterDataset(cards);
    }
});

document.getElementById("costFilter").oninput = function () {
    filterFunctions.cost = this.value !== "" ? parseInt(this.value) : null;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("attackFilter").oninput = function () {
    filterFunctions.attack = this.value !== "" ? parseInt(this.value) : null;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("defenseFilter").oninput = function () {
    filterFunctions.defense = this.value !== "" ? parseInt(this.value) : null;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("abilityFilter").oninput = function () {
    filterFunctions.ability = this.value;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("nameFilter").oninput = function () {
    filterFunctions.name = this.value;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("setFilter").oninput = function () {
    filterFunctions.set = this.value;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
}

document.getElementById("customCardFilter").onchange = function () {
    filterFunctions.customCards = this.checked;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("burnTokenFilter").onchange = function () {
    filterFunctions.burnToken = this.checked;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("freezeTokenFilter").onchange = function () {
    filterFunctions.freezeToken = this.checked;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};

document.getElementById("attackTokenFilter").onchange = function () {
    filterFunctions.attackToken = this.checked;
    var cards = document.querySelectorAll('.card-container');
    filterDataset(cards);
};