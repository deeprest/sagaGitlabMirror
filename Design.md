# DESIGN

## Design Goals

### accessibly
- Linux, MacOS, WebGL if possible
- Must run smoothly on lower-spec machines [define min spec]
- Single player. No internet connection required to play.
- Control Rebinding

### is not commercialized
- no ads. You pay with your soul. (fake ads?)
- no microtransactions.
- no in-game currency to compulsively accumulate. (or make it useless)

### does not use addiction mechanics
- no achievements to leave you feeling underachieving.
- no trophies

### not too complex. you have a life.
- no story to ignore or lore to forget (dialogue but no narrative)
- no complicated technology/skill tree

## Design Advice
Avoid: Starting Too BIG.
Avoid: Idea commitment.
Avoid: Story up front.
Avoid: Overly rigid design.
Avoid: Arbitrarily adding things.
Must: Onboard player with forced-learning mechanics.
Must: Polish lots.

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
overcoming the challenge is the source of enjoyment; not too hard or too easy.


## player perspective (what does the player need to remember)
1. primary weapon. xbuster, grenade, flame
1. primary weapon can charge. charged blast
1. secondary weapon. graphook, shield, slowmo
1. ? Weapon limit. infinite, multiple, single(Contra).
1. ? maybe the current pickup modifies current weapon?
1. ? Abilities: persistent, temporary, limited. Ex: persistent shields, temporary powerup, limited ammo.

## Keep from MMX games
special case inertia
dash feels good
wall jump
wall slide
charge weapon
collect weapons from defeated bosses
safety timer after taking damage
speed upgrades
bosses

## changes
1. Full range of aim VS only shooting forward.
MegamanX can only shoot directly forward. The ability to shoot upwards in games like Contra is an improvement. Having weapons that spread out when shot can make this limitation feel less awkward.
1. Camera Size and Movement.
  * Camera can center on an imaginary point around the player, in the aiming direction. (optional? get feedback)
  * Allow for variance in zoom, depending on how cozy the surroundings.
1. projectiles collide with walls
1. Enemies respawn at logical locations. Not arbitrary static points.
1. no instant-death spikes or bottomless pits
1. teleportation -> airdrop from chopper
1. arbitrary ground and wall angles
1. Avoid button-mashing with hold-to-fire (auto)

## Additions
lighting, normal mapped sprites, emissive
AI, navigation mesh
level generation

## aesthetics
1. Sounds are complimentary with instruments in the music.
1. Music has energy. melody. Stages take roughly 3x music loops to avoid letting the music get stale.
1. Camera. Screen shake. Camera smooth lerp.
1. Atmosphere. Environment animations.


# Influences
- dash, walljump, charged shot. Mega Man X (1993)
- cursor aiming any direction while running. Abuse (1990?)
- open map / explore. Super Metroid (1994)
- wall cling from Ninja Gaiden?; Contra? Metal Slug? Turrican?

run
jump (with air control)
aim
duck
dash
walljump
wallslide

## Mega Man X Breakdown
demonstrate enemy abilities first in a safe, controlled environment
Intro level teaches the basics:
* run and jump. basic movement. implicit goal of progression
* take damage. vulnerability
* shoot. a way to overcome vulnerability
* kill enemies. defeat obstacles to progress
* death cycle
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
- emissive lights on Samus



# Ideas for new stuff
Main Menu is an in-game menu. player can shoot the options.
start as spiderbot (maybe nothing pays you any attention when you are a spiderbot)
find a host body


boss battles. 'stage intro'; boss intro; chamber fight; death sequence; get weapon/post-death;
explore generated city.
abilities: graphook, shield, bullet-time, "jump good"(samurai jack), fly (propeller or jetpack), pickup objects and shoot them (grav gun)
weapons: blaster, bouncygrenade, stickybomb, flame, laser

- death -> spiderbot
- spiderbot that can gain control of other machines.
- creature that collects parts and builds onto itself
- pick up parts from dead enemies
- claw: wall cling; wall climb; ceiling claw climbing; dash with claw slash
- idea: generate an area by connecting random nodes that represent the player's movement- run(duration increments), jump parabola, dash-jump, wall-jump, wall-slide, graphook. AI ability to navigate/traverse environment to follow player/attack. First pathfind, then plot out movement using "movement-nodes".
- city subway. fast travel or GTA2 style train
- idea: z-doors/manifold-doors to other rooms that exist in the same space. keep all objects live, allow enemies to enter doors.


# names
Tom John Pettycash
Jon Bon Don Juan
Carl Michael McCarmichael
