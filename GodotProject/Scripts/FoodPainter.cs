using Godot;
using System;

public class FoodPainter : Node
{
    public bool on = false;

    private int dragCount_ = 0;

    // Emitted when the FoodPainter wants to request a food to be spanwed somewhere.
    [Signal]
    public delegate void SpawnFood(Vector3 pos);

    public override void _UnhandledInput(InputEvent @event)
    {
        // only respond to events if we are currently in food painting mode
        if (this.on)
        {

            // make sure we create some food with just one click (no drag needed)
            if (@event is InputEventMouseButton asMousePress)
            {
                if (asMousePress.Pressed && asMousePress.ButtonIndex == (int)ButtonList.Left)
                {
                    // if left mouse is pressed
                    // TODO create initial food
                    this.dragCount_ = 0;

                    Camera currentCam = GetViewport().GetCamera();
                    Plane dropPlane = Plane.PlaneXZ;

                    for (int i = 0; i < 3; i++)
                    {
                        Vector3 worldPos = dropPlane.IntersectRay(currentCam.ProjectRayOrigin(asMousePress.Position), currentCam.ProjectRayNormal(asMousePress.Position));
                        worldPos += new Vector3((float)GD.RandRange(-10, 10), 0, (float)GD.RandRange(-10, 10));
                        this.EmitSignal(nameof(SpawnFood), worldPos);
                    }

                    GetTree().SetInputAsHandled();
                    return;
                }
            }

            // but as long as we are dragging, keep spawning food
            if (@event is InputEventMouseMotion asMouseMotion)
            {
                if ((asMouseMotion.ButtonMask & (int)ButtonList.MaskLeft) == (int)ButtonList.MaskLeft)
                {
                    this.dragCount_ += 1;

                    if (this.dragCount_ % 3 == 0)
                    { // every 10 pixels dragged, request new foods spawned
                        Camera currentCam = GetViewport().GetCamera();
                        Plane dropPlane = Plane.PlaneXZ;

                        for (int i = 0; i < 3; i++)
                        {
                            Vector3 worldPos = dropPlane.IntersectRay(currentCam.ProjectRayOrigin(asMouseMotion.Position), currentCam.ProjectRayNormal(asMouseMotion.Position));
                            worldPos += new Vector3((float)GD.RandRange(-10, 10), 0, (float)GD.RandRange(-10, 10));
                            this.EmitSignal(nameof(SpawnFood), worldPos);
                        }
                    }

                    GetTree().SetInputAsHandled();
                    return;
                }
            }

            // detect when escape is pressed, and end this mode
            if (@event is InputEventKey asKeyEvent)
            {
                if (asKeyEvent.Pressed && asKeyEvent.Scancode == (int)KeyList.Escape)
                {
                    this.on = false;
                    this.dragCount_ = 0;
                    Input.SetCustomMouseCursor(null);

                    GetTree().SetInputAsHandled();
                    return;
                }
            }
        }
    }
}
