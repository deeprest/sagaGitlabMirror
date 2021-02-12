
# Remember
* script execution order is the reason liftbots are updated first in the entites list, because they are added to the list on Awake().
* FIXED Linux: glitchy/wrong sprite shader for entities. This is because of texture compression settings on each individual texture.
* composite sprite render layers. Create a layer for each composite, or how to keep entire composites from interlacing with one another? 
	SOLUTION: Animate a *struct* int for each piece, and update the sortingOrder of the SpriteRenderers from LateUpdate().
* pixel-perfect animations must be done in Unity for each piece. Snap to pixel density, use constant anim curves.
* 2d skeleton rigs, avatars, and IK can not be used with composites. Skeletons use rotations, and these anims are not rotation-based.


# Decide
walljumping while auto-wallsliding


# New Things
+ NEW crush damage
NEW START A DEVLOG ON ITCH.
NEW airbot propellar ability. propellar on back? dash in air to use prop
NEW health pickups dropped by enemies
NEW Big wrecking ball bot.
NEW Agile boss bot.
NEW player death effect/anim
NEW music selection on pause menu
NEW cheats menu
NEW shield on back? press down to use shield?
NEW stickybomb exploding interaction with shield:
  1) do raycast to each IDamage within range
  2) if raycast hits something else, do a projection of the collider onto a perpendicular vector of the normal
  3) use the projected endpoints to do two more raycasts
  4) if the raycast hits an IDamage collider, proceed
NEW hide arm while not shooting
NEW align shot with surface when aiming into floor/wall

CODE remove global references to CurrentPlayer where possible
CODE change cursorOuter based on control device
CODE Character agentType: remove need for Global.instance.AgentType lookup. Just use ints.


# Issues
VERIFY confirm that Teams are set for all relevent entities.
VERIFY when entities land, want a crisp landing like playerbiped. find out how they're different. 

### Needs Fix
FIX gamepad: when not aiming, default aim direction.
FIX gamepad: graphook-trigger behaviour
FIX wheelbot jitter
FIX airbot attack, use attack speed until at last known target position
FIX grap when using gamepad (double)
FIX grap going through boxes
FIX stickybomb going through vents
FIX slowmo: pushTimer
FIX airbot pickup fall through vent covers?
FIX stickybomb stick to each other in midair
FIX mech punch should break boxes
FIX trigger release shoots graphook

### Needs Improvement
- Menu: design a better menu
- Input: better display names

#### Janky
- landing snaps. possible anim frame adjustment will fix
- adjust jump anim frame. it pops at the jump arc apex
- VERIFY jumping and sliding up and over a ledge only **sometimes** snaps to ground
mech scaling when flip... looks bad. no flipping.

#### Easy / Cosmetic
- city camera bounds
- DrCain minimum speech threshold
- light reflects too much on background layer 1 with buster
- door sprite layer should be behind characters
- control name text for right stick says "RS"
- door should indicate when it refuses to open because of team affiliation.

#### Hornet
FIX hornet gets stuck
- hornet friendly fire
- hornet avoid friendlies
- hornet proximity raycast
- hornet particles should not rotate, and should disappear

#### Minimap



### Linux Issues


## Unity Bugs
- UNITY bug: [MACOS] cursor movement in last direction when shoot
- UNITY bug: [WEBGL] webgl audio loop
- ENGINE/OS abort signal on exit leads to crash
