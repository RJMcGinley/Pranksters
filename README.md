Prankster’s Revenge

Prankster’s Revenge is a turn-based card game built in Unity using C#. Players compete to complete pranks by assembling specific card sets, managing favor, and strategically interacting with opponents. Final scores are determined by a combination of prank (renown) points and favor points.

Current Status

Prankster’s Revenge is in a playable prototype with fully implemented core systems and progression mechanics.

Core Systems Implemented
Turn-based gameplay loop
Multiplayer support (2–4 players, Human + AI)
Prank completion system (set collection)
Favor system (resource generation through card offering)
Swap mechanic (interact with opponent favor areas)
Persistent save system
Player statistics tracking
Unlock progression system (dual-path)
Interactive UI with contextual highlights and previews
Endgame scoring and results display
Progression System

The game features a dual unlock system tied to player behavior:

Prank Completion Unlocks

Unlocked by completing specific prank types:

Tier 1: Crew Leader (+1 renown)
Tier 2: Expert (+3 renown)
Tier 3: Master (+5 renown)
Favor Offer Unlocks

Unlocked by accumulating favor points per prankster type:

Tier 1: Assistant (+1 favor)
Tier 2: Strategist (+3 favor)
Tier 3: Advisor (+5 favor)

Unlocks are:

Persisted between sessions
Ordered by acquisition
Conditionally usable (free vs full version)
In Progress
AI decision-making refinement
UI polish and animation pass
Visual identity for unlock cards (Assistant / Strategist / Advisor)
Planned
Advanced AI behavior system
Expanded audio design and feedback
Deckbuilding / pre-game customization (potential)
Distribution pipeline (Steam + mobile)
How to Play (Prototype)

Each turn, a player may:

Draw from the deck
Draw from the discard pile
Offer a prankster as favor (gain favor points)
Swap favor cards with an opponent
Complete a prank if requirements are met
Goal

Maximize your final score by:

Completing pranks (renown points)
Generating favor efficiently
Leveraging unlock bonuses
Controls
Mouse: Hover and click UI elements
Highlighted areas indicate valid actions
Hover panels provide contextual previews
Tech Stack
Unity (C#)
Custom turn-based architecture
Data-driven progression and unlock system
Modular UI system with dynamic highlight logic
Project Goals
Deliver tight, decision-driven gameplay with minimal downtime
Emphasize player agency through meaningful choices each turn
Blend tactical interaction with long-term progression
Build toward a full digital release (Steam + mobile)
Notes

The project began as a console-based prototype and has evolved into a fully interactive, UI-driven card game with persistent progression systems.
