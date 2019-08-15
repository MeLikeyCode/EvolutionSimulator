using Godot;
using System;

public class Food : Area
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("body_entered", this, "on_collide");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    void on_collide(Node physicsBody)
    {
        if (physicsBody is Creature creature)
        {
            creature.currentEnergy += 100;
            creature.ateAFood = true;
            this.QueueFree();
        }
    }
}
