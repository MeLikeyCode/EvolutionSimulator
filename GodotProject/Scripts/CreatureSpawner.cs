using Godot;

public class CreatureSpawner : Node
{
    public bool On = false;

    // Emitted when the CreatureSpawner wants to request a creature to be spawned somewhere.
    [Signal]
    public delegate void SpawnCreature(Vector3 pos);

    public override void _UnhandledInput(InputEvent @event)
    {
        // if we are on, emit SpawnCreature signals in response to mouse clicks
        if (this.On)
        {
            if (@event is InputEventMouseButton asMousePress)
            {
                if (asMousePress.Pressed && asMousePress.ButtonIndex == (int)ButtonList.Left)
                {
                    Vector3 worldPos = this.GetParent<World>().ScreenPosToWorldPos(asMousePress.Position);
                    this.EmitSignal(nameof(SpawnCreature),worldPos);

                    GetTree().SetInputAsHandled();
                    return;
                }
            }

            // if we receive an escape key, turn off
            if (@event is InputEventKey asKeyEvent)
            {
                if (asKeyEvent.Pressed && asKeyEvent.Scancode == (int)KeyList.Escape)
                {
                    this.On = false;
                    Input.SetCustomMouseCursor(null);

                    GetTree().SetInputAsHandled();
                    return;
                }
            }
        }
    }
}