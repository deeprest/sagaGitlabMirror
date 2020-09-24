pP

# DESIGN pillars (from gdu.io)
1. Mantra:  "Mega Man X movement with Metroid exploration in a generated city"
or "You are a robotic ninja, fighting large robots and pirating their parts."
1. Design pillars, describing *feel* (three phrases):  Nimbly pimbly. Upgrades! Irreverent Distractions.
2. Summary. Destroy bad robots and use their parts to improve yourself to save the city!
3. Features: aiming, modify oneself with exchangeable parts, generated city
4. Target platform: MacOS, Linux, etc
5. Target Audience: Me. Any old fans of MMX.
6. Game Details (see below)
7. Interface, Controls: m+k, gamepads
8. Artstyle: cyberpunk pixelart hq3x. Composite sprites with lighting, normal+emissive maps.
9. Sound / Music: rock soundtrack + instrument foley
10. Development plan.....

#### Random Ideas
* full screen texture flash to supplement explosions. use with small screen shake.
* A little robot that gets excited and follows you around (jeans badger)
* A couple of idle idiots speculating on why the robots are now evil.
* A miniboss inspired by the **relentless** posturing that evil enemies (in movies) do, which buys the good guys enough time to defeat them. A big guy that never stops taunting and cannot be damaged, but has a single weakness that is revealed at some point. Since this game has no death, maybe he's only vulnerable to the spiderbot form.
* "we're running dangerously low on stickers!"
* "My only regret..."
  * is not eating more [snausages]
  * is upgrading my wireless data plan
  * is not buying that timeshare in Florida
  * is not robbing that waffle house


## GDC Hitchhiker's Guide to Rapid prototypes
Minimum Viable product.
### SMART goals
1. Specific: precise description of what is to be accomplished.
2. Measurable: progress is visible and tracked.
3. Attainable: what actions are needed to make it happen.
4. Relevant: meaningful and realistic.
5. Time-Bound: estimated time for completion.
### Strategy
Carefully designed plan of action. Accounts for:
* Deliverables
* Features
* Functions
* Tasks
* Deadlines
* Costs
**Effectively Scoped**
### Scope
Recognize Barriers.
Bad scope can come from your Ego.
Humility can help feel out your internal barriers:
* Attitude
* Emotions
* Understanding
* Creativity
* Skill
Humility will spot external barriers:
* Time
* Resources
* Location
* Opportunity
* Competition
"Start where you are. Use what you have. Do what you can." - Arthur Ashe
### Vision
Two parts: Game Concept and Design. Focus more on the design iteration.
Core Mechanic. Secondary Mechanics. progression. Narrative.




# project Goals
## accessibly
- Must run smoothly on lower-spec machines [define min spec]
- Single player. No internet connection required to play.
- Control Rebinding
## is not commercialized
- no ads. You pay with your soul. (fake ads?)
- no microtransactions.
- no in-game currency to compulsively accumulate. (or make it useless)
## does not use addiction mechanics
- no achievements to leave you feeling underachieved.
- no trophies. your ego is big enough already.
## not too complex. you have a life.
- no tedious narrative to ignore (dialogue and presentation, but no narrative)
- no complicated technology/skill tree

## Design Advice (from unknown youtube video)
Must: Onboard player with forced-learning mechanics.
Must: Polish lots.
Avoid: Starting Too BIG.
Avoid: Idea commitment.
Avoid: Story up front.
Avoid: Overly rigid design.
Avoid: Arbitrarily adding things.
### From GDU, design mistakes
* (experiment) prototype first instead of design-up-front
* (vision) do not follow trends
* (escapism) avoid making decisions based on what's realistic
* (ease of use) UI / UX, onboarding
* inconsistent design.. (recognizable patterns)
* not having a clear vision or mantra
* ignoring feedback
* not valuing feedback


## misc thoughts
The pivot model
Flow: challenge vs ability
tension and relief
exploration: secret areas. beacons. minimap.

efficiency: time and energy
energy investment towards an expected outcome
path of least resistance VS challenge
everything out of necessity
means-end chain

games are emotionally powerful:
lure you to play
mechanics: establish a structure / limits
give you a goal
mechanics: give you abilities to reach that goal
provide obstacles to challenge your progress
overcoming the challenge is one source of enjoyment; not too hard or too easy.

