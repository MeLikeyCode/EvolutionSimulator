Input handling

-os
    -scene tree
        - first viewport <=====
            - generic _input() callback of all nodes called
            - gui_input() of controls called
            - unhandled_input() of all nodes called
            - camera does ray and calls _input of physics object
            - next viewport gets event

- poll:
    - can do Input.is_key_pressed() or is_action_pressed if you have it mapped (for mouse/key things)
- push:
    - can use one of the above mentioned callbacks

# Generating "action" events from code
var ev = new InputEventAction();
ev.SetAction("move_left");
ev.SetPressed(true);
Input.ParseInputEvent(ev); // give it to event handling logic