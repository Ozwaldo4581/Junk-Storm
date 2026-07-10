# Junk Storm — Playable Ruleset v0.1


## Unity Prototype

A lightweight two-player Unity prototype is included for early loop testing. It supports the first pass of the core flow: choosing locations, scavenging cards, recycling or discarding hand cards for resources, building early infrastructure, resolving the Junk Storm/Biodomers die, and resetting into the next generation.

The prototype includes a top info banner. Hover over generated buttons to see card, building, and action explanations before choosing them. Mixed Alloy/Plastoid building costs are auto-paid with Alloy first, then Plastoid. Stored materials persist, while temporary Labor resets during the Reset Phase. The New Game button now lives in a top-right utility area next to an indexed in-game Rulebook button for reviewing rules during play. The main play area now uses a three-column layout: generation/player info on the left, cards/hand/actions in the center, and a pentagon Location board on the right. Location card counts and threat indicators appear directly on the pentagon Location buttons, threat movement is shown step-by-step with direction arrows, and the event log reports specific movement, worker loss, Clout loss, defense use, and Location deck damage. The center panel now includes a Shop / Build window that lists all Tier 1, Tier 2, and Tier 3 buildings; greyed-out entries remain hoverable/clickable so players can see costs, requirements, and why a purchase is unavailable. During Expedition, click a Location button once per worker you want to assign, then Confirm Expedition; use Skip Expedition to send 0 workers, or use Clear Expedition Selection / Remove 1 Worker to adjust before confirming. The prototype also shows the rotating First Player and turn order, and players sharing a Location can use Soldier cards for attacks and defense.

To run it locally, open this repository as a Unity project and load `Assets/Scenes/JunkStormPrototype.unity`. The scene contains a `JunkStormUnityController` entry point that builds the prototype UI at runtime.

## 1. Game Overview

The planet is ruined. The wealthy have fled into protected biodomes, leaving the rest of humanity to survive in the wreckage. Each player is an Influencer trying to gather followers, scavenge resources, build colony infrastructure, survive the Junk Storm, and prove they deserve to lead humanity’s future.

Junk Storm is a competitive deckbuilding board game for 2–4 players.

Players build personal decks by scavenging at locations, use resources to construct buildings, gain Clout, and race to complete a Tier 3 future-defining project.

## 2. Goal of the Game

To win, a player must meet both conditions at the end of a generation:

1. Have at least **10 Clout**.
2. Build **one Tier 3 building**.

The first player to meet both conditions wins.

If multiple players meet the victory condition in the same generation, the player with the highest Clout wins. If still tied, the tied player with the most Tier 2 buildings wins. If still tied, they share power over the miserable future of humanity.

## 3. Key Terms

**Clout**
Clout represents social influence, morale, leadership, and political power. It determines how many workers a player can bring on expeditions and whether they qualify for advanced buildings.

All players start with **1 Clout**. During the Action Phase, a player may voluntarily spend 1 Clout to gain 1 worker token, or spend 1 Clout to add 1 Worker card to their Recycle Pile. Voluntary spending may reduce a player to 0 Clout without eliminating them. Forced Clout loss from hazards, Biodomers, attacks, or other non-chosen effects can reduce Clout to 0; when it does, that player is immediately eliminated.

**Workers**
Workers are people willing to follow a player into danger. Workers are represented by tokens. Workers are used during expeditions and may die to the Junk Storm, Biodomers, events, or attacks.

Each player starts with **3 worker tokens**.

**Recycle**
When a card is recycled, use its recycle ability and place it in your Recycle Pile. Recycle Pile cards remain in your deck cycle and are shuffled into a new Draw Pile when needed.

**Discard**
When a card is discarded, use its discard ability and place it in the Discard Pile, permanently removed from the game. The Discard Pile is never reshuffled into the Draw Pile.

**Generation**
A generation is one complete round of play consisting of four phases:

1. Expedition Phase
2. Action Phase
3. Junk Storm Phase
4. Reset Phase

## 4. Components

For a first prototype, use:

* 1 Colony board
* 5 Location spaces
* 1 Wilderness space
* 1 Biodome space
* 1 Junk Storm token
* 1 Biodomer token
* Worker tokens
* Clout trackers
* Player decks
* Location scavenge decks
* Biodome special deck
* Building cards or tiles
* Character cards

## 5. Setup

Each player chooses or is dealt one starting character.

Each player begins with:

* 1 Clout
* 3 worker tokens
* A 7-card starter deck:
  * 1 Dweller
  * 1 Worker
  * 1 Soldier
  * 1 Storm Shield
  * 1 Alloy
  * 1 Organics
  * 1 Plastoid

Shuffle each player’s starter deck.

Each player draws **5 cards**.

Place the Junk Storm token on Location 1.

Place the Biodomer token on Location 4.

Shuffle each Location deck and place it near the matching Location.

Shuffle the Biodome special deck and place it near the Biodome.

Randomly determine first player. First player rotates clockwise at the end of each generation.

## 6. Starter Cards

### Dweller

Recycle: Gain 1 Clout.
Discard: Gain 2 Clout.

### Worker

Recycle: Gain 1 Labor this generation.
Discard: Gain 2 Labor this generation.

Labor is a temporary generation resource used to build buildings. The Worker card is different from worker tokens. Worker tokens are followers sent on expeditions; the Worker card is a deck card with recycle and discard effects.

### Soldier

Recycle: Gain 1 Defense Strength this generation.
Discard: Choose one: cancel one Biodomer attack against you, attack another player at your Location, or defend yourself or another player at your Location from a Soldier attack.

### Storm Shield

Recycle: Draw 1 card.
Discard: Cancel all Junk Storm effects against you this generation.

### Alloy

Recycle: Generate 1 Alloy.
Discard: Generate 2 Alloy.

### Organics

Recycle: Generate 1 Organics.
Discard: Generate 2 Organics.

### Plastoid

Recycle: Generate 1 Plastoid.
Discard: Generate 2 Plastoid.

Alloy, Organics, and Plastoid are stored resources. They persist between turns and generations until spent on buildings. Labor is temporary and resets during the Reset Phase.

## 7. Character Cards

Each character has a starting bonus and one special action that may be used once per generation.

### Albus — The Scientist

Starting Bonus: Add 1 Alloy and 1 Plastoid to your starter deck.
Special Action: Once per generation, treat 1 Alloy as 1 Plastoid, or 1 Plastoid as 1 Alloy.

### Luvana — The Naturopath

Starting Bonus: Add 2 Organics to your starter deck.
Special Action: Once per generation, treat 1 Organics as 2 Organics.

### Duncan — The Soldier

Starting Bonus: Start with +2 Clout.
Special Action: Once per generation, bring 1 extra worker on an expedition beyond your Clout limit.

### Juanita — The Politician

Starting Bonus: Start with +2 Clout.
Special Action: Once per generation, reduce one building’s Clout requirement by 2.

### Gnar’wan — The Doomsday Prepper

Starting Bonus: Draw 2 extra cards during the first generation.
Special Action: Once per generation, reduce one building’s resource cost by 1 resource of your choice.

### Jo — The Lucky Basement Gamer

Starting Bonus: Draw 2 extra cards during the first generation.
Special Action: Once per generation, reduce one Clout requirement by 2.

## 8. The Generation Structure

Each generation has four phases.

---

# Phase I — Expedition Phase

## Step 1: Choose Expedition Bonus

In turn order, each player may choose one available Expedition Bonus from a building.

Tier 1 buildings only benefit the player who built them.

Tier 2 buildings are colony buildings. Any player may use a Tier 2 Expedition Bonus, and multiple players may choose the same Tier 2 bonus during the same generation.

A player may choose only one Expedition Bonus per generation.

## Step 2: Declare Expedition

In turn order, starting with the current First Player, each player may send workers to one Location, the Biodome, or the Wilderness.

A player may bring workers up to their current Clout, or may send 0 workers and skip their expedition.

Example: A player with 4 Clout may bring up to 4 workers.