### Robot, Pirate, Ninja, Monkey
**"You are a robotic ninja (monkey on inside), fighting large robots and pirating their parts."**
1. robot
mechanical, repairable
*mech*
2. ninja
agile
*primary form*
3. pirate
charismatic / social
*stop and chat with npcs*
4. monkey
small, climber
*spiderbot*


# Influences
- dash, walljump, charged shot. Mega Man X (1993)
- open map / explore. Super Metroid (1994)
- cursor aiming any direction while running. Abuse (1995?)
- wall cling from Ninja Gaiden?;
- Contra powerups?
- Metal Slug bosses?
run
jump (with air control)
aim
duck
dash
walljump
wallslide

# Mechanics
movement
aim / look
weapon use
no death: transform from biped to tentacle bot (two forms: biped and tentacle bot)
---
collecting parts: automatic by proximity. show parts on bot.
biped body assembly. Get to safety to rebuild?
collect parts: graphook, shield, propeller.
build weapons onto biped arm slots
find secret body upgrades: run speed, jump height, etc
navigation: minimap or beacons.
encounter bosses
interact with random npcs (navigation help for bosses. tells where secrets are. in-game tips.)


## Mega Man X Breakdown
*demonstrate enemy abilities first in a safe, controlled environment*
Intro level teaches everything you need to know:
* run and jump. basic movement. implicit goal of progression
* take damage. vulnerability
* death cycle
* shoot. a way to overcome vulnerability
* kill enemies. defeat obstacles to progress
* rewards, minibosses
* enemy sweet spots
* arrange for wallslide to teach walljump out of pit
* Vile: you cannot win, saved by zero (demos charge shot and dash)
* Theme: grow stronger / self improvement

## Super Metroid
- ducking
- diagonal aiming
- zoom camera
- rotating camera
- rotating sprites
- subpixel idle anim (breathing)
- 3-frame turn around anim

## Keep from MMX games
no inertia (special case inertia)
in-air velocity control
vertical jump velocity control
dash feels good
wall jump
wall slide
charge weapon
collect weapons from defeated bosses
safety timer after taking damage
speed upgrades
bosses

## changes
1. Full range of aim VS only shooting forward. With angle snaps?
1. Camera Size and Movement.
  * Camera can center on an imaginary point around the player, in the aiming direction. (optional? get feedback)
  * Allow for variance in zoom, depending on how cozy the surroundings.
1. projectiles collide with walls
1. Enemies respawn at logical locations. Not arbitrary static points.
1. no instant-death spikes or bottomless pits
1. teleportation -> airdrop from chopper
1. arbitrary ground and wall angles
1. Avoid button-mashing with hold-to-fire (auto)
1. hold dash instead of combo dash+jump to do better walljump

## Additions
lighting, normal mapped sprites, emissive
AI, navigation mesh
level generation

# Cycles
- combat cycle. aware of enemy, visual contact, observe, attack, evade => death or success
- death cycle. transformation: ninja => spider

