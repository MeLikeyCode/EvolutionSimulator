using Godot;
using System;

public class World : Spatial
{
    public float height = 100;
    public float width = 100;

    public PackedScene creatureGenerator;
    PackedScene foodGenerator_;
    Timer foodTimer_;

    // mouse pan
    Vector2 cameraDragClickPos_; // the screen position that right mouse button was clicked
    Vector3 cameraDragCamPos_; // the camera's position when right mouse button was clicked

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        creatureGenerator = (PackedScene)GD.Load("res://Scenes/Creature.tscn");

        // create some initial food
        foodGenerator_ = (PackedScene)GD.Load("res://Scenes/Food.tscn");
        for (int i = 0; i < 20; i++)
        {
            Food food = (Food)foodGenerator_.Instance();
            this.AddChild(food);
            float randomX = (float)GD.RandRange(0,this.width);
            float randomZ = (float)GD.RandRange(0,this.height);
            food.Translation = new Vector3(randomX,0,randomZ);
        }

        // periodically spawn food
        foodTimer_ = new Timer();
        this.AddChild(foodTimer_);
        foodTimer_.WaitTime = 1;
        foodTimer_.Connect("timeout",this,nameof(on_spawn_food));
        foodTimer_.Start();

        // connect gui
        GUI gui = (GUI)this.GetNode("GUI");
        gui.Connect(nameof(GUI.CreateCreatures),this,nameof(OnCreateCreatures));

        gui.Connect(nameof(GUI.SetFoodSpawnRate),this,nameof(OnSetFoodSpawnRate));

        gui.Connect(nameof(GUI.SetTimeScale),this,nameof(OnSetTimeScale));

        Menu menu = this.GetNode<Menu>(nameof(Menu));
        menu.Connect(nameof(Menu.QuitApp),this,nameof(OnGUIQuit));

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

        // unhandled keyboard
        if (@event is InputEventKey keyPressed){
            // unhandled escape pressed
            if (keyPressed.Pressed && keyPressed.Scancode == (int)KeyList.Escape){
                GetTree().SetInputAsHandled();
                Menu menu = this.GetNode<Menu>("Menu");
                menu.Visible = true;
                menu.FocusMode = Control.FocusModeEnum.All;
                menu.GrabFocus();
            }
        }
    }

    // executed when the create creatures button is clicked.
    // will create the creatures specified.
    void OnCreateCreatures(float mass, float radius, float movementForceMag, int number){
        for (int i = 0; i < number; i++)
        {
            Creature creature = (Creature)creatureGenerator.Instance();
            creature.world = this;
            float randX = (float)GD.RandRange(0,this.width);
            float randZ = (float)GD.RandRange(0,this.height);
            Vector3 pos = new Vector3(randX,0,randZ);
            creature.Translation = pos;
            this.AddChild(creature);
            creature.SetProperties(mass,radius,movementForceMag);
        }
    }

    void on_spawn_food(){
        Food food = (Food)foodGenerator_.Instance();
        this.AddChild(food);

        float randomX = (float)GD.RandRange(0,this.width);
        float randomZ = (float)GD.RandRange(0,this.height);
        food.Translation = new Vector3(randomX,0,randomZ);
    }

    void OnSetFoodSpawnRate(float rate){
        this.foodTimer_.WaitTime = rate;
    }

    void OnSetTimeScale(float value){
        Engine.TimeScale = value;
    }

    void OnGUIQuit(){
        this.GetTree().Quit();
    }
}
