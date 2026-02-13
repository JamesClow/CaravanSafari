# The Last Caravan (Working Title)

## High Concept

A roguelike tower defense / RTS hybrid where your base is always moving.

You control heroes leading the caravan (solo or multiplayer).  
Each day, you move forward, gather resources, and prepare.  
When night falls, horrors descend—but the caravan keeps moving.  
At the end of each path awaits a boss.  

Survive long enough to reach it. Stay too long and risk getting overwhelmed. Move too quick and risk getting pummeled by the boss.

**The game is built on multiple layers of tension**—speed vs. preparation, power vs. mobility, exploration vs. safety, and defending a moving target while managing competing priorities.

---

# Inspirations

- Diplomacy Is Not an Option — large wave pressure and resource tension  
- Tribes of Midgard — night escalation and boss gates  
- Bad North — minimalist tactical clarity  
- Hades — direct hero control and build diversity  

---

# Core Pillars

1. **A Moving Base**
2. **Direct Hero Control**
3. **Roguelike Progression**
4. **Strategic Reconfiguration**
5. **Escalating Night Pressure**
6. **Tool-Based Role Emergence**
7. **Resource Management & Economy**

---

# Core Gameplay Loop

## Day Phase (Travel + Strategy)

- Caravan progresses along a fixed world path.
- Players can:
  - Scout off-path areas (risk/reward) 
  - Gather resources (managing weight and capacity)
  - Manage inventory and resource allocation
  - Choose upgrades and prepare for night.

---

## Night Phase (Defense While Moving)

- **The caravan continues moving**—night doesn't mean stopping.
- Each day/night cycle is a **wave**—Wave 1, Wave 2, Wave 3, etc.
- Enemy waves attack from all directions as darkness falls.
- Players:
  - Directly control heroes in real-time combat.
  - Gather surronding units to follow the player.
  - Place limited defensive structures.
  - Coordinate tasks using available tools (caravan control, unit management, resource gathering).
- Survive until dawn or reach the next waypoint.

**Key Tension:** Defend a moving target while managing multiple responsibilities.

---

## End of Path

- Arena-style boss encounter.
- Defeat boss → Progress to next biome.
- Lose → Run ends.

---

# Core Systems

## 1. Moving Base System (Primary Hook)

The caravan is modular and reconfigurable.

Each wagon represents a gameplay role:

- Archer Wagon
- Supply Wagon
- Forge Wagon
- Healer Tent
- Ballista Cart
- Infantry Cart

Formation matters:

```
Frontline Wagons
Support Wagons
Rear Supply
```

Players redesign their base every day.

---

## 2. Hero Control System

Players directly control hero units.

Possible hero archetypes:
- Melee Warlord
- Ranged Commander
- Spellcaster Monarch
- Defensive Paladin
- Beastmaster

Each hero's build influences:
- Unit synergy
- Defense style
- Upgrade options
- Tactical flexibility

Hybrid of RTS + ARPG combat.

---

## 3. Multiplayer Role System

**Core Concept:** Any hero can perform any action—roles emerge from the tools and abilities they carry, not rigid class restrictions.


**Universal Actions Available to All Heroes:**
- Explore off-path areas
- Upgrade caravan pieces
- Reorder caravan formation
- Direct combat control

**Tools Define Specialization:**

Tools unlock or enhance specific capabilities:
- Control caravan movement (speed up, slow down, stop)
- Call units back to the caravan
- Hunt enemies for resources
- Gather resources from the environment
- Place defensive structures

**Tool Examples:**
- **Caravan Horn** — Move the caravan more efficiently; call units back faster and from farther away
- **Axe** — Chop down trees for wood resources
- **Pickaxe** — Mine ore from deposits
- **Resource Detection** — Locate hidden resources and events
- **Spear** — Hunt enemies more effectively for materials (bones)


**Emergent Role Assignment:**
- Players naturally fall into roles based on what tools they've acquired
- Roles can shift dynamically as tools are swapped or shared
- Communication and coordination determine who handles what in the moment
- Solo play: one hero manages all tasks, switching tools as needed
- Multiplayer: players coordinate who picks up which tools to cover all responsibilities

**Key Tension:** Everyone can do everything, but efficiency comes from specialization through tools. Players must coordinate who grabs what tools and who handles which tasks in the chaos of moving defense.

---

## 4. Roguelike Structure

Each run includes:
- Deterministic wave compositions (Wave 1 always has the same enemies, Wave 2 always has the same enemies, etc. per biome)
- Random relics
- Different caravan upgrades
- Variable biome modifiers

