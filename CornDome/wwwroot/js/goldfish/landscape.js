class Landscape extends HTMLElement {
    constructor() {
        super();
        this.flooped = false;
        this.damage = 0;
        this.frozen = false;
        this.flipped = false;
        this.originalBackgroundImage = '';
        this.attachShadow({ mode: 'open' });

        this.shadowRoot.innerHTML = `
        <style>
        .landscape {
            display: flex;
            flex: 1 1 auto;
        }

        .drop-landscape {
            position: relative;
            height: 100%;
            margin: 0 auto;
        }

        .card-drop {
            position: absolute;
            margin-left: auto;
            margin-right: auto;
            left: 0;
            right: 0;
            text-align: center;
            top: 0;
            height: 60%;
            width: 60%;
        }

        .status {
            position: absolute;
            margin-left: auto;
            margin-right: auto;
            left: 0;
            right: 0;
            text-align: center;
            top: 60%;
            width: 80%;
            background-color: white;
            font-weight: bold;
        }

        .damage { color: red; }
        .frozen { color: blue; }
        .flooped {
            transform: rotate(0.25turn);
        }

        .landscape-background {
            background-size: contain;
            background-repeat: no-repeat;
            background-position: top;
            height: 100%;
            width: 100%;
        }

        </style>
        <context-menu target-class="landscape-background" class="landscape-background-menu"></context-menu>
        <context-menu target-class="card" class="landscape-card-menu"></context-menu>
        <div class="landscape drop-landscape" ondrop="this.dispatchEvent(new CustomEvent('card-drop', { detail: event, bubbles: true }));" ondragover="event.preventDefault()">
            <div class="landscape-background" style="width: 100%;">
            </div>
            <div class="card-drop">
            </div>
            <div class="status"><span class="damage"></span> <span class="frozen"></span></div>
        </div>
        `;
    }

    static get observedAttributes() {
        return ['data-landscape', 'landscape-image', 'alt-text'];
    }

    // Attribute changes handling
    attributeChangedCallback(name, oldValue, newValue) {
        if (name === 'landscape-image') {
            this.originalBackgroundImage = newValue;
            this.shadowRoot.querySelector('.landscape-background').style.backgroundImage = `url(${this.originalBackgroundImage})`;
        } else if (name === 'alt-text') {
            this.shadowRoot.querySelector('.landscape-background').alt = newValue;
        }
    }

    connectedCallback() {
        this.shadowRoot.querySelector('.landscape').setAttribute('data-landscape', this.getAttribute('data-landscape'));

        const dropZone = this.shadowRoot.querySelector('.landscape');
        dropZone.addEventListener('drop', this.handleDrop.bind(this));

        var addDamage1Fn = function () { this.addDamage(1); };
        var addDamage3Fn = function () { this.addDamage(3); };
        var removeDamage1Fn = function () { this.removeDamage(1); };
        var removeDamage3Fn = function () { this.removeDamage(3); };

        this.cardMenu = this.shadowRoot.querySelector('.landscape-card-menu');
        const cardMenuItems = [
            { label: 'Floop', action: this.floop.bind(this) },
            { label: 'Add Damage 1', action: addDamage1Fn.bind(this) },
            { label: 'Add Damage 3', action: addDamage3Fn.bind(this) },
            { label: 'Remove Damage 1', action: removeDamage1Fn.bind(this) },
            { label: 'Remove Damage 3', action: removeDamage3Fn.bind(this) },
            { label: 'Discard', action: this.removeCard.bind(this) },
            { label: 'To Hand', action: this.toHand.bind(this) }
        ];
        this.cardMenu.setMenuItems(cardMenuItems);

        this.landscapeMenu = this.shadowRoot.querySelector('.landscape-background-menu');
        const landscapeMenuItems = [
            { label: 'Flip', action: this.flip.bind(this) },
            { label: 'Freeze', action: this.freeze.bind(this) }
        ];
        this.landscapeMenu.setMenuItems(landscapeMenuItems);

        const landscapeElement = this.shadowRoot.querySelector('.landscape');
        landscapeElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();  // Disable default browser context menu
            this.landscapeMenu.showMenu(event.pageX, event.pageY, event.target);
        });
    }

    disconnectedCallback() {
        const dropZone = this.shadowRoot.querySelector('.landscape');
        dropZone.removeEventListener('drop', this.handleDrop);
    }

    handleDrop(event) {
        event.preventDefault();

        try {
            const rawData = event.dataTransfer.getData('text');
            const card = JSON.parse(decodeURIComponent(rawData));

            if (this.card) {
                this.removeCard();
            }

            this.setCard(card);
            this.dispatchEvent(new CustomEvent('landscape-card-drop', {
                detail: {
                    landscape: this.getAttribute('data-landscape'),
                    cardData: card,
                },
                bubbles: true,
                composed: true
            }));
        }
        catch (ex) {
            console.error('Object is not droppable: ' + ex);
        }
    }

    handCardTemplate(card) {
        var data = encodeURIComponent(JSON.stringify(card));
        var floopClass = this.flooped ? 'flooped' : '';
        return `
                    <div
                        class="landscape-card ${floopClass}"
                        data-id="${card.Id}"
                        data-card="${data}">
                                <img class="deck-card-image" alt="${card.LatestRevision.Name}" style = "width: 100%;" src="/CardImages/${card.LatestRevision.GetRegularImage}" />
                     </div>
                `;
    }

    setCard(card) {
        this.card = card;

        const cardDrop = this.shadowRoot.querySelector('.card-drop');
        cardDrop.innerHTML = "";
        let template = this.handCardTemplate(card);
        cardDrop.insertAdjacentHTML("beforeend", template);


        const cardElement = this.shadowRoot.querySelector('.landscape-card');
        cardElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();  // Disable default browser context menu
            this.cardMenu.showMenu(event.pageX, event.pageY, event.target);
        }); 
    }

    resetLandscape() {
        this.card = null;
        this.damage = 0;
        this.flooped = false;
        this.frozen = false;
        this.flipped = false;
        const cardDrop = this.shadowRoot.querySelector('.card-drop');
        cardDrop.innerHTML = "";
        this.shadowRoot.querySelector('.damage').innerText = '';
    }

    removeCard() {
        document.dispatchEvent(new CustomEvent('discard-card', { detail: this.card }));
        this.resetLandscape();
    }

    toHand() {
        document.dispatchEvent(new CustomEvent('to-hand', { detail: this.card }));
        this.resetLandscape();
    }

    floop() {
        if (this.flooped) {
            this.shadowRoot.querySelector('.landscape-card').classList.remove('flooped');
        }
        else {
            this.shadowRoot.querySelector('.landscape-card').classList.add('flooped');
        }

        this.flooped = !this.flooped;
    }

    flip() {
        this.flipped = !this.flipped;

        if (this.flipped) {
            this.shadowRoot.querySelector('.landscape-background').style.backgroundImage = `url(/img/lifebackground.jpg)`;
        } else {
            this.shadowRoot.querySelector('.landscape-background').style.backgroundImage = `url(${this.originalBackgroundImage})`;
        }
    }

    freeze() {
        this.frozen = !this.frozen;

        if (this.frozen) {
            this.shadowRoot.querySelector('.frozen').innerText = "Frozen";
        } else {
            this.shadowRoot.querySelector('.frozen').innerText = "";
        }
    }

    addDamage(amount) {
        this.damage += amount;
        this.shadowRoot.querySelector('.damage').innerText = `Damage: ${this.damage}`;
    }

    removeDamage(amount) {
        if (amount > this.damage) {
            this.damage = 0;
        } else {
            this.damage -= amount;
        }

        if (this.damage === 0) {
            this.shadowRoot.querySelector('.damage').innerText = '';
        } else {
            this.shadowRoot.querySelector('.damage').innerText = `Damage: ${this.damage}`;
        }
    }

    getCard() {
        return this.card;
    }
}

// Define the custom button element
customElements.define('cd-landscape', Landscape);