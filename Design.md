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

## Design Mistakes to Avoid
Starting too big.
Onboarding player, ease of learning mechanics.
Idea commitment.
Overly rigid design.
Story upfront.
Polish lots.
Avoid adding things arbitrarily.

## misc thoughts
The pivot model
Flow: challenge vs ability
tension and relief: area design.
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

## Additions
lighting, normal mapped sprites, emissive
AI, navigation mesh
level generation

## aesthetics
1. atmosphere.
Environment animations.
2. sounds
Complimentary with instruments in the music.
3. music
energy. melody.
4. camera
Screen shake. Camera smooth lerp.
