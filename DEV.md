pP
*Establish the design before resuming work*
+ Pawns and Controllers
+ turret projectiles do not hit player
+ fix chop drop input

- spiderbot.
spiderpawn has dynamic body
liftbot kinematic
remove trigger layer, add OnTriggerEnter, collider->trigger = true
removed layers: flameprojectile, enemy
- reclaim body

- Playback system

- confirm that Teams are set for all characters.
- remove global references to CurrentPlayer where possible
- cursorOuter is different based on control device
- Character agentType: remove need for Global.instance.AgentType lookup. Just use ints.
- trigger release shoots graphook
- mech punch should break boxes

### Composite Sprite Problems
+ composite sprite render layers. Create a layer for each composite, or how to keep entire composites from interlacing with one another? SOLUTION: Animate a *struct* int for each piece, and update the sortingOrder of the SpriteRenderers from LateUpdate().

- pixel-perfect animations must be done in Unity for each piece. Snap to pixel density, use constant anim curves.
- 2d skeleton rigs, avatars, and IK can not be used with composites. Skeletons use rotations, and these anims are not rotation-based.


### Janky
- landing snaps. possible anim frame adjustment will fix
- adjust jump anim frame. it pops at the jump arc apex
- high offset off angled floor
- fix long slide clipping. collision should reduce velocity, not only position.
+ mushy on lift. consider the velocity of the object being collided-with. [CarryObject]

### Menu / UI
+ buttons accept mouse clicks
- design a better menu

### Input
- better display names
- bindings: read/write

### Straight Up Bugs
+ wheelbots are buggy
- airbot attack, use attack speed until at last known target position
- fix grap when using gamepad (double)
- fix grap going through boxes
- fix stickybomb going through vents

### Easy / Cosmetic
- city camera bounds
- DrCain minimum speech threshold
- light reflects too much on background layer 1 with buster
- door sprite layer should be behind characters
- control name text for right stick says "RS"
- door should indicate when it refuses to open because of team affiliation.

### Hornet
- hornet friendly fire
hornet avoid friendlies
hornet proximity raycast
hornet particles should not rotate, and should disappear

### Minimap
- minimap. Needs shader/coloring. Should follow player position, not camera
- minimap render static once. render only characters on second render texture

### New Stuff
- hide arm while not shooting
- align shot with surface when aiming into floor/wall
+ draw an aiming line

### Unity Bugs
- UNITY bug: [MACOS] cursor movement in last direction when shoot
- UNITY bug: [WEBGL] webgl audio loop

### Linux Issues
- mouse sensitivity is too high even on lowest setting (0.05)
- scroll wheel sensitivity is too low
- support Ctrl-Q to quit
- no per-pixel lights (check OpenGL/Vulcan support)
- control names are not converted in diagetics (but okay in menu)
+ glitchy/wrong sprite shader for mech on linux. [SOLVED. This is because of texture compression settings on each individual texture.]
