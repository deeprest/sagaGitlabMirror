pP

# DONE

## REVIEW
+ camera zones
+ removed cursor min snap
+ parallax optimization in editor

# FEATURES
boss. 'stage intro'; boss intro; chamber fight; death sequence; get weapon/post-death;
generated city.
abilities: graphook, shield, bullet-time
weapons: blaster, bouncygrenade, stickybomb, flame, laser

## ideas
- death -> You emerge from corpse as a small spiderbot that can gain control of other machines.
- enemy creature that collects parts and builds onto itself
- pickup temporary weapons from defeated foes. (undermines weapon mastery, but appropriate for one-off weapons)
- add airbot prop ability
- claw: ceiling claw climbing. dash with claw slash
- idea: generate an area by connecting random nodes that represent the player's movement- run(duration increments), jump parabola, dash-jump, wall-jump, wall-slide, graphook. AI ability to navigate/traverse environment to follow player/attack. First pathfind, then plot out movement using "movement-nodes".
- city subway. fast travel or GTA2 style train
- idea: z-doors/manifold-doors to other rooms that exist in the same space. keep all objects live, allow enemies to enter doors.

## improvements
- hide arm while not shooting
- minimap. Needs shader/coloring. Should follow player position, not camera
- weapon ideas: +bouncygrenade, +multishot, rockets, seeking missiles, +beam/laser, +stickybomb/magnet, invisible/cloaking
- temporary powerups: speedboost, slowtime, reflective shield, supercharge
- permanent mods: run speed, wall slide speed, dash duration, rapid shot, extra projectiles
- FSM: enemies should react to projectiles in proximity
- 4 color buttons: change weapon, menu, interact, use item/powerup
- bindings: read/write, layouts
- align shot with surface when aiming into floor/wall
- horizontal door trigger / downward boxcast for triggers
- use line renderer for laser. simplifies shield reflection code.
- detect cmd-f fullscreen switch and update setting
- menu navigation needs redesign.
- sprite body turn when aiming behind
- cursor influence: under min, and walljump need exceptions
- consolidate all sprites into bigsheet
+ replace old sprite anims with unity animators
- add spark effect to wall jump
- fix long slide clipping
- improve loading screen
- avoid becoming stuck in vertical pinch points: new bools for low-side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.

# ISSUES
- hornet friendly fire
- gamepad double select diagetic menu
- fix grap when using gamepad (double)
- fix grap going through boxes
- fix stickybomb going through vents
- release build screen fade in on level transition gets stuck
- reduce audiooneshot calls to avoid dropout
- wheelbot box collider hits corners
- reproducible: charge particles on after transition. reset player state on level transition
- UNITY bug: cursor movement in last direction when shoot
- UNITY bug: webgl audio loop

### Linux Issues
- support Ctrl-Q to quit
- no per-pixel lights (check OpenGL/Vulcan support)
- control names are not converted in diagetics (but okay in menu)
- scroll wheel sensitivity is too low
- mouse sensitivity is too high even on lowest setting (0.05)
- glitchy/wrong sprite shader for mech on linux
