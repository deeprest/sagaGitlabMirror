
# bugs
- reproducible: charge particles on after transition. reset player state on level transition
- once: stuck in mid-air (after transition?)
- reproducible: jump up wall sprite glitch

# todo
- borrow camera dolly/ramp from Flatlander, find ideal camera distance(s); lock while in chamber.
- aim auto-snap within distance
- try aim snap to 1/8th
- gamepad controls; camera, button to shoot or autoaim

* BOSS.
- audio: intro, loop, battle
- pickup temporary weapons from defeated foes
* death, respawn

# improvements
- upgrade to new input system
- Menu with control options (and remap)
- camera bounds
- avoid becoming stuck in vertical pinch points: new bools for low-side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.
- weapon ideas: slowtime, speedboost, shields, rapidfire, doublewhammy, rockets, seeking missiles, supercharge, beam, stickybomb, invisible
- 4 color buttons: change weapon, menu, interact, use item/powerup
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
destroying enemies is satisfying
safety timer after taking damage

# changes
only shoot forward -> aiming
teleportation -> airdrop from chopper
character/robot styles
camera view size changes
camera movement follows aiming
projectiles collide with walls
lighting, normal mapped sprites, emissive


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

# Fan Games
["Cancelled/Dead Mega Man X Fangames"](https://www.youtube.com/watch?v=PB8pMBSK8AU)
["Mega Man X Elf Wars (aiming)"](https://youtu.be/xGahhqoooT0?t=109)
["Mega Man X++"](https://www.youtube.com/watch?v=twI3res-obs)
["Mega Man X: Corrupted"](http://www.megamanxcorrupted.com/)
["Apsel Haven Mega Man X fangame"](https://www.youtube.com/watch?v=CwW_cziXs4U)
["Mega Man X AD"](https://reploidsoft.blogspot.com/)
# Similar Derived Games
["20XX"](https://store.steampowered.com/app/322110/20XX/)


## random
bad design mode: super slow text you cannot skip. (super metroid: "SPACE COLON".."Y")
jack basswards
bran slapmuffin
dert mcderples


## where to release demo
gamejolt
sprites.co.uk forums
send to Stone McKnuckle
pixeljoint or tigsource (to find pixel artist for next project)
## distribution platforms
itch.io
GOG
Steam
## target platforms
1. Linux
2. MacOS
3. PS4
# announcements
tigsource forums
gamejolt
indiegamesplus
itch.io devlog
twitter
reddit
