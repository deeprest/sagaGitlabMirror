pP
*Establish the design before resuming work*

### Before Next Release

## todo


### Menu / UI
+ buttons accept mouse clicks
- design a better menu, maybe

### Input
- better display names
- bindings: read/write

### Straight Up Bugs
- wheelbots are buggy
- city camera bounds
- airbot attack, use attack speed until at last known target position
- fix grap when using gamepad (double)
- fix grap going through boxes
- fix stickybomb going through vents

### Minor / Cosmetic
- DrCain minimum speech threshold
- light reflects too much on background layer 1 with buster
- door sprite layer should be behind characters
- control name text for right stick says "RS"

### Janky
- landing snaps. possible anim frame adjustment will fix
- adjust jump anim frame. it pops at the jump arc apex
- high offset off angled floor
- fix long slide clipping. collision should reduce velocity, not simply change position.
- mushy on lift. consider the velocity of the object being collided-with.

### Hornet
hornet avoid friendlies
hornet proximity raycast
hornet particles should not rotate, and should disappear

### Minimap
- minimap. Needs shader/coloring. Should follow player position, not camera
- minimap render static once. render only characters on second render texture


### New Stuff
- hide arm while not shooting
- align shot with surface when aiming into floor/wall
- draw an aiming line
- weapon ideas: +bouncygrenade, +multishot, rockets, seeking missiles, +beam/laser, +stickybomb/magnet, invisible/cloaking
- temporary powerups: speedboost, dash duration, ~slowtime, ~reflective shield, supercharge, extra projectiles
- permanent mods: run speed, wall slide speed, dash duration, rapid shot
- Controllers and Pawns
- CharacterUpdate. Pause.
- Optimize performance by deactivating sections of the scene when far away from player.
- AI: enemies should react to projectiles in proximity
---
- 4 color buttons: change weapon, menu, interact, use item/powerup
- horizontal door trigger / downward boxcast for triggers
- use line renderer for laser. simplifies shield reflection code.
- detect cmd-f fullscreen switch and update setting
- menu navigation needs redesign.
- cursor influence: walljump needs exception
- consolidate all sprites into bigsheet: boss mech
---
- sprite body turn when aiming behind. Requires sprite work.
---
- hornet friendly fire
- gamepad double select diagetic menu
- release build screen fade in on level transition gets stuck
- reduce audiooneshot calls to avoid dropout
- reproducible: charge particles on after transition. reset player state on level transition
- avoid becoming stuck in vertical pinch points: new bools for low-side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.
---
### Unity Bugs
- UNITY bug: cursor movement in last direction when shoot
- UNITY bug: webgl audio loop

### Linux Issues
- support Ctrl-Q to quit
- no per-pixel lights (check OpenGL/Vulcan support)
- control names are not converted in diagetics (but okay in menu)
- scroll wheel sensitivity is too low
- mouse sensitivity is too high even on lowest setting (0.05)
- glitchy/wrong sprite shader for mech on linux
