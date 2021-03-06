
## Remember
* script execution order is the reason liftbots are updated first in the entites list, because they are added the list on Awake().

# New Things
NEW START A DEVLOG ON ITCH.
NEW music selection on pause menu
NEW health pickups dropped by enemies
NEW cheats menu
NEW damage when crushed by boxes
NEW airbot propellar ability
NEW propellar on back? dash in air to use prop
NEW shield on back? press down to use shield?
NEW stickybomb exploding interaction with shield:
  1) do raycast to each IDamage within range
  2) if raycast hits something else, do a projection of the collider onto a perpendicular vector of the normal
  3) use the projected endpoints to do two more raycasts
  4) if the raycast hits an IDamage collider, proceed
NEW hide arm while not shooting
NEW align shot with surface when aiming into floor/wall
NEW Big wrecking ball robot.
NEW Agile melee ninja.
CODE remove global references to CurrentPlayer where possible
CODE cursorOuter is different based on control device
CODE Character agentType: remove need for Global.instance.AgentType lookup. Just use ints.
VERIFY confirm that Teams are set for all characters.

# Issues

- generation does not destroy entities spawned at runtime. Should it? Showing off generation in a build is nice, but if it means implementing features never seen in the game, is it worth it?
- is background a no-show in enemy_lineup?

### Needs Fix
FIX wheelbot jitter
FIX airbot attack, use attack speed until at last known target position
FIX grap when using gamepad (double)
FIX grap going through boxes
FIX stickybomb going through vents
FIX slowmo: pushTimer
FIX airbot pickup fall through vent covers?
FIX hornet gets stuck
FIX stickybomb stick to each other in midair
FIX mech punch should break boxes
FIX trigger release shoots graphook

### Needs Improvement
- Menu: design a better menu
- Input: better display names

#### Janky
- landing snaps. possible anim frame adjustment will fix
- adjust jump anim frame. it pops at the jump arc apex
- high offset off angled floor
- FIX long slide clipping. collision should reduce velocity, not only position.
+ mushy on lift. consider the velocity of the object being collided-with. [CarryObject]
- jumping and sliding up and over a ledge only **sometimes** snaps to ground
+ FIX: spline culling. Use line renderer instead.
mech scaling when flip... looks bad. no flipping.
"danger ball and chain" needs destruction anims; explosions, and particles?


#### Composite Sprite Problems
+ composite sprite render layers. Create a layer for each composite, or how to keep entire composites from interlacing with one another? SOLUTION: Animate a *struct* int for each piece, and update the sortingOrder of the SpriteRenderers from LateUpdate().
- pixel-perfect animations must be done in Unity for each piece. Snap to pixel density, use constant anim curves.
- 2d skeleton rigs, avatars, and IK can not be used with composites. Skeletons use rotations, and these anims are not rotation-based.


#### Easy / Cosmetic
- city camera bounds
- DrCain minimum speech threshold
- light reflects too much on background layer 1 with buster
- door sprite layer should be behind characters
- control name text for right stick says "RS"
- door should indicate when it refuses to open because of team affiliation.

#### Hornet
- hornet friendly fire
- hornet avoid friendlies
- hornet proximity raycast
- hornet particles should not rotate, and should disappear

#### Minimap
- minimap. Needs shader/coloring. Should follow player position, not camera
- minimap render static once. render only characters on second render texture


### Linux Issues
+ support Ctrl-Q to quit
- mouse sensitivity is too high even on lowest setting (0.05)
- scroll wheel sensitivity is too low
+ no per-pixel lights (check OpenGL/Vulcan support) [texture compression]
+ glitchy/wrong sprite shader for mech on linux. [SOLVED. This is because of texture compression settings on each individual texture.]


## Unity Bugs
- UNITY bug: [MACOS] cursor movement in last direction when shoot
- UNITY bug: [WEBGL] webgl audio loop
- ENGINE/OS abort signal on exit leads to crash
