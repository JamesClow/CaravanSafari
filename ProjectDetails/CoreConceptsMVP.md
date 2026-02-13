# The Last Caravan (Working Title)

## High Concept

A roguelike tower defense / auto-battler hybrid where your base is always moving.

You control a caravan that never stops moving.  
Each day, you move forward, gather resources, and upgrade your caravan.  
When night falls, horrors descend—but the caravan keeps moving.  
Your units fight automatically while you manage movement, resources, and upgrades.  
At the end of each path awaits a boss.  

Survive long enough to reach it. Stay too long and risk getting overwhelmed. Move too quick and risk getting pummeled by the boss.

**The game is built on multiple layers of tension**—speed vs. preparation, defending a moving target, and strategic decision-making under pressure.

---

# Inspirations

- Diplomacy Is Not an Option — large wave pressure and resource tension  
- Tribes of Midgard — night escalation and boss gates  
- Bad North — minimalist tactical clarity  
- Auto-battlers — automatic combat with strategic positioning  

---

# Core Pillars

1. **A Moving Base**
2. **Auto-Battler Combat**
3. **Deterministic Waves**

---

# Core Gameplay Loop

## Day Phase (Travel + Strategy)

- Caravan progresses along a fixed world path.
- Players gather resources and choose upgrades.
- Prepare for the next wave.

---

## Night Phase (Defense While Moving)

- **The caravan continues moving**—night doesn't mean stopping.
- Each day/night cycle is a **wave**—Wave 1, Wave 2, Wave 3, etc.
- Enemy waves attack from all directions as darkness falls.
- Units fight automatically in real-time auto-battler style.
- Players manage caravan movement and upgrades during combat.
- Survive until dawn or reach the next waypoint.

**Key Tension:** Defend a moving target with automatic combat.

---

## End of Path

- Final wave encounter.
- Complete path → Run ends successfully.
- Lose → Run ends.

---

# Core Systems

## 1. Moving Base System (Primary Hook)

The caravan is the player's base and moves continuously along a path.

**Core Mechanics:**
- Caravan moves forward automatically
- Players can control speed (speed up, slow down, stop)
- Movement affects how many waves are faced

---

## 2. Auto-Battler Combat System

**Real-Time Auto-Battler:**
- Units deploy from the caravan and fight automatically
- Players don't directly control units in combat
- Strategic focus: caravan movement and upgrade timing

**Unit System:**
- Units are acquired and upgraded between waves
- Units fight automatically based on their AI

---

## 3. Deterministic Wave System

**Consistent Waves:**
- Wave 1 always has the same enemies, Wave 2 always has the same, etc.
- Players can learn and strategize for specific waves
- Waves escalate in difficulty

---

# Tension Systems

The game creates constant pressure through three core tensions:

## 1. Speed vs. Power

**The Core Dilemma:**
- Move fast → Less time to gather/upgrade → Underprepared and underpowered → Risk getting pummeled
- Move slow → More time to gather/upgrade → More powerful → But more waves → Harder enemies → Risk getting overwhelmed
- Every detour and every upgrade choice affects speed and power

Players must find the optimal balance: powerful enough for the boss, but not so slow that late-game waves become impossible.

---

## 2. Defending a Moving Target

**The Unique Challenge:**
- Caravan never stops, even during enemy attacks
- Must defend from all directions while base is in motion
- Cannot rely on static defensive positions
- Must manage caravan movement, resources, and upgrades during combat

Creates constant pressure and prevents players from settling into comfortable defensive positions.

---

## 3. Strategic Decision-Making

**Core Decisions:**
- Caravan movement (speed affects wave count)
- Upgrade choices (what to improve and when)
- Resource allocation

Creates strategic tension without execution overload.

---

**The Cumulative Effect:** These three tensions compound, creating moments where players must make difficult choices with lasting consequences. The game keeps players in a state of constant decision-making under pressure.

---

# Resource Management

**Simple System:**
- Gather resources during day phase
- Spend resources on upgrades between waves
- Resources come from defeating enemies and gathering from environment
- Focus on meaningful upgrade choices: what to improve and when

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
- Moving caravan (continuous movement, speed control)
- Deterministic wave system (consistent waves for learnability)
- Real-time auto-battler combat (units fight automatically)
- Basic upgrade system (spend resources to improve caravan/units)
- Simple resource gathering (from enemies and environment)

**Content:**
- Single path with multiple waves
- Basic unit types
- Basic upgrade options

**Not in MVP:**
- Exploration mechanics
- Multiple resource types
- Defensive structures
- Complex unit management
- Meta progression
- Multiple biomes
- Boss encounters
- Multiplayer

**Focus:** Validate the core tension loop of defending a moving base with auto-battler combat through deterministic, learnable waves.

---

# What Makes This Game Unique

- Most tower defense games use static bases.
- Most roguelikes lack base-building.
- Most auto-battlers lack strategic movement and positioning.
- This game merges all three.

A moving base with auto-battler combat.

Strategic movement + upgrade choices.

Defending a moving target where combat is automatic but movement and upgrades matter.

---

# Steam Tags (Target Audience)

- Roguelike
- Auto-Battler
- Tower Defense
- Base Building
- Strategy

---

# Future Expansion Ideas

- Additional unit types
- Expanded upgrade system
- Faction-based caravan themes
- Dynamic world map instead of fixed path
- Procedural biome generation
- Endless survival mode
- Multiplayer functionality

---

# Summary

A roguelike tower defense game where the base never stands still.

You travel.  
You upgrade.  
You survive the night—while still moving.  
You move on — or you fall.