In the Unity prototype, click a valid Location once to assign 1 worker, click the same Location again to assign another worker, or click a different valid Location to switch destinations and reset the assignment to 1 worker. Click **Confirm Expedition** to send the assigned workers, or **Skip Expedition** to send 0 workers without selecting a Location. Click **Clear Expedition Selection** to start over, or **Remove 1 Worker** to reduce the assignment.

A player cannot go to a Location with the Junk Storm or Biodomers; that Location is unsafe.

## Step 3: Resolve Expedition

### Scavenging at a Location

For each worker you brought, draw 1 card from that Location’s scavenge deck.

Example: If you brought 3 workers, draw 3 scavenge cards.

If you draw an Event card, resolve it immediately and discard it.

All non-Event cards you scavenge go into your Recycle Pile unless an Expedition Bonus says otherwise.

### Invading the Biodome

To invade the Biodome, you must bring at least 5 workers and have at least 5 Clout.

When you invade:

* Lose all 5 workers.
* Lose 5 Clout.
* Draw 1 card from the Biodome special deck and place it in your Recycle Pile.

This is forced Clout loss. If it reduces your Clout to 0, you are immediately eliminated.

### Entering the Wilderness

The Wilderness has no scavenge deck.

While in the Wilderness:

* You cannot be targeted by Attack cards.
* You cannot use Defense cards.
* You are still vulnerable to the Junk Storm and Biodomers.

The Wilderness is mainly used for the optional “Legendary Exodus” victory variant.

---

# Phase II — Action Phase

All players continue using the cards in their current hands.

In turn order, each player may do the following:

1. Play any number of cards from hand.
2. Use Recycle or Discard abilities.
3. Spend 1 Clout to gain 1 worker token or spend 1 Clout to add 1 Worker card to your Recycle Pile.
4. Build up to one building.
5. Use one character special action, if available.
6. Use Attack, Defense, or Passive cards when appropriate.

Players may play as many cards as they want, but each card must be either recycled or discarded when played. Recycled cards return through future reshuffles; discarded cards are permanently removed.

## Soldier Conflict

During the Action Phase, a player at a non-Colony Location may discard a Soldier card to attack another player at the same Location. If the attack is undefended, it succeeds: the attacker steals 1 random card from the target player's hand, if one is available, and places it in their Recycle Pile. The target also loses 1 Clout whether or not a card was available to steal. The target may discard a Soldier from hand to cancel the attack; if the target cannot defend, another player at that same Location may discard Soldier to defend the target. Because this is forced Clout loss, the target is immediately eliminated if the attack reduces their Clout to 0. Soldier attacks are not allowed at the Colony.

## Building Limit

Each player may build only **one building per generation**.

## Stored Resources and Temporary Labor

Alloy, Organics, and Plastoid gained from Resource cards are added to a player's stored resources. Stored resources persist between turns and generations and are spent when buildings are built. Labor is generated by Worker cards, is spent on building costs, and resets to 0 during the Reset Phase.

---

# Phase III — Junk Storm Phase

Roll 1d10.

If the result is **1–5**, move the Junk Storm.

If the result is **6–10**, move the Biodomers.

## Direction

Odd result: move counterclockwise.
Even result: move clockwise.

## Distance

Move the token a number of Location spaces equal to:

* Junk Storm: the number rolled.
* Biodomers: the number rolled minus 5.

Example: A roll of 8 means the Biodomers move 3 spaces clockwise.

## Junk Storm Effects

The Junk Storm affects every Location it passes over, including the space where it lands.

For each Location the Junk Storm passes over:

* Discard the top card of that Location’s scavenge deck.

If a player is at an affected Location:

* That player discards the top card of their Draw Pile.
* All workers that player brought on the expedition die.
* The player loses 1 Clout for each worker lost.

A player may discard a Storm Shield from hand to cancel all Junk Storm effects against them for that generation.

## Biodomer Effects

The Biodomers attack any player at the Location where they land.

When attacked by Biodomers, a player loses 2 workers.

For each worker lost this way, lose 1 Clout.

A player may discard a Soldier or Weapon Outfitting card from hand to cancel the Biodomer attack.

If a player has fewer than 2 workers, they lose all remaining workers and lose Clout equal to the number of workers lost.

---

