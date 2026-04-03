
const input = document.getElementById("tag-input");
const dropdown = document.getElementById("tag-dropdown");
const container = document.getElementById("tagContainer");

function renderTags() {
    document.querySelectorAll(".tag").forEach(t => t.remove());

    selected.forEach(set => {
        const tag = document.createElement("div");
        tag.className = "tag";
        tag.innerHTML = `${set.description} <button onclick="removeTag(${set.id})">×</button>`;
        container.insertBefore(tag, input);
    });
}

function renderDropdown(filter = "") {
    dropdown.innerHTML = "";
    updateHiddenInput();

    allSets
        .filter(s => s.description.toLowerCase().includes(filter.toLowerCase()))
        .filter(s => !selected.find(sel => sel.id === s.id))
        .forEach(set => {
            const item = document.createElement("div");
            item.textContent = set.description;
            item.onclick = () => addTag(set);
            dropdown.appendChild(item);
        });
}

function addTag(set) {
    selected.push(set);
    input.value = "";
    renderTags();
    renderDropdown();
}

function updateHiddenInput() {
    const ids = selected.map(s => s.id);
    document.getElementById("setIdsInput").value = ids.join(",");
}

function removeTag(id) {
    const index = selected.findIndex(s => s.id === id);
    selected.splice(index, 1);
    renderTags();
    renderDropdown(input.value);
}

input.addEventListener("input", e => {
    renderDropdown(e.target.value);
});

renderTags();
renderDropdown();