(() => {
    const nameFilter = document.getElementById("nameFilter");
    const typeFilter = document.getElementById("typeFilter");
    const overlordFilter = document.getElementById("overlordFilter");
    const rows = [...document.querySelectorAll(".darklands-card-row")];
    const visibleCount = document.getElementById("visibleCount");
    const emptyResults = document.getElementById("emptyResults");

    function filterCards() {
        const name = nameFilter.value.trim().toLowerCase();
        const type = typeFilter.value.toLowerCase();
        const overlord = overlordFilter.value.toLowerCase();

        let visible = 0;

        rows.forEach(row => {
            const matchesName = (row.dataset.name || "").toLowerCase().includes(name);
            const matchesType = !type || (row.dataset.type || "").toLowerCase() === type;
            const matchesOverlord = !overlord || (row.dataset.overlord || "").toLowerCase() === overlord;

            const matches = matchesName && matchesType && matchesOverlord;

            row.classList.toggle("d-none", !matches);

            if (matches) visible++;
        });

        visibleCount.textContent = `${visible} of ${rows.length} cards`;
        emptyResults.classList.toggle("d-none", visible !== 0);
    }

    nameFilter.addEventListener("input", filterCards);
    typeFilter.addEventListener("change", filterCards);
    overlordFilter.addEventListener("change", filterCards);

    document.getElementById("clearFilters").addEventListener("click", () => {
        nameFilter.value = "";
        typeFilter.value = "";
        overlordFilter.value = "";
        filterCards();
    });
})();