# Phase IV — Reset Phase

Each player may move any number of cards remaining in hand to their Recycle Pile.

All surviving workers return to their player.

All player pieces return to the Colony.

Clear temporary flags. Stored resources remain, and temporary Labor resets to 0.

Check victory conditions.

Rotate first player clockwise.

Each player draws back up to 5 cards.

If a player’s Draw Pile runs out, shuffle their Recycle Pile to form a new Draw Pile. Never shuffle the Discard Pile back into the Draw Pile.

---

## 9. Locations

Each Location has its own scavenge deck.

### Location 1 — City Ruins

Contains: Alloy, Organics, Plastoid, Attack cards, Event cards.

### Location 2 — Suburbs

Contains: Organics, Plastoid, Dweller-style cards, Passive cards, Event cards.

### Location 3 — Abandoned Research Facility

Contains: Alloy, Plastoid, Laboratory cards, Passive cards, Defense cards.

### Location 4 — Abandoned Military Base

Contains: Alloy, Plastoid, Soldier cards, Weapon cards, Attack cards.

### Location 5 — Farmlands

Contains: Organics, Worker cards, Dweller cards, Event cards.

### Wilderness

No scavenge deck.

### Biodome

Contains only Special cards.

## 10. Recommended Prototype Location Decks

For the first playtest, make each Location deck 15 cards.

### City Ruins Deck

* 3 Alloy
* 3 Organics
* 3 Plastoid
* 2 Thieving Resources
* 2 Spreading Rumors
* 1 Unexpected Haul
* 1 Trouble Returning Home

### Suburbs Deck

* 4 Organics
* 3 Plastoid
* 2 Dweller
* 2 Resource Trader
* 2 Silver-Tongued
* 1 Rescuing New Colonists
* 1 Unforeseen Death

### Research Facility Deck

* 4 Plastoid
* 3 Alloy
* 2 Expert Scavenger
* 2 Adept Planner
* 2 Preemptive Intelligence
* 1 Unexpected Haul
* 1 Trouble Returning Home

### Military Base Deck

* 4 Alloy
* 3 Plastoid
* 2 Soldier
* 2 Weapon Outfitting
* 2 Defecting Workers
* 1 Sabotaged Defenses
* 1 Unforeseen Death

### Farmlands Deck

* 7 Organics
* 2 Worker
* 2 Working Man’s Man
* 2 Augmented Resource
* 1 Rescuing New Colonists
* 1 Unforeseen Death

## 11. Card Types

## Resource Cards

Resource cards add permanent stored resources used for buildings. Stored Alloy, Organics, and Plastoid persist between generations until spent. Labor is generated by Worker cards and resets each generation.

### Alloy

Recycle: Generate 1 Alloy.
Discard: Generate 2 Alloy.

### Organics

Recycle: Generate 1 Organics.
Discard: Generate 2 Organics.

### Plastoid

Recycle: Generate 1 Plastoid.
Discard: Generate 2 Plastoid.

---

## Attack Cards

Attack cards may be played against other players.

A player may cancel an Attack card by revealing and recycling Preemptive Intelligence.

### Thieving Resources

Play immediately after another player scavenges. That player reveals the cards they scavenged. Choose one Resource card they scavenged and place it in your Recycle Pile.

### Spreading Rumors

Play at any time during the Action Phase. One player loses 1 Clout.

### Forcing Options

Play during another player’s Action Phase. That player discards 2 cards from hand.

### Defecting Workers

Play when another player declares an expedition. That player brings 1 fewer worker on that expedition. The removed worker does not die and returns to that player.

### Sabotaged Defenses

Play when another player uses a Storm Shield, Soldier, or Weapon Outfitting card. Cancel that defense card.

Sabotaged Defenses cannot cancel Preemptive Intelligence.

---

## Defense Cards

Defense cards protect against attacks, the Junk Storm, or Biodomers.

### Storm Shield

Discard: Cancel all Junk Storm effects against you this generation.

### Weapon Outfitting

Discard: Cancel one Biodomer attack against you.

### Preemptive Intelligence

Recycle: Cancel one Attack card targeting you.

---

## Passive Cards

