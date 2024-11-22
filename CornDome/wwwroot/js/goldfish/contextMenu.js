class ContextMenu extends HTMLElement {
    constructor() {
        super();
        this.target = null;
        const shadow = this.attachShadow({ mode: 'open' });

        // Create styles for the context menu
        const style = document.createElement('style');
        style.textContent = `
        /* Custom context menu styles */
        .custom-menu {
            display: none;
            position: fixed;
            background-color: #fff;
            border: 1px solid #ccc;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            z-index: 2000;
            width: 154px;
        }
        .custom-menu ul {
            list-style: none;
            margin: 0;
            padding: 0;
            z-index: 2001;
        }
        .custom-menu ul li {
            padding: 10px;
            cursor: pointer;
            color: #000 !important;
            border-bottom: 1px solid #eee;
            z-index: 2002;
        }
        .custom-menu ul li:hover {
            background-color: #f0f0f0;
        }
        .custom-menu ul li:last-child {
            border-bottom: none;
        }
        /* Dummy content */
        .content {
            width: 100%;
            height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: #f4f4f4;
        }
        `;
        shadow.appendChild(style);

        // Create the menu container
        this.menu = document.createElement('div');
        this.menu.classList.add('custom-menu');
        shadow.appendChild(this.menu);

        // Bind event handlers
        this.hideMenu = this.hideMenu.bind(this);
    }

    connectedCallback() {
        // Attach the context menu to the target element
        // Get the target ID from the attribute
        const targetId = this.getAttribute('target-id');

        if (targetId !== null && targetId !== '') {
            const target = document.getElementById(targetId);
            if (target !== null) {
                target.addEventListener('contextmenu', (event) => {
                    event.preventDefault();
                    this.showMenu(event.pageX, event.pageY, event.target);
                });
            }
        }

        const targetClass = this.getAttribute('target-class');
        if (targetClass !== null && targetClass !== '') {
            const targets = document.getElementsByClassName(targetClass);
            if (targets.length > 0) {
                targets.forEach((targ) => {
                    targ.addEventListener('contextmenu', (event) => {
                        event.preventDefault();
                        this.showMenu(event.clientX, event.clientY, event.target);
                    });
                });
            }
        }
        

        // Hide the menu when clicking outside
        document.addEventListener('click', this.hideMenu);
        document.addEventListener('close-all-menus', this.hideMenu);
    }

    disconnectedCallback() {
        const targetId = this.getAttribute('target-id');
        const target = document.getElementById(targetId);

        if (target) {
            target.removeEventListener('contextmenu', this.showMenu);
        }

        document.removeEventListener('click', this.hideMenu);
        document.removeEventListener('close-all-menus', this.hideMenu);
    }

    // Show the context menu at specified position
    showMenu(x, y, target = null) {
        this.menu.style.left = `${x}px`;
        this.menu.style.top = `${y}px`;
        this.menu.style.display = 'block';
        this.target = target;
        this.renderMenuItems();
    }

    // Hide the context menu
    hideMenu(event) {
        this.menu.style.display = 'none';
    }

    // Render the menu items
    renderMenuItems() {
        this.menu.innerHTML = ''; // Clear existing items

        var list = document.createElement('ul');

        this.items.forEach(item => {
            const menuItem = document.createElement('li');
            menuItem.classList.add('context-menu-item');
            menuItem.textContent = item.label;

            // Add click handler if defined
            if (item.action && typeof item.action === 'function') {
                menuItem.addEventListener('click', (event) => {
                    event.preventDefault();
                    item.action(this.target);
                    document.dispatchEvent(new CustomEvent('close-all-menus'));
                });
            }

            list.appendChild(menuItem);
        });

        this.menu.appendChild(list);
    }

    setMenuItems(items) {
        this.items = items;
    }
}

// Define the custom button element
customElements.define('context-menu', ContextMenu);