# Oasis RPG Online - Client

Welcome to the **Oasis RPG Online** client repository. This project is a modular Unity game engine based on the **MVC (Model-View-Controller)** pattern, designed to decouple network logic, UI, and entity processing services.

This client is built to work in conjunction with the [oasis_emulator](https://github.com/felipecorreiasilva/oasis_emulator), which serves as the absolute authority for game state and business logic.

## MVC Architecture (App/ Layer)
The `App/` folder orchestrates system interactions:

* **Network/ (Packet Handlers):** Pure communication layer. Implements the `Clif` (Client Interface) pattern for packet processing (`Login`, `Char`, `Map`), acting as the network's model.
* **UI/ (View):** Visualization layer containing the HUD and contextual windows.
* **Controllers/ (Controller):** Bridges that receive network events, execute logic in the `Services/` layer, and update the `UI/`.

## Services/ (Engine Brain)
Where the "truth" of the game resides. These services process raw data and manage the world state.

* **Core/**: Base infrastructure. Contains `NetworkManager` (socket connections) and `DataManager` (config loading, e.g., `Assets/data/clientinfo.xml`).
* **Entities/ (BL - Block List)**: Entity managers (rAthena-style):
    * `Character/`: `PCManager`, `MobManager`, `NpcManager`.
    * `Items/`: `ItemManager`.
    * `WorldObjects/`: `PortalManager`.
* **Combat/**: Combat calculation rules (`SkillManager`, `StatusManager` for `SC_`).
* **Effects/**: Visual feedback rules (`EffectManager` for `EFST_`, `TooltipManager`).
* **Content/**: Progression content (`QuestManager`, `DatabaseManager`).

---

## Connection Configuration
The client uses **`Assets/data/clientinfo.xml`** to define connection settings (IP/Port). The `DataManager` automatically parses this file on startup, allowing the `NetworkManager` to connect without hardcoded values.

---

## Communication Flow
1. **Reception:** `NetworkManager` (`Services/Core/`) receives socket data and forwards it to Handlers in `App/Network/`.
2. **Processing:** The `Controller` receives the signal, consults the `Services/` (Model) to apply business rules.
3. **Visualization:** The `Controller` fires an event, which the `UI/` (View) listens to, updating the player's screen.

---

## Project Conventions
- **rAthena Pattern:** Naming conventions like `Clif`, `BL`, and `Opcodes` follow the official emulator standard.
- **Lowercase:** All folder and file names follow a lowercase pattern (`data/`, `luafiles/`).
- **BL (Block List):** Any world object with a unique server ID is an `Entity` and must reside in `Services/Entities/`.

---

## How to Contribute
1. **Respect MVC:** The `UI/` must never access `Network/` directly. Always use a `Controller/` as an intermediary.
2. **Encapsulation:** New systems should be implemented as `Managers` within `Services/`.
3. **Security:** The client does not contain critical validation logic. The [oasis_emulator](https://github.com/felipecorreiasilva/oasis_emulator) is the absolute authority on game state.

---
*Developed for the Oasis RPG ecosystem.*