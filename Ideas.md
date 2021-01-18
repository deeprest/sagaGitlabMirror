# Ideas

music zones, or proximity to notable enemies

collect abilities: graphook, shield, propeller.

navigation: minimap or beacons.
encounter bosses
interact with random npcs (navigation help for bosses. tells where secrets are. in-game tips.)

Main Menu is an in-game menu. player can shoot the options.
Home / HQ
Main Menu is first room in home, with option to load game.
Level Selection is done by interacting with objects in the home area.
mini-tutorials throughout home area
Shooting Range for demonstrating new weapons.
Danger Room for practice.

boss battles. 'stage intro'; boss intro; chamber fight; death sequence; get weapon/post-death;
explore generated city.
abilities: graphook, shield, bullet-time, "jump good"(samurai jack), fly (propeller or jetpack), pickup objects and shoot them (grav gun)
weapons: blaster, bouncygrenade, stickybomb, flame, laser

## pick up parts from dead enemies
+ buster, x3 upgraded
+ flame
+ ring of fire
+ bouncygrenade
- super bouncy ball
+ stickybomb
- super sticky bomb
+ laser
- laser field
- rockets,
- homing missiles
+ graphook
+ run speed
+ jump height
+ dash speed
+ shield
+ slow-mo device


- claw: wall cling; wall climb; ceiling claw climbing; dash with claw slash
- idea: generate an area by connecting random nodes that represent the player's movement- run(duration increments), jump parabola, dash-jump, wall-jump, wall-slide, graphook. AI ability to navigate/traverse environment to follow player/attack. First pathfind, then plot out movement using "movement-nodes".
- city subway. fast travel or GTA2 style train
- idea: z-doors/manifold-doors to other rooms that exist in the same space. keep all objects live, allow enemies to enter doors.

- weapon ideas: +bouncygrenade, +multishot, rockets, seeking missiles, +beam/laser, +stickybomb/magnet, invisible/cloaking
- temporary powerups: speedboost, dash duration, ~slowtime, ~reflective shield, supercharge, extra projectiles
- permanent mods: run speed, wall slide speed, dash duration, rapid shot
- Controllers and Pawns
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
+ gamepad double select diagetic menu
- release build screen fade in on level transition gets stuck
- reduce audiooneshot calls to avoid dropout
- reproducible: charge particles on after transition. reset player state on level transition
- avoid becoming stuck in vertical pinch points: new bools for low+side contacts OR lower the upper corner on side collisions. The former preserves sliding up slanted ceilings when jumping, the latter immediately stops the jump.
