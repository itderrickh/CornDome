var availableFields = {
    all: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    creature: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    spell: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    building: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    hero: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    landscape: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    teamwork: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    none: []
};

var adminAvailableFields = {
    all: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    creature: ['abilityField', 'setField', 'costField', 'attackField', 'defenseField', 'landscapeField'],
    spell: ['abilityField', 'setField', 'costField', 'landscapeField'],
    building: ['abilityField', 'setField', 'costField', 'landscapeField'],
    hero: ['abilityField', 'setField'],
    landscape: ['landscapeField'],
    teamwork: ['abilityField', 'setField', 'costField', 'landscapeField'],
    none: []
};

var editableFields = isAdmin ? adminAvailableFields : availableFields;

function getFieldsToShow(value) {
    var fieldsToShow = 'all';
    switch (value) {
        case 0:
            fieldsToShow = 'creature';
            break;
        case 1:
            fieldsToShow = 'spell';
            break;
        case 2:
            fieldsToShow = 'building';
            break;
        case 3:
            fieldsToShow = 'landscape';
            break;
        case 4:
            fieldsToShow = 'hero';
            break;
        case 5:
            fieldsToShow = 'teamwork';
            break;
    }

    return fieldsToShow;
}

function showOnlyRelevantFields(fieldsToShow) {
    editableFields.all.forEach((value) => {
        var fields = document.getElementsByClassName(value);
        Array.from(fields).forEach(field => {
            field.style.display = 'none';
        });
    });

    editableFields[fieldsToShow].forEach((value) => {
        var fields = document.getElementsByClassName(value);
        Array.from(fields).forEach(field => {
            field.style.display = 'inherit';
        });
    });
}

var cardTypeField = document.getElementsByClassName("cardType");
Array.from(cardTypeField).forEach(field => {
    field.onchange = function (e) {
        var selectedValue = parseInt(e.currentTarget.value, 10);
        var fieldsToShow = getFieldsToShow(selectedValue);

        showOnlyRelevantFields(fieldsToShow);
    };
});


var selectedValue = parseInt(cardTypeField[0].value, 10);
var fieldsToShow = getFieldsToShow(selectedValue);

showOnlyRelevantFields(fieldsToShow);

document.querySelectorAll('.fileInput').forEach((input) => {
    input.addEventListener('change', (event) => {
        const file = event.target.files[0];
        if (!file) return;

        const reader = new FileReader();

        // find the nearest newPreview in the same parent container
        const preview = event.target.closest('.preview-box').querySelector('img, #newPreview')
                        || event.target.closest('.col-6').querySelector('.newPreview');

        reader.onload = (e) => {
            if (preview) preview.src = e.target.result;
        };
        reader.readAsDataURL(file);
    });
});