**Establish the design before resuming work**

# DESIGN pillars (from gdu.io)
1. Mantra:  "You are a robotic ninja, fighting large robots and pirating their parts."
1. Design pillars, describing *feel* (three phrases):  Nimbly pimbly. Upgrades! Irreverent Distractions.
2. Summary. Destroy bad robots and use their parts to improve yourself to save the city!
3. Features: aiming, /*modify oneself with exchangeable parts,*/ generated city
4. Target platform: MacOS, Linux, etc
5. Target Audience: Any fans of MMX that want to see evolved gameplay.
6. Game Details (see below)
7. Interface, Controls: m+k, gamepads
8. Artstyle: cyberpunk pixelart hq3x. Composite sprites with lighting, normal+emissive maps.
9. Sound / Music: rock soundtrack + instrument foley
10. Development plan.....


# Project Goals

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

# Influences
- dash, walljump, charged shot. Mega Man X (1993)
- open map / explore. Super Metroid (1994)
- cursor aiming any direction while running. Abuse (1995?)
- wall cling from Ninja Gaiden?;
- Contra style powerups?
run
jump (with air control)
aim
duck?
dash
walljump
wallslide










# Mechanics
movement
aim / look
weapon use
no death: transform from biped to tentacle bot (two forms: biped and tentacle bot)

---





## Major changes to the Mega Man X formula
1. Player can aim in any direction instead of only shooting forward.
1. Arbitrary ground and wall angles
1. No instant-death spikes or bottomless pits.
1. Enemies respawn at visible and known locations, not invisible static points.
1. Replace teleportation with airdrop from chopper.
1. Projectiles collide with walls.
1. Avoid button-mashing with hold-to-fire. This is to save wear-and-tear on game controller triggers, and mouse buttons.
1. Camera Size and Movement.
    * Camera can center on an imaginary point around the player, in the aiming direction. (optional? get feedback)
    * Allow for variance in zoom, depending on how cozy the surroundings.
1. Hold dash instead of combo dash+jump to do better walljump. (Improved in MMX2)


## Keep from MMX games
no inertia (special case inertia, like on lifts)
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

## Additions
1. lighting, normal mapped sprites, emissive
1. AI, navigation mesh
1. level generation


# Cycles
- combat cycle. aware of enemy, visual contact, observe, attack, evade => death or success
- death cycle. transformation: ninja => spider

- stage cycle. HQ to generated level. boss, weapon.
- system upgrades: sensory (blobs on perif), targeting (projectile path, highlight enemies), weakness scan (show weak spots on large enemies)


## player perspective (what does the player need to remember)
1. primary weapon. xbuster, grenade, flame
1. primary weapon can charge. charged blast
1. secondary weapon. graphook, shield, slowmo
1. ? Weapon limit. infinite, multiple, single(Contra).
1. ? maybe the current pickup modifies current weapon?
1. ? Abilities: persistent, temporary, limited. Ex: persistent shields, temporary powerup, limited ammo.


## aesthetics
1. Sounds are complimentary with instruments in the music.
1. Music has energy. melody.
1. Music zones, possibly centered around major enemies. ( In MMX, stages take roughly 3x music loops to avoid letting the music get stale. )
1. Camera. Screen shake. Camera smooth lerp.
1. Atmosphere. Environment animations.


## Death possibilities
1. Respawn
2. Parts. reclaim parts
3. Gears. Health can be reclaimed from dropped "gears" ala Sonic. Respawn if empty.
4. Instant Replay. Go back before death and replay with a few seconds leeway.
- Instant Replay: Playback system, use for death cycle:  go back in time to last grace period, play a couple seconds, and have a countdown, then let the player resume where they were at that point. If they die within 3 seconds, rewind even further to grace period before that.
5. Tiny

## design graveyard
#### spiderbot
collect parts: legs, arms, head
biped body assembly. Get to safety to rebuild?
collecting parts: automatic by proximity. show parts on bot.

#### body parts enable abilities. recollect on death.
build weapons onto biped arm slots
find body upgrades: run speed, jump height, etc

#### pickle rick / tiny when damaged
