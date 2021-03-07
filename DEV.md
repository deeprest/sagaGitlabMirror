# Remember
* script execution order is the reason liftbots are updated first in the entites list, because they are added to the list on Awake().
* FIXED Linux: glitchy/wrong sprite shader for entities. This is because of texture compression settings on each individual texture.
* composite sprite render layers. Create a layer for each composite, or how to keep entire composites from interlacing with one another? 
	SOLUTION: Animate a *struct* int for each piece, and update the sortingOrder of the SpriteRenderers from LateUpdate().
* pixel-perfect animations must be done in Unity for each piece. Snap to pixel density, use constant anim curves.
* 2d skeleton rigs, avatars, and IK can not be used with composites. Skeletons use rotations, and these anims are not rotation-based.



# New Things

NEW player death effect/anim
NEW xbuster big charged shot

NEW cursor stops at edge of screen

NEW align shot with surface when aiming into floor/wall

NEW stickybomb exploding interaction with shield:
  1) do raycast to each IDamage within range
  2) if raycast hits something else, do a projection of the collider onto a perpendicular vector of the normal
  3) use the projected endpoints to do two more raycasts
  4) if the raycast hits an IDamage collider, proceed

NEW enemy that uses and drops the graphook
NEW graphook: show valid raycast targets

NEW music selection on pause menu
NEW cheats menu

NEW punchy mech needs anticipation anim before punch

NEW hornet turnaround sprites
NEW Hornet friendly fire
NEW Hornet avoid friendlies
NEW Hornet proximity raycast
NEW Hornet particles should not rotate, and should disappear


CODE remove global references to CurrentPlayer where possible
CODE change cursorOuter based on control device
CODE Character agentType: remove need for Global.instance.AgentType lookup. Just use ints.
CODE Teams. Use ScriptableObject for Teams instead of enum. Have colors that modify projectiles.

NEW Big wrecking ball bot.
NEW Agile boss bot.

# Issues

## Verify These Issues Exist
VERIFY particle weirdness rain / smoke in chamber
VERIFY confirm that Teams are set for all relevent entities.
VERIFY when entities land, want a crisp landing like playerbiped. find out how they're different. 
VERIFY jumping and sliding up and over a ledge only **sometimes** snaps to ground
VERIFY airbot pickup fall through vent covers?

### Needs Fix
FIX(ED?) smirk smoke particle vertex glitch
FIX cancelling graphook gives ava velocity
FIX stickybomb should not stick to breakable text
FIX music and scene list need center column alignment
FIX graphook sticks
FIX stickybomb sticks to other stickybombs in midair
FIX gamepad: when not aiming, default aim direction.
FIX gamepad: graphook-trigger behaviour
FIX(ED?) wheelbot jitter; wheelbot falls through floor sometimes
FIX grap when using gamepad (double)
FIX stickybomb going through vents
FIX slowmo, override velocity incorrect
FIX highway song intro steps on loop
FIX airbot ability when using minimap
FIX walljumpdash with airbit ability equipped
FIX fly ability rotation when only holding up key


### Needs Improvement
Menu: design a better menu
Input: better display names


#### Polish
POLISH walljump spark effect is on wrong side
POLISH landing snaps. possible anim frame adjustment will fix
POLISH adjust jump anim frame. it pops at the jump arc apex
POLISH city camera bounds
POLISH DrCain minimum speech threshold
POLISH light reflects too much on background layer 1 with buster
POLISH door sprite layer should be behind characters
POLISH control name text for right stick says "RS"
POLISH door should indicate when it refuses to open because of team affiliation.
POLISH trishot hit anim color
POLISH hide arm while not shooting
POLISH ava low health breathing anim


## Unity Bugs
- UNITY bug: [MACOS] cursor movement in last direction when shoot
- UNITY bug: [WEBGL] webgl audio loop
- ENGINE/OS abort signal on exit leads to crash
