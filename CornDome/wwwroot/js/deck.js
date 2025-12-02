document.getElementById("saveButton").onclick = async function () {
    var query = new URLSearchParams(document.location.search);
    var deck = query.get("deck");
    var db = new DecksDatabase();

    try {
        await db.open();

        const newDeck = { name: "New deck " + getFormattedDateTime(), content: deck };
        const addMessage = await db.addDeck(newDeck);

        alert(addMessage);
    } catch (ex) {
        alert("Error adding deck: ", ex);
    }
}