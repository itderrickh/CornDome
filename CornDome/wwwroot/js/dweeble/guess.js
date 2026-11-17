(() => {
    const root = document.querySelector(".daily-game");

    if (!root) return;

    const letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".split("");

    let state = null;

    const token = document.querySelector(
        'input[name="__RequestVerificationToken"]'
    )?.value;

    async function api(handler, body = null) {
        const options = {
            method: body ? "POST" : "GET",
            headers: {
                "Content-Type": "application/json"
            }
        };

        if (body) {
            options.headers.RequestVerificationToken = token;
            options.body = JSON.stringify(body);
        }

        const response = await fetch(
            `/Dweeble?handler=${handler}`,
            options
        );

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.error || "Something went wrong.");
        }

        return data;
    }

    function message(text, type = "") {
        document.getElementById("gameMessage").textContent = text;
        document.getElementById("gameMessage").className =
            "daily-message " + type;
    }

    async function refresh() {
        try {
            state = await api("State");
            render();
        } catch (error) {
            message(error.message, "error");
        }
    }

    function render() {
        if (!state) return;

        document.getElementById("dailyDate").textContent = state.date;

        document.getElementById("mistakes").textContent =
            `${state.mistakes} / ${state.maxMistakes}`;

        document.getElementById("lettersGuessed").textContent =
            state.guessedLetters.length;

        document.getElementById("nameDisplay").textContent =
            state.namePattern;

        document.getElementById("abilityDisplay").textContent =
            state.abilityPattern;

        const fields = [
            "landscape",
            "cost",
            "attack",
            "defense",
            "type"
        ];

        fields.forEach(key => {
            const display = document.getElementById(key + "Display");
            const input = document.getElementById(key + "Guess");
            const button = root.querySelector(
                `[data-guess="${key}"]`
            );

            const value = state[key];

            display.textContent = value ?? (
                key === "landscape" || key === "type"
                    ? "???"
                    : "?"
            );

            if (value !== null) {
                display.classList.add("revealed");
            } else {
                display.classList.remove("revealed");
            }

            input.disabled =
                state.isComplete || value !== null;

            button.disabled =
                state.isComplete || value !== null;
        });

        renderKeyboard();
        renderResult();
    }

    function renderKeyboard() {
        const keyboard = document.getElementById("keyboard");

        keyboard.innerHTML = "";

        letters.forEach(letter => {
            const button = document.createElement("button");

            button.textContent = letter;
            button.className = "daily-key";

            const used = state.guessedLetters.includes(letter);

            if (used) {
                const correct =
                    state.namePattern.includes(letter) ||
                    state.namePattern.includes(letter.toLowerCase());

                button.classList.add(
                    correct ? "correct" : "incorrect"
                );
            }

            button.disabled = used || state.isComplete;

            button.addEventListener("click", () => {
                guessLetter(letter);
            });

            keyboard.appendChild(button);
        });
    }

    async function guessLetter(letter) {
        if (state.isComplete) return;

        try {
            state = await api("GuessLetter", {
                value: letter
            });

            message(
                state.guessedLetters.includes(letter)
                    ? `You guessed ${letter}!`
                    : "Letter submitted."
            );

            render();
        } catch (error) {
            message(error.message, "error");
        }
    }

    async function guessAttribute(key) {
        if (state.isComplete || state[key] !== null) return;

        const input = document.getElementById(key + "Guess");
        const value = input.value.trim();

        if (!value) {
            message("Enter a guess first.", "error");
            return;
        }

        try {
            state = await api("GuessAttribute", {
                key,
                value
            });

            input.value = "";

            message(
                state[key] !== null
                    ? `Correct! ${key} revealed.`
                    : "Incorrect guess!",
                state[key] !== null ? "success" : "error"
            );

            render();
        } catch (error) {
            message(error.message, "error");
        }
    }

    function launchConfetti() {
        const colors = ["#d6a84f", "#36b37e", "#ffffff", "#dc5b5b", "#4287f5"];
        const confettiCount = 120;

        for (let i = 0; i < confettiCount; i++) {
            const piece = document.createElement("div");

            piece.style.position = "fixed";
            piece.style.left = `${Math.random() * 100}vw`;
            piece.style.top = "-15px";
            piece.style.width = `${Math.random() * 8 + 5}px`;
            piece.style.height = `${Math.random() * 8 + 5}px`;
            piece.style.backgroundColor =
                colors[Math.floor(Math.random() * colors.length)];
            piece.style.opacity = "1";
            piece.style.pointerEvents = "none";
            piece.style.zIndex = "9999";
            piece.style.borderRadius = Math.random() > 0.5 ? "50%" : "2px";

            document.body.appendChild(piece);

            const duration = Math.random() * 2000 + 2000;
            const drift = (Math.random() - 0.5) * 300;
            const rotation = Math.random() * 1080;

            const animation = piece.animate(
                [
                    {
                        transform: "translate3d(0, 0, 0) rotate(0deg)",
                        opacity: 1
                    },
                    {
                        transform: `translate3d(${drift}px, 110vh, 0) rotate(${rotation}deg)`,
                        opacity: 0
                    }
                ],
                {
                    duration,
                    easing: "cubic-bezier(.25,.46,.45,.94)",
                    delay: Math.random() * 500,
                    fill: "forwards"
                }
            );

            animation.onfinish = () => piece.remove();
        }
    }

    function renderResult() {
        if (!state.isComplete) return;

        document.getElementById("gameResult").style.display = "block";

        document.getElementById("resultTitle").textContent =
            state.won ? "You got it!" : "Better luck tomorrow!";

        document.getElementById("resultDescription").textContent =
            `Today's mystery card was ${state.cardName}. ` +
            `You made ${state.mistakes} incorrect guesses.`;

        if (state.won) {
            document.getElementById("daily-name-section").classList.add("win");
            launchConfetti();
        }

        message(
            state.won
                ? "Congratulations! You solved today's card."
                : "Today's challenge is complete."
        );
    }

    root.querySelectorAll("[data-guess]").forEach(button => {
        button.addEventListener("click", () => {
            guessAttribute(button.dataset.guess);
        });
    });

    root.querySelectorAll(".attribute-input input").forEach(input => {
        input.addEventListener("keydown", event => {
            if (event.key === "Enter") {
                const key = input.id.replace("Guess", "");
                guessAttribute(key);
            }
        });
    });

    refresh();
})();