using Godot;
using System;

public class World : Spatial
{
    public float height = 100;
    public float width = 100;

    PackedScene creatureGenerator_;
    PackedScene foodGenerator_;
    Timer foodTimer_;

    // mouse pan
    Vector2 cameraDragClickPos_; // the screen position that right mouse button was clicked
    Vector3 cameraDragCamPos_; // the camera's position when right mouse button was clicked

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Engine.TimeScale = 5;

        creatureGenerator_ = (PackedScene)GD.Load("res://Scenes/Creature.tscn");

        // create some initial food
        foodGenerator_ = (PackedScene)GD.Load("res://Scenes/Food.tscn");
        for (int i = 0; i < 20; i++)
        {
            Food food = (Food)foodGenerator_.Instance();
            this.AddChild(food);
            float randomX = (float)GD.RandRange(-this.width/2.0,this.width/2.0);
            float randomZ = (float)GD.RandRange(-this.height/2.0,this.height/2.0);
            food.Translation = new Vector3(randomX,0,randomZ);
        }

        // periodically spawn food
        foodTimer_ = new Timer();
        this.AddChild(foodTimer_);
        foodTimer_.WaitTime = 1;
        foodTimer_.Connect("timeout",this,"on_spawn_food");
        foodTimer_.Start();

        // connect gui
        GUI gui = (GUI)this.GetNode("GUI");
        gui.Connect("CreateCreatures",this,"OnCreateCreatures");

        gui.Connect("UpdateFoodSpawnRate",this,"OnUpdateFoodSpawnRate");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public override void _UnhandledInput(InputEvent @event){
        // unhandled mouse click
        if (@event is InputEventMouseButton mousePressed){
            // left button
            if (mousePressed.ButtonIndex == (int)ButtonList.Right){
                cameraDragClickPos_ = mousePressed.Position;
                cameraDragCamPos_ = GetViewport().GetCamera().Translation;
                GetTree().SetInputAsHandled();
                return;
            }
            // wheel
            if (mousePressed.ButtonIndex == (int)ButtonList.WheelDown){
                GetViewport().GetCamera().Translation += new Vector3(0,2,0);
                GetTree().SetInputAsHandled();
                return;
            }
            else if(mousePressed.ButtonIndex == (int)ButtonList.WheelUp){
                GetViewport().GetCamera().Translation += new Vector3(0,-2,0);
                GetTree().SetInputAsHandled();
                return;
            }
        }

        // unhandled mouse move
        if (@event is InputEventMouseMotion mouseMoved){
            // while right button is down
            if ((mouseMoved.ButtonMask & (int)ButtonList.MaskRight) == (int)ButtonList.MaskRight ){
                Camera camera = GetViewport().GetCamera();
                
                Vector2 currentMousePos = mouseMoved.Position;
                Vector2 shiftVector = currentMousePos - cameraDragClickPos_;
                shiftVector /= 10;
                shiftVector *= camera.Translation.y / 35;
                shiftVector *= -1;

                camera.Translation = cameraDragCamPos_ + new Vector3(shiftVector.x,0,shiftVector.y);

                GetTree().SetInputAsHandled();
                return;
            }
        }
    }

    // executed when the create creatures button is clicked.
    // will create the creatures specified.
    void OnCreateCreatures(float mass, float radius, int number){
        for (int i = 0; i < number; i++)
        {
            Spatial creatureRoot = (Spatial)creatureGenerator_.Instance();
            Creature creature = (Creature)creatureRoot.GetNode("Area");
            creatureRoot.RemoveChild(creature);
            creatureRoot.QueueFree();
            creature.world = this;
            this.AddChild(creature);

            creature.SetProperties(mass,radius);
        }
    }

    void on_spawn_food(){
        Food food = (Food)foodGenerator_.Instance();
        this.AddChild(food);

        float randomX = (float)GD.RandRange(-this.width/2.0,this.width/2.0);
        float randomZ = (float)GD.RandRange(-this.height/2.0,this.height/2.0);
        food.Translation = new Vector3(randomX,0,randomZ);
    }

    void OnUpdateFoodSpawnRate(float rate){
        this.foodTimer_.WaitTime = rate;
    }
}