Passive cards may be revealed from hand when their effect applies. After being used, recycle them.

### Expert Scavenger

When you scavenge, draw 2 additional scavenge cards.

### Resource Trader

Treat one Resource card in your hand as a different resource type this generation.

### Working Man’s Man

Bring up to 2 extra workers on an expedition beyond your Clout limit.

### Frugal Builder

Reduce the cost of one Tier 1 or Tier 2 building by 1 resource of your choice.

A building’s cost cannot be reduced below 1 total resource.

### Augmented Resource

Treat one Resource card in your hand as producing one additional resource of its type.

### Silver-Tongued

Treat your Clout as 1 higher this generation.

### Adept Planner

Recycle: Draw 2 cards.

### Recycled Technologies

Recycle: Take one Action, Defense, or Passive card from your Recycle Pile and place it into your hand.

---

## Event Cards

Event cards resolve immediately when scavenged, then are discarded.

### Unforeseen Death

Lose 1 worker and 1 Clout.

### Rescuing New Colonists

Gain 1 worker token and 1 Clout.

### Unexpected Haul

Look at the top 2 cards of the current Location deck. Choose one and place it in your Recycle Pile. Shuffle the other back into the Location deck.

### Trouble Returning Home

Discard 1 card from your hand.

---

## Special Cards

Special cards are found only in the Biodome deck.

### Extra Scavenge

After you complete an expedition, immediately take a second expedition with any surviving workers.

### Junk Storm Master

After the Junk Storm/Biodomer die is rolled, change the result by +1 or -1.

### Advanced Heist Technology

At the beginning of the Action Phase, choose another player. Look at their hand and steal one card. Place it in your hand.

This cannot be defended against.

### Sabotaged Expedition

At the beginning of the Expedition Phase, secretly choose one building. If any player chooses that building’s Expedition Bonus this generation, they receive no Expedition Bonus instead.

This cannot be defended against.

## 12. Buildings

Buildings come in three tiers.

Tier 1 buildings are personal buildings. They are built in a player’s camp.

Tier 2 buildings are colony buildings. They benefit everyone, but the player who builds one receives an immediate Clout bonus.

Tier 3 buildings are victory projects.

## Tier 1 Buildings

Each player may build up to 4 Tier 1 buildings.

Each Tier 1 building costs stored resources and 1 Labor.

When you build a Tier 1 building, gain 2 Clout.

### Farm

Cost: 4 Organics + 1 Labor
Player Bonus: Gain 2 Clout.
Expedition Bonus: When scavenging, you may place any Organics you scavenge on top of your Draw Pile instead of in your Recycle Pile.

### Military Base

Cost: 3 Alloy, 1 Plastoid + 1 Labor
Player Bonus: Gain 2 Clout.
Expedition Bonus: If Biodomers attack you this generation and you cannot defend, lose 1 fewer worker.

### Laboratory

Cost: 1 Alloy, 3 Plastoid + 1 Labor
Player Bonus: Gain 2 Clout.
Expedition Bonus: If the Junk Storm affects you this generation and you cannot defend, discard 1 fewer card from your Draw Pile.

### Transportation Station

Cost: 2 Alloy, 2 Plastoid + 1 Labor
Player Bonus: Gain 2 Clout.
Expedition Bonus: When scavenging, you may place any Alloy or Plastoid you scavenge on top of your Draw Pile instead of in your Recycle Pile.

### Town Hall

Cost: 3 Organics, 1 Alloy or 1 Plastoid + 1 Labor
Player Bonus: Gain 2 Clout.
Expedition Bonus: If another player declares an expedition to your Location after you, draw 2 additional scavenge cards.

### Apartment Building

Cost: 3 Organics, 1 Alloy or 1 Plastoid + 1 Labor
Player Bonus: Gain 2 Clout.
Expedition Bonus: Bring 2 additional workers on your expedition beyond your Clout limit.

---

## Tier 2 Buildings

Only one copy of each Tier 2 building may be built.

Each Tier 2 building requires:

* One matching Tier 1 building.
* One additional Tier 1 building.
* At least 6 Clout.
* The listed stored resources.
* 2 Labor.

