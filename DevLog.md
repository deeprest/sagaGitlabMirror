
# opinion
Megaman X1-X3 suffers from the same problems: small viewing area, can only shoot forward, enemies respawn immediately out of sight, levels are static.
Megaman X4 is so bad that I never played another MMX game again. I've watched gameplay videos to keep up to date with the MMX games, but it seems the series never recovered.

Chose to use no 2d physics (except for bouncygrenade). just want collision detection. Used boxcast for directional response.
sprite animations: using state machine. multi-part sprite characters.
normal maps. cannot use sprite atlas. lighting
deployment

# Enemies
- navmesh - sidestep
- wheelbot
- liftbot
- airbot
- hornet
  - forward/back tilt
  - guns
  - drop enemies (wheels instead of walkers)
  - death sequence
- Mech (inprogress)
- chopdrop

# Abilities
- grappling hook
- shield
- slowmo

# Weapons
- xbuster - charge
* multi-shot, variants
- bouncy grenade

# Log
- new input system. gamepad, linux. Keys: O, P
- customutility build buttons: macos and linux
- webgl issues: intial video mode is borked; mousewheel does not work;
- bug: menu elements navigate to diagetic in world
- gamepad shoot hold does not charge
- menu navigation
- cursor sensitivity setting
- confirm screen settings dialog nav issue

- turrets only aim when target is visible
- bottom of wall jump issue
- add dash anim effect
- remove pathfinding from lift, just use points-radius
- minor shield single frame flip issue
- control rebinding
- optimize performance pass
- reticle easier to see
- lower volume when paused

- lift shake issue
- lift push x

- twinstick aiming. no cursor look
minimum cursorDelta; cursor off and no shot when under min.
- directional aiming scheme when using gamepad
cursor sensistivity lower with mouse, higher with gamepad (if non-directional)
directional cursor distance = 3
left thumbstick = move

- dialogue box
- home scene: lifts pos. when enter chamber, longer run right.
- shield turret settings adjustment. lower shields. shoot angle range. on ceiling.
- fix diagetic text in front
- respawn using cyclic index
- work on marching square nodes
