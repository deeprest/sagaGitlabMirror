## review
+ highway scene: pillar top collision box is too high
+ home scene: lifts pos. when enter chamber, longer run right.
+ shield turrent settings adjustment. lower shields. shoot angle range. on ceiling.
+ fix diagetic text in front
+ respawn using cyclic index

## todo
- work on marching square nodes
- align shot with surface when aiming into floor/wall
- cursor influence: under min, and walljump need exceptions
- fix grap when using gamepad (double)
- fix grap going through boxes
- detect cmd-f fullscreen switch and update setting
- add ability icon to hud

## issues
- gamepad double select diagetic menu
- menu navigation needs to be more clear. redesign with pages.
- release build screen fade in on level transition gets stuck
- horizontal door trigger
- reduce audiooneshot calls to avoid dropout
- wheelbot box collider hits corners
- reproducible: charge particles on after transition. reset player state on level transition

### low priority
- consolidate all sprites into bigsheet
- replace old sprite anims with unity animators
- UNITY bug: cursor movement in last direction when shoot
- intro scene: no hud, no menu or biped controls
- hide arm while not shooting
- add spark effect to wall jump
- fix long slide clipping
- highway loading screen

## WebGL issues
- UNITY bug: webgl audio loop

## Linux Issues
- support Ctrl-Q to quit
- no per-pixel lights (check OpenGL/Vulcan support)
- control names are not converted in diagetics (but okay in menu)
- scroll wheel sensitivity is too low
- mouse sensitivity is too high even on lowest setting (0.05)
- glitchy/wrong sprite shader for mech on linux

## ideas
- death -> You emerge from corpse as a small spiderbot that can gain control of other machines.
- bindings: json read/write, layouts
- add airbot prop ability
- graphook foe
- pickup temporary weapons from defeated foes. (undermines weapon mastery, but appropriate for one-off weapons)
- FSM: enemies should react to projectiles in proximity
- SERIALIZEDOBJECT: (design) save/load game; player status, serializedobjects
- weapon ideas: +bouncygrenade, +multishot, rockets, seeking missiles, beam/laser, stickybomb/magnet, invisible/cloaking
- powerups: speedboost, slowtime, reflective shield, supercharge
- permanent mods: run speed, wall slide speed, dash duration, rapid shot, extra projectiles
- 4 color buttons: change weapon, menu, interact, use item/powerup
- idea: generate an area by connecting random nodes that represent the player's movement- run(duration increments), jump parabola, dash-jump, wall-jump, wall-slide, graphook,
- idea: z-doors/manifold-doors to other rooms that exist in the same space. keep all objects live, allow enemies to enter doors.
- idea: AI ability to navigate/traverse environment to follow player/attack. First pathfind, then plot out movement using "movement-nodes"
- wall slide at angle in and out. building exterior
- claw: ceiling claw climbing. dash with claw slash
- enemy creature that collects parts and builds onto itself
- city subway. fast travel or GTA2 style train?

# DESIGN
## Design Mistakes to Avoid
starting too big
onboarding player, ease of learning mechanics
idea commitment
overly rigid design
story upfront
polish lots
add things arbitrarily

## player perspective
primary = xbuster, grenade, flame
primary charge = charged blast
secondary = graphook, shield, slowmo
? maybe the current pickup modifies current weapon?
? Weapon limit. infinite, multiple, single(Contra).
? Abilities: persistent, temporary, limited. Ex: persistent shields, temporary powerup, limited ammo.
- avoid becoming stuck in vertical pinch points: new bools for low-side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.

## keep
special case inertia
dash feels good
wall jump
wall slide
charge weapon
bosses
collect weapons from defeated bosses
destroying enemies is satisfying
safety timer after taking damage

## changes
only shoot forward -> aiming
teleportation -> airdrop from chopper
character/robot styles
camera view size changes
camera movement follows aiming
projectiles collide with walls
lighting, normal mapped sprites, emissive
arbitrary ground and wall angles
no instant-death spikes or bottomless pits
AI, navigation mesh

## aesthetics
1. atmosphere.

2. sounds
Consistent with instruments in the music.
3. music
energy. melody.

1. Camera view range.
Camera can center on an imaginary point around the player, in the aiming direction. Allow for variance in zoom, depending on how cozy the surroundings.
2. Full range of aim.
MegamanX can only shoot forwards, not upwards or even slightly higher. The ability to shoot upwards in games like Contra is an improvement. Having weapons that spread out when shot can make this limitation feel less awkward.
3. Enemies respawn at logical locations.
Not arbitrary static points.


# Design Requirements
## accessibly
- Linux, MacOS, WebGL if possible
- runs smoothly on lower-spec machines
- (single player) no internet connection required to play.
- remap controls
## is not commercialized
- no ads. You pay with your soul. (fake ads?)
- no microtransactions.
- no in-game currency to compulsively accumulate. (or make it useless?)
## does not use addiction mechanics
- no achievements to leave you feeling underachieving.
- no trophies
## not too complex. you have a life.
- no story to ignore or lore to forget (dialogue but no narrative)
- no complicated technology/skill tree


# thank you
https://seansleblanc.itch.io/better-minimal-webgl-template for webgl template
https://etclundberg.itch.io/ for early feedback
jbarrios on TIGSource forums
