+ raycast from center to arm pos to avoid shooting through doors while adjacent
+ camera shake
+ smoke: dash, wall slide
+ arm normals, pulse while charging
+ player take damage
  temp no input
  face direction
  inertia up and away
  after damage anim, non-damage blinking duration
  if charged, fire charged weapon
  emissive flash
  hide arm  

- fix arm anim positions
- health on HUD

- avoid becoming stuck in vertical pinch points: new bools for low-side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.

- borrow camera dolly/ramp from Flatlander, find ideal camera distance(s)
- camera bounds

- hornet
  forward/back tilt
  guns
  missiles
  drop enemies (wheels instead of walkers)
* death, respawn
* boss
- atmosphere: rain/dust flying around, blinking lights
* upgrade to new input system
- Menu with control options (and remap)
- 2x requested grappling hook

**DESIGN**
# keep
special case inertia
dash feels good
wall jump
wall slide
charge weapon
bosses
collect weapons from defeated bosses
destroying enemies is satisfying
safety timer after taking damage

# changes
only shoot forward -> aiming
teleportation -> airdrop from chopper
character/robot styles
camera view size changes (1.5,2.5,3.5)
camera movement follows aiming
projectiles collide with everything
lighting, normal mapped sprites, emissive
---
weapons like Raiden, Contra
grappling hook

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
