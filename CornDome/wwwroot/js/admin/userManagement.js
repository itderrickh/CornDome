document.addEventListener('DOMContentLoaded', function () {
    const search = document.getElementById('userSearch');
    const rows = Array.from(document.querySelectorAll('.user-row'));
    const count = document.getElementById('userCount');
    const noResults = document.getElementById('noResults');

    function filterUsers() {
        const query = search.value.trim().toLowerCase();
        let visible = 0;

        rows.forEach(row => {
            const matches = row.textContent.toLowerCase().includes(query);

            row.classList.toggle('d-none', !matches);

            if (matches) visible++;
        });

        count.textContent = `${visible} of ${rows.length} users`;
        noResults.classList.toggle('d-none', visible !== 0);
    }

    search.addEventListener('input', filterUsers);
});