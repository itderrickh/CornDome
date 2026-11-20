var availableFields = {
    all: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    creature: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    spell: ['abilityField', 'setField', 'costField', 'landscapeField'],
    building: ['abilityField', 'setField', 'costField', 'landscapeField'],
    hero: ['abilityField', 'setField'],
    landscape: ['landscapeField'],
    teamwork: ['abilityField', 'setField', 'costField', 'landscapeField'],
    none: []
};

document.getElementById("cardType").onchange = function (e) {
    var fieldsToShow = 'all';
    var selectedValue = parseInt(e.currentTarget.value, 10);
    switch (selectedValue) {
        case 0:
            fieldsToShow = 'creature';
            break;
        case 1:
            fieldsToShow = 'spell';
            break;
        case 2:
            fieldsToShow = 'building';
            break;
        case 4:
            fieldsToShow = 'hero';
            break;
        case 3:
            fieldsToShow = 'landscape';
            break;
        case 5:
            fieldsToShow = 'teamwork';
            break;
    }

    availableFields.all.forEach((value) => {
        document.getElementById(value).style.display = 'none';
    });

    availableFields[fieldsToShow].forEach((value) => {
        document.getElementById(value).style.display = '';
    });
};

document.getElementById('fileInput').addEventListener('change', (event) => {
    const file = event.target.files[0];
    if (!file) return;

    const reader = new FileReader();

    // find the nearest newPreview in the same parent container
    const preview = event.target.parentElement.querySelector(".newPreview");

    reader.onload = (e) => {
        if (preview) preview.src = e.target.result;
    };
    reader.readAsDataURL(file);
});

document.getElementById("addCardForm").addEventListener("submit", function () {
    const submitButton = document.getElementById("submitButton");

    if (submitButton.disabled) {
        return;
    }

    submitButton.disabled = true;
    submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Creating Card...';
});

const fileInput = document.getElementById("fileInput");
const previewContainer = document.querySelector(".image-preview-container");
const previewImage = document.querySelector(".newPreview");

fileInput.addEventListener("change", function () {
    const file = this.files[0];

    if (!file) {
        previewImage.src = "";
        previewContainer.classList.remove("has-image");
        return;
    }

    if (!file.type.startsWith("image/")) {
        this.value = "";
        previewImage.src = "";
        previewContainer.classList.remove("has-image");
        return;
    }

    const reader = new FileReader();

    reader.onload = function (e) {
        previewImage.src = e.target.result;
        previewContainer.classList.add("has-image");
    };

    reader.readAsDataURL(file);
});