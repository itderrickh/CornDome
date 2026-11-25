const antiforgeryToken = document.querySelector(
    'input[name="__RequestVerificationToken"]'
).value;

async function postCardAction(handler, data) {
    const body = new URLSearchParams(data);

    const response = await fetch(`?handler=${handler}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "RequestVerificationToken": antiforgeryToken
        },
        body: body
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
        throw new Error(result.message || "Something went wrong.");
    }

    return result;
}

document.querySelectorAll(".custom-card-visibility").forEach(select => {
    let previousValue = select.value;

    select.addEventListener("change", async function () {
        const newValue = this.value;

        this.disabled = true;

        try {
            await postCardAction("UpdateVisibility", {
                id: this.dataset.id,
                visibility: newValue
            });

            previousValue = newValue;
        }
        catch (error) {
            this.value = previousValue;
            alert(error.message);
        }
        finally {
            this.disabled = false;
        }
    });
});

document.querySelectorAll(".delete-custom-card").forEach(button => {
    button.addEventListener("click", async function () {
        const id = this.dataset.id;
        const name = this.dataset.name;

        if (!confirm(`Are you sure you want to delete "${name}"? This cannot be undone.`))
            return;

        this.disabled = true;

        try {
            await postCardAction("Delete", { id: id });

            document.getElementById(`card-${id}`).remove();
        }
        catch (error) {
            alert(error.message);
            this.disabled = false;
        }
    });
});