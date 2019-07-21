using Godot;
using System;

public class Main : Spatial
{
    PackedScene creatureGenerator_;
    PackedScene foodGenerator_;
    Timer foodTimer_;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // create some Creatures
        creatureGenerator_ = (PackedScene)GD.Load("res://Scenes/Creature.tscn");
        for (int i = 0; i < 15; i++)
        {
            Spatial creatureRoot = (Spatial)creatureGenerator_.Instance();
            Creature creature = (Creature)creatureRoot.GetNode("Area");
            creatureRoot.RemoveChild(creature);
            creatureRoot.QueueFree();
            this.AddChild(creature);
        }

        // create some food
        foodGenerator_ = (PackedScene)GD.Load("res://Scenes/Food.tscn");
        for (int i = 0; i < 20; i++)
        {
            Food food = (Food)foodGenerator_.Instance();
            this.AddChild(food);
            float randomX = (float)GD.RandRange(-30,30);
            float randomZ = (float)GD.RandRange(-30,30);
            food.Translation = new Vector3(randomX,0,randomZ);
        }

        foodTimer_ = new Timer();
        this.AddChild(foodTimer_);
        foodTimer_.WaitTime = 1;
        foodTimer_.Connect("timeout",this,"on_spawn_food");
        foodTimer_.Start();

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    void on_spawn_food(){
        Food food = (Food)foodGenerator_.Instance();
        this.AddChild(food);

        float randomX = (float)GD.RandRange(-30,30);
        float randomZ = (float)GD.RandRange(-30,30);
        food.Translation = new Vector3(randomX,0,randomZ);
    }
}
