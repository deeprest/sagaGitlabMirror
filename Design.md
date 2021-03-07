

# DESIGN pillars (from gdu.io)
1. Mantra:  "You are a robotic ninja, destroying robots and pirating their parts."
1. Design pillars, describing *feel* (three phrases):  Nimbly pimbly. Upgrade! Irreverent Distractions.
2. Summary. Destroy bad robots and use their parts to improve yourself to save the city.
3. Features: aiming, generated city
4. Target platforms: Linux, MacOS, etc
5. Target Audience: Any fans of MMX that want to see the gameplay evolve.
6. Game Details (see below)
7. Interface, Controls: m+k, gamepads
8. Artstyle: cyberpunk pixelart hq4x. Composite sprites with lighting, normal+emissive maps.
9. Sound / Music: rock soundtrack + instrument foley
10. [Development plan]


# Project Goals

**Accessibility**
Must run smoothly on lower-spec machines [define min spec]
Single player. No internet connection required to play.
Control Rebinding, m+k or gamepad

**Is not commercialized**
no ads. You pay with your soul. (fake ads?)
no microtransactions.
no in-game currency to purchase.

**Does not use addiction mechanics**
no in-game currency to compulsively accumulate. (or possibly make it useless)
no achievements to leave you feeling underachieved.
no trophies. your ego is big enough already.

**Not too complex. you have a life**
no tedious narrative to ignore (dialogue and presentation, but no narrative)
no complicated technology/skill tree

Do not have much (or any, if possible) UI crap on screen.



# Influences
* Mega Man X (1993) dash, walljump, wallside, charged shot.
* Super Metroid (1994) open map / explore.
* Abuse (1995) cursor aiming any direction while running.

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
lighting, normal mapped sprites, emissive
AI, navigation mesh
level generation


# Cycles
combat cycle. aware of enemy, visual contact, observe, attack, evade => death or success
death cycle. possibly: transformation, or respawn, or instant replay.
stage cycle. HQ to generated level. boss, weapon.




# Analysis
strafe-shooting = removes need for very precise aiming
swarming enemies with high health = requires escape or wide-radius weapons
bouncy grenade spread shot = spammy mess, hope for the best
well-projected enemy shots at long intervals, low health = higher rewards for skill

# Decide
walljumping while auto-wallsliding
NEW shield on back? press down to use shield?
gameplay layers, doors
one-way platforms


## player perspective (what does the player need to remember)
1. primary weapon. xbuster, grenade, flame
1. primary weapon can charge. charged blast
1. secondary "ability". graphook, shield, slowmo, airbot propeller
1. ? Weapon limit. infinite, multiple, single(Contra).
1. ? maybe the current ability modifies current weapon?
1. ? Abilities: persistent, temporary, limited. Ex: persistent shields, temporary powerup, limited ammo.


## aesthetics
Sounds are complimentary with instruments in the music.
Music has energy. melody.
Music zones, possibly centered around major enemies. ( In MMX, stages take roughly 3x music loops to avoid letting the music get stale. )
Camera. Screen shake. Camera smooth lerp.
Atmosphere. Environment animations.


## Death possibilities
1. Respawn
2. Parts. reclaim parts
3. Gears. Health can be reclaimed from dropped "gears" ala Sonic. Respawn if empty.
4. Instant Replay. Go back before death and replay with a few seconds leeway.
- Instant Replay: Playback system, use for death cycle:  go back in time to last grace period, play a couple seconds, and have a countdown, 
	then let the player resume where they were at that point. If they die within 3 seconds, rewind even further to grace period before that.
5. Tiny


## Boss (quickboss)
when boss has low health
has line of sight to player
distance to player
is in specific areas of chamber
if player:
	has shield up
	has recently shot
	is charging shot
	is dashing
	is running
	is jumping
	is on wall



Use abilities gained from defeating enemies in the stage to defeat the boss.
Can only use one ability per each MOUNT point. Abilities persist as long as they are installed. Replacing an ability drops it on the ground.

# design graveyard
#### spiderbot
collect parts: legs, arms, head
biped body assembly. Get to safety to rebuild?
collecting parts: automatic by proximity. show parts on bot.

#### body parts enable abilities. recollect on death.
build weapons onto biped arm slots
find body upgrades: run speed, jump height, etc

#### pickle rick / tiny when damaged



# Random
https://en.wikipedia.org/wiki/Frog_battery

Destination vs exploration type of player. 
Top of each large tower hss a “Boss”. 
The basement has humble robot workers that are sometimes helpful. 
You are kind of like a robot batman and robin hood. 

Times square
Chinatown
Industrial area
Port or docks
Highway runs through some areas
Subway goes to downtown areas


Energy for robots named LiFe. Lithium Iron.
Music changes for bosses and special rooms, but also districts
Procedural gen levels: districts.

Story like shotgun ninja
Dialogue like monkey island 
Anim background like street fighter 

tl;dr read it or do not. There are no shortcuts.

Boogie Knights
Bruce Sprucegoose 
Cous Cous McSpruceGoose
David Duchovany’s Dramatic Depth

Intro stage. 
Viles ship breaks through ground forcing x into a chamber. 

Titles:
Not a Mega Man Clone
Captain Commerce Will Sue
Ninja Sex Robot
Ninja Robot Sex City
Machine Sex Profit City
Ninja Robot Sex Profit City
Corporate Future Robot City Sex Profit
Robot Ninja Sex Battle Profit
TWERTS

# Next Project
Chainsaw Romance:
Dr cain talking about city under attack
Break through window jump on chopper, while riding shoot airbots with weapon A
Fall from chopper, slide down tower
Demonstrate weapon B while running
China town: acquire weapon
After chinatown, craziness then
Flying on jet propelled craft then
Flung into air and
Slide down a crazy long wall and
Promptly land and start tearing shit apart then
Rampage through a factory and
Watch boss intro.
Fight boss
Final strike, many explosions 
Crowd gathers like end of race, strike mmx pose. Show title.


