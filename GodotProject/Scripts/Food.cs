using Godot;
using System;

public class Food : Area
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("area_entered",this,"on_collide_with_creature");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    void on_collide_with_creature(Area area){
        Creature creature = (Creature)area;
        creature.current_energy += 100;
        this.QueueFree();
    }
}
