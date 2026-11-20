function showStats() {
    document.getElementById("showStats").style.display = "none";
    document.getElementById("statsSection").style.display = "block";
    document.getElementById("actionsSection").style.display = "block";
}

function deckToQuery(deck) {
    var heroString = deck.hero != null ? parseInt(deck.hero) : "";
    var landscapeString = packString(deck.landscapes.map((x) => parseInt(x)));
    var cardString = packString(deck.cards.map((x) => parseInt(x)));
    if (heroString || landscapeString || cardString) {
        var data = [heroString, landscapeString, cardString].join(';');
        return data;
    }

    return "";
}


// Current deck visibility
let currentVisibility = document.getElementById("deckVisibilityButton")
    ? parseInt(document.getElementById("deckVisibilityButton").dataset.currentVisibility, 10)
    : null;

// Collect the current deck content
function getDeckContent() {
    const deck = {
        hero: null,
        landscapes: [],
        cards: []
    };

    document.querySelectorAll(".deck-card").forEach(card => {
        if (card.dataset.type === "hero") {
            deck.hero = card.dataset.cardId;
        } else if (card.dataset.type === "landscape") {
            deck.landscapes.push(card.dataset.cardId);
        } else if (card.dataset.type === "card") {
            deck.cards.push(card.dataset.cardId);
        }
    });

    return deckToQuery(deck);
}

// Shared API save function
async function saveDeckChanges(values = {}) {
    const params = new URLSearchParams(window.location.search);
    const deckId = params.get("id");

    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;

    const visibility = values.visibility ??
        (currentVisibility ?? parseInt(document.getElementById("deckVisibility").value, 10));

    const description = values.description ??
        document.getElementById("deckDescription").value;

    const deckName = values.deckName ??
        document.getElementById("deckName").value;

    const iconId = document.getElementById("deckIconCardId").value;

    try {
        const response = await fetch('/api/Decks', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify({
                deckContent: getDeckContent(),
                visibility: visibility,
                description: description,
                iconId: iconId,
                deckId: deckId,
                deckName: deckName
            })
        });

        const result = await response.json();

        if (!response.ok) {
            let message = result.title || "Something went wrong.";

            if (result.errors) {
                message += "\n" + Object.entries(result.errors)
                    .flatMap(([field, messages]) =>
                        messages.map(msg => `${field}: ${msg}`)
                    ).join("\n");
            }

            throw new Error(message);
        }

        return result;
    } catch (error) {
        console.error("Deck save failed:", error);
        alert(error.message || "Unable to save deck.");
        return null;
    }
}

// Existing modal save functionality
document.getElementById("saveDatabaseButton").onclick = async function () {
    const button = this;
    button.disabled = true;

    const result = await saveDeckChanges();

    button.disabled = false;

    if (result) {
        alert(result.message);

        const modalElement = document.getElementById("saveModal");
        const modal = bootstrap.Modal.getInstance(modalElement);

        modal?.hide();

        document.getElementById("apiError").classList.add("d-none");
    }
};


// ==========================================
// INLINE DECK EDITING
// ==========================================

function setupInlineEditor(displayId, editorId, inputId, saveId, cancelId, field) {
    const display = document.getElementById(displayId);
    const editor = document.getElementById(editorId);
    const input = document.getElementById(inputId);
    const saveButton = document.getElementById(saveId);
    const cancelButton = document.getElementById(cancelId);

    if (!display || !editor || !input) return;

    let originalValue = input.value;

    function startEditing() {
        originalValue = input.value;
        display.classList.add("d-none");
        editor.classList.remove("d-none");
        input.focus();

        if (input.tagName === "INPUT") {
            input.select();
        }
    }

    function cancelEditing() {
        input.value = originalValue;
        editor.classList.add("d-none");
        display.classList.remove("d-none");
    }

    async function saveEditing() {
        const newValue = input.value.trim();

        if (field === "deckName" && !newValue) {
            alert("Deck name cannot be empty.");
            input.focus();
            return;
        }

        if (newValue === originalValue) {
            cancelEditing();
            return;
        }

        saveButton.disabled = true;

        const result = await saveDeckChanges({
            [field]: newValue
        });

        saveButton.disabled = false;

        if (!result) return;

        originalValue = newValue;

        if (field === "deckName") {
            const title = display.querySelector("i:first-child");
            if (title) title.textContent = newValue;

            document.getElementById("deckName").value = newValue;
            document.title = newValue;
        } else {
            // Update description without removing the pencil icon.
            display.childNodes[0].textContent = newValue || "Add a description...";

            document.getElementById("deckDescription").value = newValue;
        }

        input.value = newValue;

        editor.classList.add("d-none");
        display.classList.remove("d-none");
    }

    display.addEventListener("click", startEditing);

    saveButton.addEventListener("click", saveEditing);
    cancelButton.addEventListener("click", cancelEditing);

    input.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            event.preventDefault();
            cancelEditing();
        }

        if (event.key === "Enter" &&
            (input.tagName === "INPUT" ||
                (event.ctrlKey && input.tagName === "TEXTAREA"))) {
            event.preventDefault();
            saveEditing();
        }
    });
}

setupInlineEditor(
    "deckTitleDisplay",
    "deckTitleEditor",
    "deckTitleInput",
    "saveTitle",
    "cancelTitle",
    "deckName"
);

setupInlineEditor(
    "deckDescriptionDisplay",
    "deckDescriptionEditor",
    "deckDescriptionInput",
    "saveDescription",
    "cancelDescription",
    "description"
);


// ==========================================
// VISIBILITY DROPDOWN
// ==========================================

const visibilityButton = document.getElementById("deckVisibilityButton");

if (visibilityButton) {
    const visibilityLabel = document.getElementById("deckVisibilityLabel");

    const visibilityNames = {
        0: "Public",
        1: "Unlisted",
        2: "Private"
    };

    document.querySelectorAll("[data-visibility]").forEach(item => {
        item.addEventListener("click", async function () {
            const newVisibility = parseInt(this.dataset.visibility, 10);

            if (newVisibility === currentVisibility) return;

            const result = await saveDeckChanges({
                visibility: newVisibility
            });

            if (!result) return;

            currentVisibility = newVisibility;

            visibilityButton.dataset.currentVisibility = newVisibility;

            if (visibilityNames[newVisibility] === "Public") {
                visibilityLabel.innerHTML = `<i class="bi bi-globe me-2"></i><span>Public</span>`;
            }
            else if (visibilityNames[newVisibility] === "Unlisted") {
                visibilityLabel.innerHTML = `<i class="bi bi-link-45deg me-2"></i><span>Unlisted</span>`;
            }
            else if (visibilityNames[newVisibility] === "Private") {
                visibilityLabel.innerHTML = `<i class="bi bi-lock me-2"></i><span>Private</span>`;
            }
            document.getElementById("deckVisibility").value = newVisibility;
        });
    });
}
