class DecksDatabase {
    constructor(dbName = 'carddweebdb', storeName = 'decks') {
        this.dbName = dbName;
        this.storeName = storeName;
        this.db = null;
    }

    // Open the database
    open() {
        return new Promise((resolve, reject) => {
            const request = indexedDB.open(this.dbName, 1);

            request.onupgradeneeded = (event) => {
                const db = event.target.result;
                if (!db.objectStoreNames.contains(this.storeName)) {
                    db.createObjectStore(this.storeName, { autoIncrement: true });
                }
            };

            request.onsuccess = (event) => {
                this.db = event.target.result;
                resolve(this.db);
            };

            request.onerror = (event) => {
                reject(event.target.error);
            };
        });
    }

    getAll() {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readonly');
            const store = transaction.objectStore(this.storeName);
            const request = store.openCursor(); // Use openCursor to access both key and value

            const decksWithIds = [];

            request.onsuccess = (event) => {
                const cursor = event.target.result;
                if (cursor) {
                    // Add the id (cursor.key) and the deck object (cursor.value)
                    decksWithIds.push({
                        id: cursor.key,
                        ...cursor.value
                    });
                    cursor.continue(); // Move to the next record
                } else {
                    // All decks have been fetched
                    resolve(decksWithIds);
                }
            };

            request.onerror = (event) => reject(event.target.error);
        });
    }

    updateDeck(id, updatedDeck) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readwrite');
            const store = transaction.objectStore(this.storeName);

            // Create an object with the updated data, keeping the same ID
            const request = store.put(updatedDeck, id); // Use the ID to update the record

            request.onsuccess = () => resolve(`Deck updated successfully`);
            request.onerror = (event) => reject(event.target.error);
        });
    }

    getById(id) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readonly');
            const store = transaction.objectStore(this.storeName);
            const request = store.get(id);

            request.onsuccess = (event) => {
                const result = event.target.result;
                if (result !== undefined) {
                    resolve({ id, ...result });
                } else {
                    reject(`No deck found with ID ${id}`);
                }
            };

            request.onerror = (event) => reject(event.target.error);
        });
    }

    // Add a new deck
    addDeck(deck) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readwrite');
            const store = transaction.objectStore(this.storeName);
            const request = store.add(deck);

            request.onsuccess = () => resolve('Deck added successfully');
            request.onerror = (event) => reject(event.target.error);
        });
    }

    // Delete a deck by ID
    deleteDeck(id) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readwrite');
            const store = transaction.objectStore(this.storeName);
            const request = store.delete(id);

            request.onsuccess = () => resolve(`Deck deleted successfully`);
            request.onerror = (event) => reject(event.target.error);
        });
    }

    // Delete all decks
    deleteAll() {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readwrite');
            const store = transaction.objectStore(this.storeName);
            const request = store.clear();

            request.onsuccess = () => resolve('All decks deleted successfully');
            request.onerror = (event) => reject(event.target.error);
        });
    }

    // Add multiple decks
    addMultiple(decksArray) {
        return new Promise((resolve, reject) => {
            if (!this.db) {
                reject('Database not open');
                return;
            }

            const transaction = this.db.transaction(this.storeName, 'readwrite');
            const store = transaction.objectStore(this.storeName);

            let completed = 0;
            let hasError = false;

            decksArray.forEach((deck, index) => {
                const request = store.add(deck);

                request.onsuccess = () => {
                    completed++;
                    if (completed === decksArray.length && !hasError) {
                        resolve('All decks added successfully');
                    }
                };

                request.onerror = (event) => {
                    if (!hasError) {
                        hasError = true;
                        reject(`Failed to add deck at index ${index}: ${event.target.error}`);
                    }
                };
            });

            if (decksArray.length === 0) {
                resolve('No decks to add');
            }
        });
    }
}