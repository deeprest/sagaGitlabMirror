# New Things
remove trigger layer, add OnTriggerEnter, collider->trigger = true
removed layers: flameprojectile, enemy

- NEW propellar on back? dash in air to use prop
- NEW shield on back? press down to use shield?
- NEW playercontroller record/input, moveup/movedown
- NEW stickybomb exploding interaction with shield:
  1) do raycast to each IDamage within range
  2) if raycast hits something else, do a projection of the collider onto a perpendicular vector of the normal
  3) use the projected endpoints to do two more raycasts
  4) if the raycast hits an IDamage collider, proceed
- hide arm while not shooting
- align shot with surface when aiming into floor/wall
+ draw an aiming line
- Use abilities gained from defeating enemies in the stage to defeat the boss. Can only use one ability per each MOUNT point. Abilities persist as long as they are installed. Replacing an ability drops it on the ground.
- Weapons are acquired from bosses. You can switch to them anytime.
- Bosses.  Big wrecking ball robot. Agile melee ninja.
- confirm that Teams are set for all characters.
- remove global references to CurrentPlayer where possible
- cursorOuter is different based on control device
- Character agentType: remove need for Global.instance.AgentType lookup. Just use ints.
- trigger release shoots graphook
- mech punch should break boxes

## Remember
* script execution order is the reason liftbots are updated first in the entites list, because they are added the list on Awake().


# Issues

### Needs Fix
- wheelbot jitter
- airbot attack, use attack speed until at last known target position
- fix grap when using gamepad (double)
- fix grap going through boxes
- fix stickybomb going through vents

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
