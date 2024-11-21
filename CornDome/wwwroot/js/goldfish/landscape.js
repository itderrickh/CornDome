class Landscape extends HTMLElement {
    constructor() {
        super();
        this.flooped = false;
        this.damage = 0;
        this.attachShadow({ mode: 'open' });

        this.shadowRoot.innerHTML = `
        <style>
        .landscape {
            display: flex;
            flex: 1 1 auto;
        }

        .drop-landscape {
            position: relative;
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

        .damage {
            position: absolute;
            margin-left: auto;
            margin-right: auto;
            left: 0;
            right: 0;
            text-align: center;
            top: 60%;
            color: red;
            width: 40%;
            background-color: white;
            font-weight: bold;
        }
        .flooped {
            transform: rotate(-0.25turn);
        }
        </style>
        <context-menu target-class="card" class="landscape-card-menu"></context-menu>
        <div class="landscape drop-landscape" ondrop="this.dispatchEvent(new CustomEvent('card-drop', { detail: event, bubbles: true }));" ondragover="event.preventDefault()">
            <div style="width: 100%;">
                <img class="card-image" />
            </div>
            <div class="card-drop">
            </div>
            <div class="damage"></div>
        </div>
        `;
    }

    static get observedAttributes() {
        return ['data-landscape', 'landscape-image', 'alt-text'];
    }

    // Attribute changes handling
    attributeChangedCallback(name, oldValue, newValue) {
        if (name === 'landscape-image') {
            this.shadowRoot.querySelector('.card-image').src = newValue;
        } else if (name === 'alt-text') {
            this.shadowRoot.querySelector('.card-image').alt = newValue;
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
            { label: 'Discard', action: this.removeCard.bind(this) }
        ];
        this.cardMenu.setMenuItems(cardMenuItems);
    }

    disconnectedCallback() {
        const dropZone = this.shadowRoot.querySelector('.landscape');
        dropZone.removeEventListener('drop', this.handleDrop);
    }

    handleDrop(event) {
        event.preventDefault();
        const rawData = event.dataTransfer.getData('text');
        const card = JSON.parse(decodeURIComponent(rawData));

        this.setCard({ data: card });
        this.dispatchEvent(new CustomEvent('landscape-card-drop', {
            detail: {
                landscape: this.getAttribute('data-landscape'),
                cardData: card,
            },
            bubbles: true,
            composed: true
        }));
    }

    handCardTemplate(card, flooped) {
        var data = encodeURIComponent(JSON.stringify(card));
        var floopClass = flooped ? 'flooped' : '';
        return `
                    <div draggable="true"
                        class="card ${floopClass}"
                        data-id="${card.id}"
                        data-card="${data}">
                                <img class="deck-card-image" alt="${card.name}" style = "width: 100%;" src="/CardImages/${card.imageurl}" />
                     </div>
                `;
    }

    setCard(card) {
        this.card = card;

        const cardDrop = this.shadowRoot.querySelector('.card-drop');
        cardDrop.innerHTML = "";
        let template = handCardTemplate(card.data, false);
        cardDrop.insertAdjacentHTML("beforeend", template);


        const cardElement = this.shadowRoot.querySelector('.card');
        cardElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();  // Disable default browser context menu
            this.cardMenu.showMenu(event.layerX, event.layerY);
        });
    }

    removeCard() {
        this.card = null;
        this.damage = 0;
        this.flooped = false;

        const cardDrop = this.shadowRoot.querySelector('.card-drop');
        cardDrop.innerHTML = "";
        this.shadowRoot.querySelector('.damage').innerText = '';
    }

    floop() {
        if (this.flooped) {
            this.shadowRoot.querySelector('.card').classList.remove('flooped');
        }
        else {
            this.shadowRoot.querySelector('.card').classList.add('flooped');
        }

        this.flooped = !this.flooped;
    }

    addDamage(amount) {
        this.damage += amount;
        this.shadowRoot.querySelector('.damage').innerText = `Damage: ${this.damage}`;
    }

    removeDamage(amount) {
        if (amount > this.damage)
            this.damage = 0;
        else
            this.damage -= amount;

        this.shadowRoot.querySelector('.damage').innerText = `Damage: ${this.damage}`;
    }

    getCard() {
        return this.card;
    }
}

// Define the custom button element
customElements.define('cd-landscape', Landscape);