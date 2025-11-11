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
        const intFields = ['cost', 'set', 'attack', 'defense', 'landscape', 'cardType'];
        const dir = sort.direction === 'a' ? 1 : -1;
        const key = sort.by;

        if (intFields.includes(key)) {
            let aVal = parseInt(aField.dataset[key], 10);
            let bVal = parseInt(bField.dataset[key], 10);

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