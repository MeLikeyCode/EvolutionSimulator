# Problems Encountered Recently
- change script properties **after** the instantiated scene has been added as a child to the new scene
    - _ready() callback is called when (and every time) the node enters a scene tree
    - so if you set script properties on the newly instantiated scene and *then* add it as a child to a new scene, the _read() function of that node will be called, and if you are initializing variables in the _read() function, your custom set variables will be over written

# Problems Encountered Frequently