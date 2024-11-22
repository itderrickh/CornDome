﻿class Building extends HTMLElement {
    constructor() {
        super();
        this.flooped = false;
        this.card = null;
        this.attachShadow({ mode: 'open' });

        this.shadowRoot.innerHTML = `
        <style>
        .building {
            position: relative;
            height: 200px;
            width: 133px;
            margin: 0 auto;
        }
        .flooped {
            transform: rotate(0.25turn);
        }
        </style>
        <context-menu target-class="building" class="building-menu"></context-menu>
        <div class="building" ondrop="this.dispatchEvent(new CustomEvent('card-drop', { detail: event, bubbles: true }));" ondragover="event.preventDefault()">
            <div class="card-drop"></div>
        </div>
        `;
    }

    static get observedAttributes() {
        return ['data-building'];
    }

    connectedCallback() {
        this.shadowRoot.querySelector('.building').setAttribute('data-building', this.getAttribute('data-building'));

        const dropZone = this.shadowRoot.querySelector('.building');
        dropZone.addEventListener('drop', this.handleDrop.bind(this));
    }

    disconnectedCallback() {
        const dropZone = this.shadowRoot.querySelector('.building');
        dropZone.removeEventListener('drop', this.handleDrop);
    }

    handleDrop(event) {
        event.preventDefault();
        const rawData = event.dataTransfer.getData('text');
        const card = JSON.parse(decodeURIComponent(rawData));

        if (this.card) {
            this.discard();
        }

        this.setCard(card);
        this.dispatchEvent(new CustomEvent('building-card-drop', {
            detail: {
                building: this.getAttribute('data-building'),
                cardData: card,
            },
            bubbles: true,
            composed: true
        }));
    }

    setCard(card) {
        this.card = card;

        const cardDrop = this.shadowRoot.querySelector('.card-drop');
        cardDrop.innerHTML = "";
        let template = handCardTemplate(card, false);
        cardDrop.insertAdjacentHTML("beforeend", template);

        this.buildingMenu = this.shadowRoot.querySelector('.building-menu');
        const buildingMenuItems = [
            { label: 'Floop', action: this.floop.bind(this) },
            { label: 'Discard', action: this.discard.bind(this) },
            { label: 'To Hand', action: this.toHand.bind(this) }
        ];
        this.buildingMenu.setMenuItems(buildingMenuItems);

        const buildingElement = this.shadowRoot.querySelector('.building');
        buildingElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();  // Disable default browser context menu
            this.buildingMenu.showMenu(event.pageX, event.pageY, event.target);
        });
    }

    resetBuilding() {
        this.card = null;
        this.flooped = false;
        const cardDrop = this.shadowRoot.querySelector('.card-drop');
        cardDrop.innerHTML = "";
    }

    discard() {
        document.dispatchEvent(new CustomEvent('discard-card', { detail: this.card }));
        this.resetBuilding();
    }

    toHand() {
        document.dispatchEvent(new CustomEvent('to-hand', { detail: this.card }));
        this.resetBuilding();
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
}

// Define the custom button element
customElements.define('cd-building', Building);