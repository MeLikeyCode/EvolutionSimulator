using Godot;
using System;

public class Test : RigidBody
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
            this.AddCentralForce(new Vector3(0, 0, -100));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        // uint randInt = GD.Randi() % 4; // 0 - 3 (both ends inclusive)
        // GD.Print(randInt);

        // if (randInt == 0)
        // {
        //     this.AddCentralForce(new Vector3(0, 0, -1));
        // }
        // else if (randInt == 1)
        // {
        //     this.AddCentralForce(new Vector3(0, 0, 1));
        // }
        // else if (randInt == 2)
        // {
        //     this.AddCentralForce(new Vector3(-1, 0, 0));
        // }
        // else
        // {
        //     this.AddCentralForce(new Vector3(1, 0, 0));
        // }

        GD.Print(this.LinearVelocity);

    }
}
