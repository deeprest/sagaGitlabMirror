# done (add to devlog)


# todo
- control rebinding
- reduce audiooneshot calls to avoid dropout
- remove pathfinding from lift, just use points+radius
- fix long slide clipping
- add airbot prop ability
- hide arm while not shooting
- TURRETS only aim when target is visible
- MECH. pathing

## lower priority
- add ability icon to hud
- music scene switch
- add dash anim effect
- add spark effect to wall jump
- fix nav mesh obstacles in city scene
- consolidate more sprites into bigsheet
- detect cmd-f fullscreen switch and update setting
- fix glitchy sprite shader for grenade mech on linux
- UNITY-bug: cursor movement in last direction when shoot

## ideas
- graphook foe
- enemies should react to projectiles in proximity
- (design) save/load game; player status, serializedobjects, UI interface
- use state machine for enemies? how capable are the bots? They currently pathfind and sidestep.
- pickup temporary weapons from defeated foes. (undermines weapon mastery, but appropriate for one-off weapons)
- weapon ideas:  doublewhammy/triple, rockets, seeking missiles, beam, stickybomb, invisible
- powerups: speedboost, slowtime, reflective shield, supercharge
- permanent mods: run speed, wall slide speed, dash duration, rapid shot, extra projectiles
- 4 color buttons: change weapon, menu, interact, use item/powerup
- idea: generate an area by connecting random nodes that represent the player's movement- run(duration increments), jump parabola, dash-jump, wall-jump, wall-slide, graphook,
- idea: z-doors/manifold-doors to other rooms that exist in the same space. keep all objects live, allow enemies to enter doors.
- idea: AI ability to navigate/traverse environment to follow player/attack. First pathfind, then plot out movement using "movement-nodes"
- wall slide at angle in and out. building exterior
- ceiling claw climbing
- dash with claw slash
- serialize dead robot parts
- enemy creature that collects parts and builds onto itself
- subway. fast travel or GTA2 style train?

## bugs / issues
- release build screen fade in on level transition gets stuck
- reproducible: charge particles on after transition. reset player state on level transition


# DESIGN
## Design Mistakes to Avoid
starting too big
onboarding player, ease of learning mechanics
idea commitment
overly rigid design
story upfront
polish lots
add things arbitrarily

primary = xbuster, grenade, flame
primary charge = charged blast
secondary = graphook, shield, slowmo
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
1. animation
- idle
- run cycle
- jump, fall
- wall slide
- dash
- weapon charge
- take damage
- low health

1. atmosphere.

2. sounds
Consistent with instruments in the music.
3. music
energy. melody.

In my opinion, Megaman X1-X3 suffers from the same problems: small viewing area, can only shoot forward, enemies respawn immediately out of sight, levels are static.
Megaman X4 is bad enough that I never played another MMX game again. I've watched gameplay videos to keep up to date with the MMX games, but it seems the franchise never improved.

1. Camera view range.
Camera can center on an imaginary point around the player, in the aiming direction. Allow for variance in zoom, depending on how cozy the surroundings (raycast in a few directions to determine).
2. Full range of aim.
MegamanX can only shoot forwards, not upwards or even slightly higher. The ability to shoot upwards in games like Contra is an improvement. Having weapons that spread out when shot can make this limitation feel less awkward.
3. Enemies respawn at logical locations.
Not arbitrary static points.

# Fan Games
["Cancelled/Dead Mega Man X Fangames"](https://www.youtube.com/watch?v=PB8pMBSK8AU)
["Mega Man X Elf Wars (aiming)"](https://youtu.be/xGahhqoooT0?t=109)
["Mega Man X++"](https://www.youtube.com/watch?v=twI3res-obs)
["Mega Man X: Corrupted"](http://www.megamanxcorrupted.com/)
["Apsel Haven Mega Man X fangame"](https://www.youtube.com/watch?v=CwW_cziXs4U)
["Mega Man X AD"](https://reploidsoft.blogspot.com/)
# Similar Derived Games
["20XX"](https://store.steampowered.com/app/322110/20XX/)


# Features
## accessibly
- Linux, MacOS, Windows?
- runs smoothly on lower-spec machines
- (single player) no internet connection required to play.
- remap controls
## is not commercialized
- no ads. You pay with you soul.
- no microtransactions.
- no in-game currency to compulsively accumulate.
## does not use addiction mechanics
- no achievements to leave you feeling underachieving.
- no trophies to falsely prop up your ego.
## not too complex. you have a life.
- no complicated technology/skill tree
- no story to ignore or lore to forget


## where to release/announce demo
gamejolt
sprites.co.uk forums
send to Stone McKnuckle
pixeljoint or tigsource (to find artist for full project)
## distribution platforms
itch.io
GOG
Steam

# announcements
tigsource forums
gamejolt
indiegamesplus
itch.io devlog
twitter
reddit

# random
Can gain other weapons from enemies.
Chopdrop instead of teleportation.
death -> You emerge from corpse as a small spiderbot that can gain control of other machines.
death -> The player awakens in the lab, newly-rebuilt. The attending mechanic explains that the explosion destroyed your body but not your core. He has given you a new body but could not retrieve your weapons.

## random 'names'
jack basswards
bran slapmuffin
dert mcderples
Slappy McTicklemonster
