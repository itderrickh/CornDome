var filterFunctions = {
    cardType: '',
    landscape: '',
    attack: null,
    defense: null,
    cost: null,
    name: '',
    ability: ''
};

function filterDataset() {
    var elements = document.querySelectorAll('.card-container');
    for (var ele of elements) {
        var actions = [];
        if (filterFunctions.cardType !== "")
            actions.push(ele.dataset.cardType === filterFunctions.cardType)
        if (filterFunctions.landscape !== "")
            actions.push(ele.dataset.landscape === filterFunctions.landscape);
        if (filterFunctions.cost !== null)
            actions.push(parseInt(ele.dataset.cost) === filterFunctions.cost);
        if (filterFunctions.attack !== null)
            actions.push(parseInt(ele.dataset.attack) === filterFunctions.attack);
        if (filterFunctions.defense !== null)
            actions.push(parseInt(ele.dataset.defense) === filterFunctions.defense);
        if (filterFunctions.name !== '') {
            var name = ele.dataset.name.toLowerCase();
            var filterName = filterFunctions.abilitytoLowerCase()
            actions.push(name.indexOf(filterName) > -1);
        }
        if (filterFunctions.ability !== '') {
            var ability = ele.dataset.ability.toLowerCase();
            var filterAbility = filterFunctions.abilitytoLowerCase()
            actions.push(ability.indexOf(filterAbility) > -1);
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