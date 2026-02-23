document.getElementById("saveButton").onclick = async function () {
    var query = new URLSearchParams(document.location.search);
    var deck = query.get("deck");
    var gzdeck = query.get("gzdeck");
    var db = new DecksDatabase();

    var deckToSave = "";
    if (!deck) {
        deckToSave = deck;
    }
    else {
        deckToSave = gzdeck;
    }
    try {
        await db.open();

        const newDeck = { name: "New deck " + getFormattedDateTime(), content: deckToSave };
        const addMessage = await db.addDeck(newDeck);

        alert(addMessage);
    } catch (ex) {
        alert("Error adding deck: ", ex);
    }
}