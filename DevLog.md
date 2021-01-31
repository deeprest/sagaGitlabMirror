

FIXED camera zone soft lock when using door on dr cain chamber
FIXED [use gen safe zone] generation does not destroy entities spawned at runtime. Should it? Showing off generation in a build is nice, but if it means implementing features never seen in the game, is it worth it?
FIXED mushy on lift. consider the velocity of the object being collided-with. [CarryObject]
NOFIX: high offset off angled floor. This is a known cosmetic problem, addressed with the "multi_sample" code in UpdateCollision(), not in use.
FIXED long slide clipping. collision should reduce velocity, not only position.

Chose to use no 2d physics (except for bouncygrenade).
+ sprite animations: using state machine.
+ multi-part sprite characters.
+ normal maps, so cannot use sprite atlas.
+ 2D lighting
+ deployment

+ new input system. gamepad, linux. Keys: O, P
+ customutility build buttons: macos and linux
+ menu elements navigate to diegetic in world
+ gamepad shoot hold does not charge
+ menu navigation
+ cursor sensitivity setting
+ confirm screen settings dialog nav issue

+ turrets only aim when target is visible
+ bottom of wall jump issue
+ add dash anim effect
+ remove pathfinding from lift, just use points-radius
+ minor shield single frame flip issue
+ control rebinding
+ optimize performance pass
+ reticle easier to see
+ lower volume when paused

+ lift shake issue
+ lift push x

+ twinstick aiming. no cursor look
+ minimum cursorDelta; cursor off and no shot when under min.
+ directional aiming scheme when using gamepad

+ dialogue box
+ shield turret settings adjustment. lower shields. shoot angle range. on ceiling.
+ fix diegetic text in front
+ respawn using cyclic index
+ marching square nodes

+ camera zones
+ removed cursor min snap
+ parallax optimization in editor
+ add spark effect to wall jump
+ replace old sprite anims with unity animators

+ cursor locking. on pause menu, diegetic, app focus change
+ pause menu settings navigation
+ scene list
+ scene trigger fails / scene load fails
+ settings UI
+ back button
+ fix diegetic selects when focus menu
+ minimum shoot vector, otherwise shots have no velocity (gamepad)
+ control binding says "bad control index" for minimap (gamepad)
+ city highway music intro is bungled
+ screen settings mechanism seems broken
