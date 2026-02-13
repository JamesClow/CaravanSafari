# The Last Caravan (Working Title)

## High Concept

A roguelike tower defense where you collect units by day and defend by night.

You control a hero unit that explores the map.  
During the day, you find neutral units scattered around, call them to your side, and bring them back to your home base.  
When night falls, the units you captured fight on your team as waves of enemies attack.  
The base continues moving, and you must defend it with the army you built.  

Survive long enough to reach the end. Stay too long and risk getting overwhelmed. Move too quick and risk not having enough units.

**The game is built on multiple layers of tension**—exploration vs. time, unit collection vs. defense preparation, and defending a moving target.

---

# Inspirations

- Diplomacy Is Not an Option — large wave pressure and resource tension  
- Tribes of Midgard — night escalation and boss gates  
- Bad North — minimalist tactical clarity  
- Pikmin — unit collection and management  
- Overlord — calling units to your side  

---

# Core Pillars

1. **A Moving Base**
2. **Hero-Controlled Unit Collection**
3. **Deterministic Waves**

---

# Core Gameplay Loop

## Day Phase (Exploration & Unit Collection)

- Hero unit is directly controllable (camera follows hero).
- Neutral units are scattered around the map.
- Hero explores the map, finding neutral units.
- When hero gets close to neutral units, hold a "call" button.
- Call button summons all units within a certain radius to the hero.
- Hero leads the called units back to the home base.
- Units are captured/recruited when brought to base.
- Base continues moving along the path during day phase.
- Time pressure: more units collected = stronger defense, but less time to explore.

**Key Tension:** Explore and collect units vs. return to base before night falls.

---

## Night Phase (Defense While Moving)

- **The base continues moving**—night doesn't mean stopping.
- Each day/night cycle is a **wave**—Wave 1, Wave 2, Wave 3, etc.
- Enemy waves attack from all directions as darkness falls.
- Captured units fight automatically on your team.
- Hero can still move and call units (if any remain on map).
- Survive until dawn or reach the next waypoint.

**Key Tension:** Defend a moving target with the units you collected.

---

## End of Path

- Final wave encounter.
- Complete path → Run ends successfully.
- Lose → Run ends.

---

# Core Systems

## 1. Moving Base System (Primary Hook)

The home base moves continuously along a path.

**Core Mechanics:**
- Base moves forward automatically
- Base position determines where units are deployed from
- Movement affects how many waves are faced
- Base continues moving during both day and night phases

---

## 2. Hero Control & Unit Collection

**Hero Unit:**
- Player directly controls the hero unit
- Camera follows the hero wherever they go
- Hero can walk around the map freely
- Hero is the primary unit for exploration

**Unit Calling System:**
- Neutral units are scattered around the map
- When hero gets close to neutral units, hold "call" button
- Call button summons all units within a certain radius to the hero
- Called units follow the hero
- Hero leads units back to home base
- Units are captured/recruited when brought to base

**Unit Management:**
- Captured units are stored at the base
- Units fight automatically during night phase
- Units are your primary defense resource

---

## 3. Deterministic Wave System

**Consistent Waves:**
- Wave 1 always has the same enemies, Wave 2 always has the same, etc.
- Players can learn and strategize for specific waves
- Waves escalate in difficulty
- More waves = harder enemies

---

# Tension Systems

The game creates constant pressure through three core tensions:

## 1. Exploration vs. Time

**The Core Dilemma:**
- Explore more → Find more units → Stronger defense
- Explore more → Less time to return to base → Risk being caught away from base when night falls
- Stay close to base → Safe but fewer units → Weaker defense
- Venture far → More units but risky → Stronger if you make it back

Players must balance exploration depth with time management.

---

## 2. Unit Collection vs. Defense Preparation

**The Strategic Choice:**
- Collect many units → Strong defense but takes time
- Collect few units → Fast but weak defense
- Quality vs. quantity: which units to prioritize?
- When to stop collecting and return to base?

Players must decide how many units are "enough" before returning.

---

## 3. Defending a Moving Target

**The Unique Challenge:**
- Base never stops, even during enemy attacks
- Must defend from all directions while base is in motion
- Cannot rely on static defensive positions
- Units fight automatically, but hero can still explore/collect during night

Creates constant pressure and prevents players from settling into comfortable defensive positions.

---

**The Cumulative Effect:** These three tensions compound, creating moments where players must make difficult choices with lasting consequences. The game keeps players in a state of constant decision-making under pressure.

---

# Unit Collection System

**Core Loop:**
- Neutral units scattered around map during day phase
- Hero explores and finds units
- Hero calls units within radius
- Hero leads units back to base
- Units are captured when brought to base
- Captured units fight during night phase

**Strategic Elements:**
- Which units to prioritize?
- How many units to collect before returning?
- When to stop exploring and prepare for night?
- Risk of being caught away from base when night falls

---

# Art Direction Options

Option A: Minimalist Low-Poly (Bad North clarity)  
Option B: Neon Dark Fantasy  
Option C: Thin-line pastel Moebius-inspired  

Strongest positioning:  
Low-poly with dramatic night lighting and strong silhouettes.

---

# MVP Scope

To avoid scope creep, MVP should focus on:

**Core Systems:**
- Hero unit (directly controllable, camera follows)
- Moving base (continuous movement along path)
- Unit calling system (hold button to summon units within radius)
- Unit collection (bring units to base to capture them)
- Deterministic wave system (consistent waves for learnability)
- Automatic unit combat (captured units fight automatically during night)

**Content:**
- Single path with multiple waves
- Basic neutral unit types scattered on map
- Basic enemy types for waves

**Not in MVP:**
- Multiple resource types
- Defensive structures
- Complex unit management/upgrades
- Meta progression
- Multiple biomes
- Boss encounters
- Multiplayer
- Unit abilities/specializations

**Focus:** Validate the core tension loop of exploring to collect units by day and defending a moving base with those units by night through deterministic, learnable waves.

---

# What Makes This Game Unique

- Most tower defense games use static bases.
- Most roguelikes lack base-building.
- Most unit collection games (Pikmin, Overlord) don't have tower defense mechanics.
- This game merges all three.

A moving base where you collect units by day and defend by night.

Hero exploration + unit collection + automatic defense.

Defending a moving target where your army size depends on how well you explored and collected during the day.

---

# Steam Tags (Target Audience)

- Roguelike
- Tower Defense
- Base Building
- Strategy
- Action

---

# Future Expansion Ideas

- Additional unit types with different abilities
- Unit upgrades/specializations
- Hero abilities and upgrades
- Dynamic world map instead of fixed path
- Procedural map generation
- Endless survival mode
- Multiplayer functionality
- Unit formations and tactics

---

# Summary

A roguelike tower defense game where you collect units by day and defend by night.

You explore.  
You collect.  
You survive the night—while still moving.  
You move on — or you fall.