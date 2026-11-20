const cardSearch = document.getElementById("cardSearch");
const cardId = document.getElementById("cardId");
const cardList = document.getElementById("cardList");

cardSearch.addEventListener("input", function () {
    const value = this.value;

    const option = Array.from(cardList.options)
        .find(x => x.value === value);

    cardId.value = option?.dataset.cardId ?? "";
});