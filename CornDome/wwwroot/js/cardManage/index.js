
document.addEventListener("DOMContentLoaded", function () {
    const filter = document.getElementById("nameFilter");
    const clearButton = document.getElementById("clearFilter");
    const rows = document.querySelectorAll(".card-row");
    const count = document.getElementById("visibleCardCount");
    const noCardsFound = document.getElementById("noCardsFound");
    const searchContainer = document.querySelector(".card-search");

    if (!filter) return;

    function filterCards() {
        const search = filter.value.trim().toLowerCase();
        let visible = 0;

        rows.forEach(row => {
            const name = (row.dataset.name || "").toLowerCase();
            const matches = name.includes(search);

            row.classList.toggle("d-none", !matches);

            if (matches) visible++;
        });

        count.textContent = visible;
        noCardsFound.classList.toggle("d-none", visible !== 0 || rows.length === 0);
        searchContainer.classList.toggle("has-value", search.length > 0);
    }

    filter.addEventListener("input", filterCards);

    clearButton.addEventListener("click", function () {
        filter.value = "";
        filterCards();
        filter.focus();
    });
});
