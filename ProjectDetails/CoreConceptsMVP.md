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
2. **Roguelike Progression**
3. **Basic Upgrades**
4. **Escalating Night Pressure**
5. **Resource Management & Economy**

---

# Core Gameplay Loop

## Day Phase (Travel + Strategy)

- Caravan progresses along a fixed world path.
- Players can:
  - Explore off-path areas (risk/reward) 
  - Gather resources
  - Choose upgrades and prepare for night.

---

## Night Phase (Defense While Moving)

- **The caravan continues moving**—night doesn't mean stopping.
- Each day/night cycle is a **wave**—Wave 1, Wave 2, Wave 3, etc.
- Enemy waves attack from all directions as darkness falls.
- Units fight automatically in real-time auto-battler style.
- Players:
  - Control caravan movement (speed up, slow down, stop)
  - Place limited defensive structures
  - Manage resources and upgrades during combat
- Survive until dawn or reach the next waypoint.

**Key Tension:** Defend a moving target while managing caravan and resources.

---

## End of Path

- Arena-style boss encounter.
- Defeat boss → Progress to next biome.
- Lose → Run ends.

---

# Core Systems

## 1. Moving Base System (Primary Hook)

The caravan is the player's base and moves continuously along a path.

**Basic Caravan Components:**
- Core caravan structure
- Upgradeable defensive elements
- Unit deployment positions

**Upgrade System:**
- Players can upgrade caravan components between waves
- Upgrades improve defense, unit capacity, or special abilities
- Simple, clear upgrade choices rather than complex reconfiguration

---

## 2. Auto-Battler Combat System

**Real-Time Auto-Battler:**
- Units deploy from the caravan and fight automatically
- Players don't directly control units in combat
- Strategic decisions happen before and during combat:
  - When to deploy units
  - Caravan positioning and movement
  - Resource allocation for upgrades
  - Defensive structure placement

**Unit Management:**
- Units are acquired and upgraded between waves
- Different unit types have different roles (melee, ranged, support)
- Units fight automatically based on their AI and positioning

---

## 3. Roguelike Structure

Each run includes:
- Deterministic wave compositions (Wave 1 always has the same enemies, Wave 2 always has the same enemies, etc. per biome)
- Random relics
- Different caravan upgrades
- Variable biome modifiers

Meta progression between runs:
- Unlock new unit types
- Unlock new upgrade options
- Expand caravan capacity
- Unlock new biomes

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

**Multiple Priorities:**
- Must balance: caravan movement, resource gathering, upgrade choices, defensive structure placement
- Auto-battler combat means focus is on strategic decisions, not execution
- Limited resources force meaningful choices about what to upgrade and when

Creates strategic tension without execution overload.

---

**The Cumulative Effect:** These three tensions compound, creating moments where players must make difficult choices with lasting consequences. The game keeps players in a state of constant decision-making under pressure.

---

# Strategic Systems

## Exploration

**Simple Risk/Reward:**
- Explore off-path areas for resources and upgrades
- Trade-off: More rewards vs. Less time to prepare
- Simple decision: explore or stay on path

---

## Resource Management & Economy

**Core Concept:** Simple resource gathering and spending for upgrades.

**Resource Types:**
- **Bones** — From defeated enemies
- **Wood** — From environment
- **Ore** — From environment

**Simple Economy:**
- Gather resources during day phase
- Spend resources on upgrades between waves
- Resources are spent, not stored with weight limits
- Focus on meaningful upgrade choices, not inventory management

**Resource Sources:**
- Gathering from environment (wood, ore)
- Defeating enemies (bones)
- Exploration rewards

**Key Tension:** Spend resources now for immediate power, or save for better upgrades later?

---

# Wave System

**Deterministic Waves:**
- Each day/night cycle is a numbered wave (Wave 1, Wave 2, Wave 3, etc.)
- Within each biome/level, waves are consistent across playthroughs
- Wave 1 will always have the same enemy composition, Wave 2 will always have the same, etc.
- This allows players to learn and strategize for specific waves

**Enemy Escalation:**
- Early waves: Light skirmishers
- Mid-game waves: Siege units, flying enemies, armor types
- Late-game waves: Elite commanders, mutated swarms, environmental hazards

**Benefits of Deterministic Waves:**
- Players can learn and master specific wave patterns
- Strategic planning becomes more meaningful
- Replayability comes from different builds/strategies rather than random enemy spawns
- Skill expression through wave knowledge and preparation

Bosses introduce biome-specific mechanics.

---

# Biomes

Each biome represents one "path" and culminates in a boss.

Examples include:
- Ashen Desert
- Red Sand Expanse
- Neon Dark Forest
- Frozen Highlands
- Dead Kingdom

Each biome includes:
- Unique enemy faction
- Night modifiers
- Environmental hazards
- Final boss encounter

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
- Day/Night Cycle with continuous movement (caravan never stops)
- Deterministic wave system (consistent waves per biome for learnability)
- Real-time auto-battler combat (units fight automatically)
- Basic caravan upgrades (simple upgrade system, not complex reconfiguration)
- Core enemy types that escalate through waves
- Simple resource management (gather and spend, no weight system)

**Content:**
- Single biome with multiple waves
- Basic progression (upgrades, unit unlocks, caravan improvements)

**Not in MVP:**
- Multiplayer functionality
- Boss encounters
- Multiple biomes
- Complex caravan reconfiguration
- Weight/capacity systems
- Tool-based role system
- Deep meta progression

**Focus:** Validate the core tension loop of defending a moving base with auto-battler combat through deterministic, learnable waves.

---

# What Makes This Game Unique

- Most tower defense games use static bases.
- Most roguelikes lack base-building.
- Most auto-battlers lack strategic movement and positioning.
- This game merges all three.

A moving base with auto-battler combat.

Strategic movement + upgrade choices + resource management.

Defending a moving target where combat is automatic but strategic decisions matter.

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
You manage resources.  
You face the boss.  
You move on — or you fall.