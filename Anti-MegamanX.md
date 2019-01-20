
**DEV**
global instance, hooks, vars
scene loader
timers
limit
controlled spawning
# spawn
- generate new level
- deserialized
- spawn point
- create new character (without spawn point)
- construction
- compound object replace (new level or construction)
- dragndrop in editor
## spawn variants
- limited
- by name, replacement check
- remove colliding objects
* fix the name



- no land anim while moveLeft etc
- feet no collide when running down slope
- charge sounds


**DESIGN**
# keep
no momentum to movement
dash feels good
wall jump
wall slide
hold button to charge weapon
---
destroying enemies is satisfying
safety timer after taking damage


## aesthetics
1. the sprite
- idle
- run cycle
- jump, fall
- wall slide
- dash
- weapon charge
- take damage
- low health
- ...

2. sounds
Consistent with instruments in the music.

3. music
energy. melody.


In my opinion, Megaman X1-X3 suffers from the same problems: small viewing area, can only shoot forward, enemies respawn immediately out of sight, levels are static.
Megaman X4 is so bad I never played another MMX game again. I've watched gameplay videos to keep up to date with the MMX games, but it seems the franchise never improved.

1. Camera view range.
Camera can center on an imaginary point around the player, in the aiming direction. Allow for variance in zoom, depending on how cozy the surroundings (raycast in a few directions to determine).
2. Full range of aim.
MegamanX can only shoot forwards, not upwards or even slightly higher. The ability to shoot upwards in games like Contra is an improvement. Having weapons that spread out when shot can make this limitation feel less awkward.
3. Enemies respawn at logical locations.
Not arbitrary static points.

* Spelunky-style random cell generation for city
* persistence?

**random**
skydrop intro
land of bystander- squish! => the story begins. The person who died [was important| ]
dancing randoms
screw loose; wire crossed; not all there; one X short of a Y;
thoughts come and go, recirculating weather in a fishbowl; grab an idea out of the air;
