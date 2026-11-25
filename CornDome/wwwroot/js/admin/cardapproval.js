const antiforgeryToken = document.querySelector(
    'input[name="__RequestVerificationToken"]'
).value;

async function approveCard(id) {
    const body = new URLSearchParams({ id: id });

    const response = await fetch("?handler=Approve", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "RequestVerificationToken": antiforgeryToken
        },
        body: body
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
        throw new Error(result.message || "Unable to approve card.");
    }

    return result;
}

document.querySelectorAll(".approve-card").forEach(button => {
    button.addEventListener("click", async function () {
        const id = this.dataset.id;
        const name = this.dataset.name;

        if (!confirm(`Approve "${name}"?`))
            return;

        this.disabled = true;

        try {
            await approveCard(id);

            document.getElementById(`card-${id}`).remove();

            const remaining = document.querySelectorAll("#approval-list > div").length;

            document.getElementById("pending-count").textContent =
                `${remaining} Pending`;

            if (remaining === 0) {
                document.getElementById("empty-message")?.remove();

                const message = document.createElement("div");
                message.id = "empty-message";
                message.className = "alert alert-success";
                message.innerHTML =
                    '<i class="bi bi-check-circle"></i> There are no custom cards waiting for approval.';

                document.getElementById("approval-list").appendChild(message);
            }
        }
        catch (error) {
            alert(error.message);
            this.disabled = false;
        }
    });
});

async function denyCard(id) {
    const body = new URLSearchParams({ id: id });

    const response = await fetch("?handler=Deny", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
            "RequestVerificationToken": antiforgeryToken
        },
        body: body
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
        throw new Error(result.message || "Unable to deny card.");
    }

    return result;
}

document.querySelectorAll(".deny-card").forEach(button => {
    button.addEventListener("click", async function () {
        const id = this.dataset.id;
        const name = this.dataset.name;

        if (!confirm(`Are you sure you want to deny "${name}"?`))
            return;

        this.disabled = true;

        try {
            await denyCard(id);

            document.getElementById(`card-${id}`).remove();

            const remaining = document.querySelectorAll("#approval-list > div").length;

            document.getElementById("pending-count").textContent =
                `${remaining} Pending`;

            if (remaining === 0) {
                const message = document.createElement("div");
                message.className = "alert alert-success";
                message.innerHTML =
                    '<i class="bi bi-check-circle"></i> There are no custom cards waiting for approval.';

                document.getElementById("approval-list").appendChild(message);
            }
        }
        catch (error) {
            alert(error.message);
            this.disabled = false;
        }
    });
});