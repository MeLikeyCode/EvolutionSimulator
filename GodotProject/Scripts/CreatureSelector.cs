using Godot;

public class CreatureSelector : Node
{
    public bool On = false;

    // Emitted when a creature has been selected.
    [Signal]
    public delegate void CreatureSelected(Creature creature);

    public override void _UnhandledInput(InputEvent @event)
    {
        // if we are on
        if (this.On)
        {
            if (@event is InputEventMouseButton asMousePress)
            {
                if (asMousePress.Pressed && asMousePress.ButtonIndex == (int)ButtonList.Left)
                {
                    Creature creature =  this.GetParent<World>().GetCreatureAtScreenPos(asMousePress.Position);
                    if (creature != null){
                        this.EmitSignal(nameof(CreatureSelected),creature);
                    }

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
                    Input.SetDefaultCursorShape();

                    GetTree().SetInputAsHandled();
                    return;
                }
            }
        }
    }
}