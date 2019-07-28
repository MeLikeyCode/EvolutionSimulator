using Godot;
using System;

public class Test : RigidBody
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (Input.IsKeyPressed((int)KeyList.Up)){
            this.ApplyCentralImpulse(new Vector3(0,0,-0.1f));
            // this.AddCentralForce(new Vector3(0,0,-600));
        }
        
        if (Input.IsKeyPressed((int)KeyList.Down)){
            // this.ApplyCentralImpulse(new Vector3(0,0,10));
            this.AddCentralForce(new Vector3(0,0,10));
        }
        
        if (Input.IsKeyPressed((int)KeyList.Left)){
            // this.ApplyCentralImpulse(new Vector3(0,0,-10));
            this.AddCentralForce(new Vector3(-10,0,0));
        }
        
        if (Input.IsKeyPressed((int)KeyList.Right)){
            // this.ApplyCentralImpulse(new Vector3(0,0,-10));
            this.AddCentralForce(new Vector3(10,0,0));
        }
        
    }
}