Meta progression between runs:
- Unlock new wagon types
- Unlock new hero archetypes
- Expand caravan size cap
- Unlock new biomes
- Unlock new tools and abilities

---

# Tension Systems

The game creates constant pressure through three core tensions:

## 1. Speed vs. Power

**The Core Dilemma:**
- Move fast → Less time to gather/upgrade → Underprepared and underpowered → Risk getting pummeled
- Move slow → More time to gather/upgrade → More powerful → But more waves → Harder enemies → Risk getting overwhelmed
- More wagons/resources = Stronger defense + More options, but heavier caravan = Slower = More waves
- Every detour, every resource gathered, every wagon added affects speed and power

Players must find the optimal balance: powerful enough for the boss, but not so slow that late-game waves become impossible.

---

## 2. Defending a Moving Target

**The Unique Challenge:**
- Caravan never stops, even during enemy attacks
- Must defend from all directions while base is in motion
- Cannot rely on static defensive positions
- Must manage combat, units, resources, and caravan simultaneously

Creates constant pressure and prevents players from settling into comfortable defensive positions.

---

## 3. Task Overload

**Too Many Priorities:**
- Everyone can do everything, but efficiency requires tool specialization
- Must balance: combat, unit management, resource gathering, caravan control, defensive structures
- Solo: One hero juggling all tasks, constantly switching tools
- Too many things to do, not enough time to do them all perfectly

Creates chaos and forces constant prioritization under pressure.

---

**The Cumulative Effect:** These three tensions compound, creating moments where players must make difficult choices with lasting consequences. The game keeps players in a state of constant decision-making under pressure.

---

# Strategic Systems

## Risk / Reward Exploration

Venturing off the main path:
- + Rare materials
- + Special units
- + Unique relics
- – Less time to fortify
- – Increased chance of ambush
- – Less preparation for upcoming wave

---

## Caravan Weight System

More wagons:
- Stronger defense
- More utility
- Slower movement

Slower movement:
- More waves before boss
- Face more challenging late-game waves

Players balance power vs. speed.

---

## Resource Management & Economy

**Core Concept:** Managing limited carrying capacity and diverse resource types creates constant strategic decisions.

**Resource Types:**
- **Bones** — Crafting materials from defeated enemies
- **Wood** — Building and repair materials
- **Ore** — Metal for weapons and structures

**Weight & Capacity System:**
- Caravan has limited carrying capacity
- Each resource type has weight
- Players must decide what to keep and what to discard
- More resources = slower caravan movement
- Storage wagons can increase capacity but add weight

**Strategic Decisions:**
- Gather everything vs. selective collection
- Keep rare resources vs. prioritize essentials
- Expand storage (more wagons) vs. stay mobile
- Trade resources at waypoints for better items
- Discard low-value resources to make room

**Resource Sources:**
- Gathering from environment (wood, ore)
- Hunting enemies (bones)
- Exploration rewards

**Key Tension:** Every resource gathered is a trade-off between power and mobility. Players must balance hoarding for upgrades vs. staying light and fast.

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
- Basic hero control and combat
- Tool-based role system (any hero can do anything, tools define specialization)
- Essential wagon types with formation reconfiguration
- Core enemy types that escalate through waves
- Resource management & economy (limited resource types, weight/capacity system)

**Content:**
- Single biome with multiple waves
- Basic progression (upgrades, tools, caravan improvements)

**Not in MVP:**
- Multiplayer functionality
- Boss encounters
- Multiple biomes
- Full tool variety
- Deep meta progression
- Full resource type variety (start with core types)

**Focus:** Validate the core tension loop of defending a moving base while managing tool-based roles through deterministic, learnable waves.

---

# What Makes This Game Unique

- Most tower defense games use static bases.
- Most roguelikes lack base-building.
- Most multiplayer games don't blend RTS roles with direct hero control.
- This game merges all three.

A reconfigurable base that moves.

Strategic positioning + direct combat control + emergent tool-based coordination.

Defending a moving target where everyone can do everything, but efficiency comes from tool-based specialization and coordination.

---

# Steam Tags (Target Audience)

- Roguelike
- RTS
- Tower Defense
- Base Building
- Strategy
- Action
- Co-op
- Multiplayer

---

# Future Expansion Ideas

- Additional hero archetypes
- Expanded tool system (more specialized tools and abilities)
- Faction-based caravan themes
- Dynamic world map instead of fixed path
- Procedural biome generation
- Endless survival mode
- Competitive modes
- Tool-specific progression trees

---

# Summary

A roguelike tower defense game where the base never stands still.

You travel.  
You fortify.  
You survive the night—while still moving.  
You coordinate roles.  
You face the boss.  
You move on — or you fall.