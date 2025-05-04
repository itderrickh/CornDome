var filterFunctions = {
    cardType: '',
    landscape: [],
    attack: null,
    defense: null,
    cost: null,
    name: '',
    ability: '',
    set: null,
    customCards: false
};

function filterDataset() {
    var elements = document.querySelectorAll('.card-container');
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
            var filterAbility = filterFunctions.ability.toLowerCase()
            actions.push(ability.indexOf(filterAbility) > -1);
        }
        if (filterFunctions.set !== null) {
            var set = ele.dataset.set.toLowerCase();
            var filterSet = filterFunctions.set.toLowerCase()
            actions.push(set.indexOf(filterSet) > -1);
        }
        if (!filterFunctions.customCards) {
            actions.push(ele.dataset.custom !== "True");
        }

        if (actions.every(v => v)) {
            ele.style.display = "inherit";
        } else {
            ele.style.display = "none";
        }
    }
}

document.getElementById("cardTypeFilter").onchange = function () {
    filterFunctions.cardType = this.value;
    filterDataset();
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

        filterDataset();
    }
});

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

document.getElementById("setFilter").oninput = function () {
    filterFunctions.set = this.value;
    filterDataset();
}

document.getElementById("customCardFilter").onchange = function () {
    filterFunctions.customCards = this.checked;
    filterDataset();
};