let deleteDeckId = null;

document.querySelectorAll('[data-bs-target="#deleteDeckModal"]')
    .forEach(button => {
        button.addEventListener('click', function () {
            deleteDeckId = this.dataset.deckId;

            document.getElementById('deleteDeckDescription').textContent =
                this.dataset.deckDescription;
        });
    });

document.getElementById('confirmDeleteDeck')
    .addEventListener('click', async function () {

        if (!deleteDeckId) {
            return;
        }

        this.disabled = true;
        this.textContent = 'Deleting...';

        try {
            const token = document.querySelector(
                'input[name="__RequestVerificationToken"]'
            ).value;

            const response = await fetch(`/api/Decks/${deleteDeckId}`, {
                method: 'DELETE',
                headers: {
                    'RequestVerificationToken': token
                }
            });

            if (!response.ok) {
                throw new Error(`Delete failed: ${response.status}`);
            }

            // Remove the modal backdrop and reload the page.
            window.location.reload();
        }
        catch (error) {
            console.error(error);

            alert('Unable to delete the deck.');

            this.disabled = false;
            this.textContent = 'Delete';
        }
    });