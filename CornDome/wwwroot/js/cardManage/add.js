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
        document.getElementById(value).style.display = 'inherit';
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