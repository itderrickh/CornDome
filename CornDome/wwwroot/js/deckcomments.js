(() => {
    let nextId = 1;

    const list = document.getElementById("comment-list");
    const input = document.getElementById("commentInput");
    const count = document.getElementById("commentCount");
    const sort = document.getElementById("commentSort");
    const postCommentField = document.getElementById("postComment");

    if (!list || !input || !count || !sort || !postCommentField) {
        return;
    }

    const deckId = list.dataset.deckId;

    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;

    const escapeHtml = value =>
        String(value).replace(/[&<>"']/g, c => ({
            "&": "&amp;",
            "<": "&lt;",
            ">": "&gt;",
            '"': "&quot;",
            "'": "&#039;"
        })[c]);

    function avatar(user, initial, color) {
        return `<div class="comment-avatar"
                     style="background:${escapeHtml(color)}"
                     title="${escapeHtml(user)}">
                    ${escapeHtml(initial)}
                </div>`;
    }

    function renderComment(comment) {
        var isMyDeck = false;
        if (comment.user.id == userId) {
            isMyDeck = true;
        }

        var color = isMyDeck ? "#6551a5" : "lightgreen";
        return `
            <article class="comment" data-id="${comment.id}">
                ${avatar(comment.user.userName, getInitial(comment.user.userName), color)}

                <div class="comment-body">
                    <div class="comment-meta">
                        <span class="comment-author">${escapeHtml(comment.user.userName)}</span>
                        <span class="comment-time">${formatTime(comment.timestamp)}</span>
                    </div>

                    <div class="comment-text">${escapeHtml(comment.commentText)}</div>

                    <div class="reply-container" id="reply-${comment.id}"></div>
                </div>
            </article>`;
    }

    function render() {
        let sorted = [...comments];

        if (sort.value === "newest") {
            sorted.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime());
        } else if (sort.value === "oldest") {
            sorted.sort((a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
        }

        list.innerHTML = sorted.map(renderComment).join("");
        count.textContent = comments.length;
    }

    function getInitial(user) {
        return user?.charAt(0).toUpperCase() || "?";
    }

    function formatTime(timestamp) {
        const date = new Date(timestamp);
        const seconds = Math.floor((Date.now() - date.getTime()) / 1000);

        if (seconds < 60) return "Just now";

        const minutes = Math.floor(seconds / 60);
        if (minutes < 60) return `${minutes} minute${minutes === 1 ? "" : "s"} ago`;

        const hours = Math.floor(minutes / 60);
        if (hours < 24) return `${hours} hour${hours === 1 ? "" : "s"} ago`;

        const days = Math.floor(hours / 24);
        if (days === 1) return "Yesterday";
        if (days < 7) return `${days} days ago`;

        return date.toLocaleDateString();
    }

    postCommentField.addEventListener("click", async () => {
        const text = input.value.trim();

        if (!text) return;

        if (text.length > 400) {
            alert("Comments cannot exceed 400 characters.");
            return;
        }

        if (!token) {
            alert("Unable to verify your request. Please refresh the page.");
            return;
        }

        postCommentField.disabled = true;

        try {
            const response = await fetch(`/api/Decks/${deckId}/comments`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify({ commentText: text })
            });

            if (response.status === 401) {
                alert("Please log in to comment.");
                return;
            }

            const data = await response.json();

            if (!response.ok || !data.success) {
                throw new Error(data.message || "Unable to post comment.");
            }

            const timestamp = data.comment.timestamp;

            comments.push({
                id: nextId++,
                user: data.comment.user,
                commentText: data.comment.commentText,
                timestamp: timestamp
            });

            input.value = "";
            render();
        }
        catch (error) {
            console.error(error);
            alert(error.message || "Something went wrong while posting your comment.");
        }
        finally {
            postCommentField.disabled = false;
        }
    });

    sort.addEventListener("change", render);

    list.addEventListener("click", event => {
        const button = event.target.closest("[data-action]");
        if (!button) return;

        const id = Number(button.dataset.id);
        const comment = comments.find(c => c.id === id);
        if (!comment) return;

        if (button.dataset.action === "like") {
            comment.liked = !comment.liked;
            comment.likes += comment.liked ? 1 : -1;
            render();
        }

        if (button.dataset.action === "reply") {
            const container = document.getElementById(`reply-${id}`);

            if (container.innerHTML) {
                container.innerHTML = "";
                return;
            }

            container.innerHTML = `
                <div class="reply-composer">
                    <textarea class="reply-input"
                              rows="2"
                              placeholder="Write a reply..."></textarea>
                    <div class="reply-buttons">
                        <button class="reply-form-button"
                                data-action="submit-reply"
                                data-id="${id}">
                            Post Reply
                        </button>
                        <button class="reply-form-button"
                                data-action="cancel-reply"
                                data-id="${id}">
                            Cancel
                        </button>
                    </div>
                </div>`;
        }

        if (button.dataset.action === "cancel-reply") {
            document.getElementById(`reply-${id}`).innerHTML = "";
        }

        if (button.dataset.action === "submit-reply") {
            const container = document.getElementById(`reply-${id}`);
            const replyInput = container.querySelector("textarea");
            const text = replyInput.value.trim();

            if (!text) return;

            comment.replies.push({
                id: nextId++,
                user: "You",
                initial: "D",
                color: "#6551a5",
                text,
                time: "Just now"
            });

            render();
        }
    });

    // Deck likes
    document.addEventListener("click", async function (e) {
        const button = e.target.closest(".like-icon");
        if (!button) return;

        const deckId = button.dataset.deckId;
        const countElement = button.parentElement.querySelector(".like-count");

        if (button.classList.contains("loading")) return;

        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        if (!token) {
            alert("Unable to verify your request. Please refresh the page.");
            return;
        }

        button.classList.add("loading");

        try {
            const response = await fetch(`/api/Decks/${deckId}/like`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                },
                body: JSON.stringify({ deckId: parseInt(deckId) })
            });

            if (response.status === 401) {
                alert("Please log in to like this deck.");
                return;
            }

            if (!response.ok) {
                throw new Error("Unable to toggle like.");
            }

            const data = await response.json();

            if (data.success) {
                const liked = data.liked;

                button.classList.toggle("bi-hand-thumbs-up-fill", liked);
                button.classList.toggle("bi-hand-thumbs-up", !liked);
                button.classList.toggle("liked", liked);

                if (countElement) {
                    const currentCount = parseInt(countElement.textContent) || 0;
                    countElement.textContent = Math.max(
                        0,
                        currentCount + (liked ? 1 : -1)
                    );
                }

                button.setAttribute("aria-pressed", liked);
            }
        }
        catch (error) {
            console.error(error);
            alert("Something went wrong while liking this deck.");
        }
        finally {
            button.classList.remove("loading");
        }
    });

    // Save icon hover
    document.querySelectorAll(".save-icon").forEach(icon => {
        icon.addEventListener("mouseenter", () => {
            icon.classList.replace("bi-floppy", "bi-floppy-fill");
        });

        icon.addEventListener("mouseleave", () => {
            icon.classList.replace("bi-floppy-fill", "bi-floppy");
        });
    });

    render();
})();