When you build a Tier 2 building, gain 2 Clout.

### Farming Center

Requirement: Farm + any other Tier 1 building
Cost: 6 Organics + 2 Labor
Builder Bonus: Gain 2 Clout.
Colony Bonus: Each player gains 1 worker token.
Expedition Bonus: For each worker you bring, scavenge 1 additional card.

### Weapons Factory

Requirement: Military Base + any other Tier 1 building
Cost: 4 Alloy, 3 Plastoid + 2 Labor
Builder Bonus: Gain 2 Clout.
Colony Bonus: Each player adds 1 Weapon Outfitting card to their Recycle Pile.
Expedition Bonus: You are unaffected by Biodomers this generation.

### Internet Servers

Requirement: Laboratory + any other Tier 1 building
Cost: 3 Alloy, 4 Plastoid + 2 Labor
Builder Bonus: Gain 2 Clout.
Colony Bonus: Once per generation, each player may reduce a building cost by 1 Alloy or 1 Plastoid.
Expedition Bonus: You are unaffected by the Junk Storm this generation.

### Subway Station

Requirement: Transportation Station + any other Tier 1 building
Cost: 2 Organics, plus 3 total Alloy/Plastoid in any combination + 2 Labor
Builder Bonus: Gain 2 Clout.
Colony Bonus: Each player may bring 1 additional worker on expeditions.
Expedition Bonus: You may place any scavenged cards on top of your Draw Pile instead of in your Recycle Pile.

---

## Tier 3 Buildings

To build a Tier 3 building, you must have:

* At least 10 Clout.
* The required Tier 2 building.
* The listed stored resources.
* 3 Labor.

Building a Tier 3 building does not immediately win the game. Victory is checked at the end of the generation after the Junk Storm Phase.

### Terraforming Station

Requirement: Farming Center
Cost: 6 Organics, 2 Alloy, 2 Plastoid + 3 Labor
Victory Meaning: Terraform the planet and make Earth livable again.

### Rocket Launch Pad

Requirement: Weapons Factory
Cost: 6 Alloy, 4 Plastoid + 3 Labor
Victory Meaning: Escape Earth and build humanity’s future elsewhere.

### Deep Earth Drill

Requirement: Subway Station
Cost: 4 Alloy, 4 Plastoid, 2 Organics + 3 Labor
Victory Meaning: Tunnel beneath the ruined world and create a hidden civilization.

### Advanced Communications Array

Requirement: Internet Servers
Cost: 3 Alloy, 6 Plastoid, 1 Organics + 3 Labor
Victory Meaning: Hack the nanosphere and take control of the Junk Storm.

## 13. Optional Variant — Legendary Exodus Victory

This is an alternate victory condition for later playtests.

A player may attempt the Legendary Exodus if they have at least 15 Clout.

To attempt it:

1. Travel to the Wilderness with workers equal to your Clout.
2. Take no Expedition Bonus.
3. Remain in the Wilderness through the next full generation.
4. You may not use Defense cards while in the Wilderness.
5. You may not be targeted by Attack cards while in the Wilderness.
6. If you survive the Junk Storm and Biodomers without losing any workers, you win.

You return as a legend. A terrifying, charismatic, probably doomed legend.

## 14. Recommended First Playtest

For the first test game, use these restrictions:

* Play with 2–3 players.
* Ignore the Wilderness victory.
* Use only one Tier 3 building requirement to win.
* Use only the listed cards.
* Do not add extra cards until the basic loop works.
* Track how often players feel stuck, how often workers die, and whether Clout rises too quickly or too slowly.

## 15. First Balance Questions

After the first playtest, answer these:

1. Did players reach 10 Clout too quickly?
2. Did the Junk Storm feel scary but fair?
3. Did players have enough ways to recover after losing workers?
4. Did buildings feel worth the cost?
5. Did each Location feel meaningfully different?
6. Did the Biodome feel tempting enough?
7. Did the game end before it became repetitive?
8. Did players feel like they had meaningful choices each generation?

Do not balance everything before testing. The goal of v0.1 is to find the fun loop, not perfect the numbers.
