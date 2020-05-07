


TODO
# Unity importer
1. parse layer name for token "ct" for child transform. ex: "ct-head"
2. loop through pixels in cel until an opaque pixel (or special value?) is found, make note of position
3. calculate local position based on sprite pixel density
4. modify animation editor curve position of transform when generating animation



# In Aseprite
* create a layer in Aseprite for each child transform. head, arm
* the first opaque pixel in each cel is the child transform position, the "pivot"
* pivots can be any color. Use different colors for child pivots for contrast.
* Editing pivots this way keeps the child transforms synced without edits in Unity.
* rotation not currently supported. If it was, the color value would be used for the rotation angle.
