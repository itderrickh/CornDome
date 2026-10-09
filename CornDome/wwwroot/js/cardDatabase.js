document.addEventListener("filter-complete", function (e) {
    console.log(e);
    document.getElementById("cards-found").innerHTML = `Showing ${e.detail.filteredCount} of ${e.detail.totalElements} cards`;
});

var cards = document.querySelectorAll('.card-container');
filterDataset(cards);