# Spiderbot
start as spiderbot (maybe nothing notices you when you're a spiderbot)
find a host body
- death -> spiderbot
- spiderbot that can gain control of other machines.
- creature that collects parts and builds onto itself
r2d2 brainslug
take over enemies?
find scrap to build onto oneself?
can zap doors, enemies
cannibalize small enemies to build needed body parts

- stage cycle. HQ to generated level. boss, weapon.
- system upgrades: sensory (blobs on perif), targeting (projectile path, highlight enemies), weakness scan (show weak spots on large enemies)

# Ideas
Main Menu is an in-game menu. player can shoot the options.
Home / HQ
Main Menu is first room in home, with option to load game.
Level Selection is done by interacting with objects in the home area.
mini-tutorials throughout home area
Shooting Range for demonstrating new weapons.
Danger Room for practice.


boss battles. 'stage intro'; boss intro; chamber fight; death sequence; get weapon/post-death;
explore generated city.
abilities: graphook, shield, bullet-time, "jump good"(samurai jack), fly (propeller or jetpack), pickup objects and shoot them (grav gun)
weapons: blaster, bouncygrenade, stickybomb, flame, laser

## pick up parts from dead enemies
+ buster, x3 upgraded
+ flame
+ ring of fire
+ bouncygrenade
- super bouncy ball
+ stickybomb
- super sticky bomb
+ laser
- laser field
- rockets,
- homing missiles
+ graphook
+ run speed
+ jump height
+ dash speed
+ shield
+ slow-mo device


- claw: wall cling; wall climb; ceiling claw climbing; dash with claw slash
- idea: generate an area by connecting random nodes that represent the player's movement- run(duration increments), jump parabola, dash-jump, wall-jump, wall-slide, graphook. AI ability to navigate/traverse environment to follow player/attack. First pathfind, then plot out movement using "movement-nodes".
- city subway. fast travel or GTA2 style train
- idea: z-doors/manifold-doors to other rooms that exist in the same space. keep all objects live, allow enemies to enter doors.

- weapon ideas: +bouncygrenade, +multishot, rockets, seeking missiles, +beam/laser, +stickybomb/magnet, invisible/cloaking
- temporary powerups: speedboost, dash duration, ~slowtime, ~reflective shield, supercharge, extra projectiles
- permanent mods: run speed, wall slide speed, dash duration, rapid shot
- Controllers and Pawns
- Optimize performance by deactivating sections of the scene when far away from player.
- AI: enemies should react to projectiles in proximity
---
- 4 color buttons: change weapon, menu, interact, use item/powerup
- horizontal door trigger / downward boxcast for triggers
- use line renderer for laser. simplifies shield reflection code.
- detect cmd-f fullscreen switch and update setting
- menu navigation needs redesign.
- cursor influence: walljump needs exception
- consolidate all sprites into bigsheet: boss mech
---
- sprite body turn when aiming behind. Requires sprite work.
---
+ gamepad double select diagetic menu
- release build screen fade in on level transition gets stuck
- reduce audiooneshot calls to avoid dropout
- reproducible: charge particles on after transition. reset player state on level transition
- avoid becoming stuck in vertical pinch points: new bools for low+side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.

## player perspective (what does the player need to remember)
1. primary weapon. xbuster, grenade, flame
1. primary weapon can charge. charged blast
1. secondary weapon. graphook, shield, slowmo
1. ? Weapon limit. infinite, multiple, single(Contra).
1. ? maybe the current pickup modifies current weapon?
1. ? Abilities: persistent, temporary, limited. Ex: persistent shields, temporary powerup, limited ammo.


# first anim sequence
Fade in to a dark room with lab equipment in the center.  There is a man leaning over a large capsule, and a device on a table nearby. The capsule is emitting light, but we cannot see inside.  The device on the table looks like a ball with tendrils, and is plugged in to the capsule.  The building shakes; chunks of the ceiling fall nearby.  The man looks up. "We have to get out of here". He unplugs the device and takes it under his arm and heads for the door.  Before he can reach the door the wall collapses on him. The device bounces on the floor and slides a short distance away.  It lies still for a moment before a light on the side emits a glow.

# the big picture (Apparently I'm using every science fiction trope here...)
[The man] is a scientist that worked on the team that developed the first biotic, autonomous AI systems. Originally designed for head trauma patients, neurological-interfaces were coupled with machine learning to assist in recovery. Eventually the systems moved beyond analysis and treatment and showed  neural pathway augmentation.

Before lawmakers could address ethical and legal concerns, the systems were being licensed commercially to militaries, research labs, and the wealthy.

Soldiers gained enhanced sensory perception and battlefield domination through unparalleled execution of drones. [accelerated healing, pain blockers, impulse control,

Researchers with improved cognition and eidetic memory raced to invent the singularity. The tradeoff of having supreme focus is having an ethical blindspot.
"I prefer to work with people that have a proclivity to be efficient and reliable."
"We're creating a better world. One that will fight us tooth and nail all the way to enlightenment."
"They meant well, but they lost their way. We have a plan to help them focus."

The wealthy proceeds to consolidate power throughout the governments of the world forming a global syndicate.  With a monopoly on fresh water and natural resources, the syndicate maintains control while preserving deniability. The leading research labs are now funded by the syndicate.
"The masses cannot govern themselves. It is our prerogative to give them every opportunity to succeed. "

## aesthetics
1. Sounds are complimentary with instruments in the music.
1. Music has energy. melody. Stages take roughly 3x music loops to avoid letting the music get stale.
1. Camera. Screen shake. Camera smooth lerp.
1. Atmosphere. Environment animations.
