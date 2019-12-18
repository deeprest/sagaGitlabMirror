
# thought
In my opinion, Megaman X1-X3 suffers from the same problems: small viewing area, can only shoot forward, enemies respawn immediately out of sight, levels are static.
Megaman X4 is bad enough that I never played another MMX game again. I've watched gameplay videos to keep up to date with the MMX games, but it seems the series has not improved.

Unity is great for static levels

Chose to use no 2d physics (except for bouncygrenade). just want collision detection. Used boxcast for directional response.
sprite animations (not in legacy animation, and not using state machine)
multi-part sprite characters, animation frames
normal maps. cannot use sprite atlas. lighting
deployment

- dialogue box

# Enemies
+ navmesh + sidestep
+ wheelbot
+ liftbot
+ airbot
+ hornet
  + forward/back tilt
  + guns
  + drop enemies (wheels instead of walkers)
  + death sequence
+ Mech (inprogress)
+ chopdrop

# Abilities
+ grappling hook
+ shield
- slowmo

# Weapons
+ xbuster + charge
* multi-shot, variants
+ bouncy grenade


+ new input system. gamepad, linux. Keys: O, P
+ customutility build buttons: macos and linux
+ webgl issues: intial video mode is borked; mousewheel does not work;
+ bug: menu elements navigate to diagetic in world
+ gamepad shoot hold does not charge
+ menu navigation
+ cursor sensitivity setting
+ confirm screen settings dialog nav issue
