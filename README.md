# 🌲 Dark Forest RPG

**Dark Forest RPG** is a turn-based, stat-driven dark fantasy game built with a focus on immersive design, vertical slice architecture, and magical atmosphere.

## 🧙‍♂️ Game Overview

- 🎲 **Turn-Based Combat**: Players cast spells, manage inventory, and battle foes using a stat-focused system.
- 🧩 **No Graphics Engine**: The game focuses on numerical gameplay and interactivity rather than graphical exploration.
- 🧪 **Spells & Effects**: Clickable, stat-triggered spell system, with layered effects and conditions.

## 🛠️ Technologies

| Area           | Stack/Tool                        |
|----------------|-----------------------------------|
| Frontend       | Angular (SCSS planned)            |
| Backend        | ASP.NET Core MVC controllers      |
| Architecture   | Vertical Slice                    |
| Real-time      | SignalR                           |
| Database       | MongoDB and PostgreSQL            |
| Caching        | Redis                             |

## 🧩 What Is Implemented

This project is still in early development, but the following systems are already functional or prototyped:

### 🔐 Authentication
- JWT-based login system

### 🎒 Inventory System (Prototype)
- Dynamic equipment with multiple attributes
- Equip/Unequip logic fully working

### ⚔️ Battle System (Prototype)
- Turn-based UI (desktop-friendly, not mobile-optimized 😅)
- Only PVE (no multiplayer yet)
- Future: damage pop-ups & animations

### 🎁 Item Generation
- Procedural item/loot creation
- Rule-based and random generation

### 🏆 Reward System
- Receive XP, gear, and loot after battles

### 💀 Lose Conditions
- Game over on defeat (0 HP = you're toast)

### 🔮 Ability System (Early)
- Usable abilities with status effects
- Debuffs like **bleed** implemented

### 👹 Monster Management
- Create/delete monsters
- Architecture still evolving


Feel free to contribute or suggest improvements via issues or pull requests!

🔮 *May the forest spirits guide your code...*
