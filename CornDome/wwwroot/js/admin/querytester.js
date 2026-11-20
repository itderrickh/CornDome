document.addEventListener("DOMContentLoaded", () => {
    const queryInput = document.getElementById("queryInput");
    const searchButton = document.getElementById("searchButton");
    const clearButton = document.getElementById("clearButton");

    const resultsBody = document.getElementById("resultsBody");
    const queryStatus = document.getElementById("queryStatus");
    const queryError = document.getElementById("queryError");

    const resultCount = document.getElementById("resultCount");
    const queryTime = document.getElementById("queryTime");
    const resultDescription = document.getElementById("resultDescription");

    // Change this to match your controller's actual route.
    const queryUrl = "/api/cards/query";

    let debounceTimer;
    let currentRequest;

    function validateDql(query) {
        if (!query.trim())
            return { valid: false, incomplete: false, message: "" };

        const fields = [
            "name", "ability", "rulings", "landscape", "type",
            "cost", "attack", "defense", "custom", "id"
        ];

        // Incomplete expressions should wait for more input.
        const trimmed = query.trim();

        if (/[&|!<>=:~]$/.test(trimmed) ||
            /\b(and|or)$/i.test(trimmed)) {
            return {
                valid: false,
                incomplete: true,
                message: "Waiting for a value or expression..."
            };
        }

        // Check balanced parentheses.
        let depth = 0;
        let quoted = false;

        for (let i = 0; i < query.length; i++) {
            if (query[i] === '"' && query[i - 1] !== '\\')
                quoted = !quoted;

            if (quoted)
                continue;

            if (query[i] === '(') depth++;
            if (query[i] === ')') depth--;

            if (depth < 0) {
                return {
                    valid: false,
                    incomplete: false,
                    message: "Unexpected closing parenthesis."
                };
            }
        }

        if (quoted || depth > 0) {
            return {
                valid: false,
                incomplete: true,
                message: quoted
                    ? "Unclosed quotation marks."
                    : "Waiting for closing parenthesis."
            };
        }

        // Validate field names.
        const fieldPattern = /([a-zA-Z]+)\s*(>=|<=|!=|=|:|>|<)/g;
        let match;

        while ((match = fieldPattern.exec(query)) !== null) {
            if (!fields.includes(match[1].toLowerCase())) {
                return {
                    valid: false,
                    incomplete: false,
                    message: `Unknown search field '${match[1]}'.`
                };
            }

            if (match[2] === "~=" &&
                match[1].toLowerCase() !== "name") {
                return {
                    valid: false,
                    incomplete: false,
                    message: "'~=' is only supported for card names."
                };
            }
        }

        // Check for incomplete logical operators.
        if (/(&&|\|\|)\s*(\)|$)/.test(query)) {
            return {
                valid: false,
                incomplete: true,
                message: "Expected another search condition."
            };
        }

        return {
            valid: true,
            incomplete: false,
            message: ""
        };
    }

    async function executeQuery() {
        const validation = validateDql(queryInput.value);

        if (!validation.valid) {
            queryStatus.textContent = "Invalid query";
            return;
        }

        const query = queryInput.value.trim();

        clearTimeout(debounceTimer);

        if (currentRequest) {
            currentRequest.abort();
            currentRequest = null;
        }

        queryError.classList.add("d-none");

        if (!query) {
            resetResults();
            return;
        }

        const controller = new AbortController();
        currentRequest = controller;

        queryStatus.textContent = "Searching...";
        searchButton.disabled = true;

        resultsBody.innerHTML = `
                    <tr>
                        <td colspan="6" class="text-center py-4">
                            <div class="spinner-border spinner-border-sm me-2"></div>
                            Searching cards...
                        </td>
                    </tr>`;

        try {
            const url = new URL(queryUrl, window.location.origin);
            url.searchParams.set("q", query);

            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Accept": "application/json"
                },
                signal: controller.signal
            });

            if (!response.ok) {
                const message = await response.text();
                throw new Error(message || `Request failed (${response.status})`);
            }

            const result = await response.json();

            // Ignore stale responses.
            if (currentRequest !== controller)
                return;

            renderResults(result);

            queryStatus.textContent = "Complete";
        }
        catch (error) {
            if (error.name === "AbortError")
                return;

            queryStatus.textContent = "Error";
            queryError.textContent = error.message;
            queryError.classList.remove("d-none");

            resultsBody.innerHTML = `
                        <tr>
                            <td colspan="6" class="text-center text-danger py-4">
                                Unable to execute query.
                            </td>
                        </tr>`;
        }
        finally {
            if (currentRequest === controller) {
                currentRequest = null;
                searchButton.disabled = false;
            }
        }
    }

    function renderResults(result) {
        const cards = result.data ?? [];

        resultCount.textContent = result.total ?? cards.length;
        queryTime.textContent = `${Number(result.elapsed ?? 0).toFixed(2)} ms`;

        resultDescription.textContent =
            `Showing ${cards.length} card(s)`;

        resultsBody.replaceChildren();

        if (cards.length === 0) {
            resultsBody.innerHTML = `
                        <tr>
                            <td colspan="6" class="text-center text-muted py-4">
                                No cards found.
                            </td>
                        </tr>`;
            return;
        }

        for (const card of cards) {
            const row = document.createElement("tr");

            const type = card.cardType?.value ?? card.cardType?.name ?? "-";

            const values = [
                card.name,
                type,
                card.cost ?? "-",
                card.attack ?? "-",
                card.defense ?? "-",
                card.id
            ];

            for (const value of values) {
                const cell = document.createElement("td");
                cell.textContent = value;
                row.appendChild(cell);
            }

            resultsBody.appendChild(row);
        }
    }

    function resetResults() {
        resultCount.textContent = "0";
        queryTime.textContent = "0 ms";
        resultDescription.textContent = "";
        queryStatus.textContent = "Ready";

        resultsBody.innerHTML = `
                    <tr>
                        <td colspan="6" class="text-center text-muted py-4">
                            Enter a query to search for cards.
                        </td>
                    </tr>`;
    }

    function scheduleSearch() {
        clearTimeout(debounceTimer);

        if (currentRequest) {
            currentRequest.abort();
            currentRequest = null;
        }

        const validation = validateDql(queryInput.value);

        queryError.classList.add("d-none");

        if (!queryInput.value.trim()) {
            resetResults();
            return;
        }

        if (!validation.valid) {
            queryStatus.textContent = validation.incomplete
                ? "Incomplete query"
                : "Invalid query";

            queryError.textContent = validation.message;
            queryError.classList.remove("d-none");

            return;
        }

        queryStatus.textContent = "Waiting...";

        debounceTimer = setTimeout(executeQuery, 300);
    }

    queryInput.addEventListener("input", scheduleSearch);

    queryInput.addEventListener("keydown", event => {
        if (event.key === "Enter") {
            clearTimeout(debounceTimer);
            executeQuery();
        }
    });

    searchButton.addEventListener("click", executeQuery);

    clearButton.addEventListener("click", () => {
        clearTimeout(debounceTimer);

        if (currentRequest) {
            currentRequest.abort();
            currentRequest = null;
        }

        queryInput.value = "";
        queryError.classList.add("d-none");
        searchButton.disabled = false;

        resetResults();
        queryInput.focus();
    });

    document.querySelectorAll(".example-query").forEach(button => {
        button.addEventListener("click", () => {
            queryInput.value = button.dataset.query;
            executeQuery();
        });
    });
});