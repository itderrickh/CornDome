var filterFunctions = {
    name: ''
};

document.getElementById("nameFilter").oninput = function () {
    filterFunctions.name = this.value;
    filterDataset();
};

function filterDataset() {
    var elements = document.querySelectorAll('.card-row');
    for (var ele of elements) {
        var actions = [];
        if (filterFunctions.name !== '') {
            var name = ele.dataset.name.toLowerCase();
            var filterName = filterFunctions.name.toLowerCase()
            actions.push(name.indexOf(filterName) > -1);
        }

        if (actions.every(v => v)) {
            ele.classList.remove("hide-row");
        } else {
            ele.classList.add("hide-row");
        }
    }
}