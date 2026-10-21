**Moonfall Tactics:** Unity Turn based deckbuilder (Slay the spire clone).

This is a Slay the spire clone made in Unity and C#. This project focuses on modular, reusable gameplay system for turn based card game.

**Engineering highlights:**

**Scriptable object based cards system:** Used the scriptable objects to create card data and support reusable card configuration.
**Reusable base class for player and enemy:** Created a shared base CombatantBase base class for player and enemy, for centralized common behavior.
**Turn based combat system:** Created basic turn management system. Other systems like player and enemy shield management, cards manager uses it to draw and discard card.
**Persistent Deck Management:** Implemented customizable deck configuration and JSON based save system to store deck across session.
**Card Interaction and targeting:** Created the drag and drop system for card interaction and targeted selection with trajectory visualization.

This project demonstrates my approach to structuring gameplay code, separating responsibilities, and building reusable systems in Unity.