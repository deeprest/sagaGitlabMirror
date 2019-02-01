+ raycast from center to arm pos to avoid shooting through doors while adjacent
+ camera shake
+ smoke: dash, wall slide


- player take damage
temp no input
face direction
inertia up and away
after damage anim, non-damage blinking duration
if charged, fire charged weapon

- arm normals, pulse while charging
- health on HUD
- death, respawn


* Boss
- borrow camera dolly/ramp from Flatlander, find ideal camera distance(s)
* upgrade to new input system
- atmosphere: rain/dust flying around, blinking lights

**DESIGN**
# keep
special case inertia
dash feels good
wall jump
wall slide
charge weapon
bosses
collect weapons from defeated bosses
---
destroying enemies is satisfying
safety timer after taking damage

# do not keep
only shoot forward
teleportation

# add
aiming
camera view size
camera movement follows aiming?
projectiles collide with everything
lighting, normal mapped sprites
weapons like Raiden

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

1. atmosphere

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


**random**
skydrop intro
land of bystander- squish! => the story begins. The person who died [was important|has vengeful friends]
dancing randoms
thoughts come and go, recirculating weather in a fishbowl; grab an idea out of the air;
