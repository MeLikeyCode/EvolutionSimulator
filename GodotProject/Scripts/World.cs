using Godot;
using System;
using System.Collections.Generic;

public class World : Spatial
{
    public float height = 100;
    public float width = 100;

    public PackedScene creatureGenerator;
    PackedScene foodGenerator_;
    Timer foodTimer_;

    Timer statTimer_;

    // mouse pan
    Vector2 cameraDragClickPos_; // the screen position that right mouse button was clicked
    Vector3 cameraDragCamPos_; // the camera's position when right mouse button was clicked
    private int numFoodPerSpawn; // how many food to spawn each time we decide to spawn food

    public List<Creature> creatures = new List<Creature>(); // all the creatures spanwed
    public List<Food> foods = new List<Food>(); // all the foods in the world

    bool selectingCreature_;

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
            this.foods.Add(food);
            float randomX = (float)GD.RandRange(0, this.width);
            float randomZ = (float)GD.RandRange(0, this.height);
            food.Translation = new Vector3(randomX, 0, randomZ);
        }

        // create food timer (spawns food periodically)
        foodTimer_ = new Timer();
        this.AddChild(foodTimer_);
        foodTimer_.WaitTime = 1;
        foodTimer_.Connect("timeout", this, nameof(OnSpawnFood));
        foodTimer_.Start();

        // create stat timer (periodically reclaculates stats)
        statTimer_ = new Timer();
        this.AddChild(statTimer_);
        statTimer_.WaitTime = 1;
        statTimer_.Connect("timeout", this, nameof(OnCalculateStats));
        statTimer_.Start();

        // connect gui
        GUI gui = (GUI)this.GetNode("GUI");
        gui.Connect(nameof(GUI.CreateCreatures), this, nameof(OnCreateCreatures));

        gui.Connect(nameof(GUI.SetFoodSpawnRate), this, nameof(OnSetFoodSpawnRate));

        gui.Connect(nameof(GUI.SetTimeScale), this, nameof(OnSetTimeScale));

        gui.Connect(nameof(GUI.SetWorldBounds), this, nameof(OnUpdateBounds));

        gui.Connect(nameof(GUI.SetPaintFoodMode), this, nameof(OnSetFoodPaintMode));

        gui.Connect(nameof(GUI.SetCreatureCreatureMode), this, nameof(OnSetCreateCreatureMode));

        gui.Connect(nameof(GUI.SelectCreature), this, nameof(OnSelectCreaturePressed));

        Menu menu = this.GetNode<Menu>(nameof(Menu));
        menu.Connect(nameof(Menu.QuitApp), this, nameof(OnGUIQuit));

        FoodPainter foodPainter = this.GetNode<FoodPainter>("FoodPainter");
        foodPainter.Connect(nameof(FoodPainter.SpawnFood), this, nameof(OnFoodPainterSpawnFood));

        CreatureSpawner creatureSpawner = this.GetNode<CreatureSpawner>("CreatureSpawner");
        creatureSpawner.Connect(nameof(CreatureSpawner.SpawnCreature), this, nameof(OnCreateSingleCreature));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // unhandled mouse click
        if (@event is InputEventMouseButton mousePressed)
        {
            // right button
            if (mousePressed.ButtonIndex == (int)ButtonList.Right)
            {
                cameraDragClickPos_ = mousePressed.Position;
                cameraDragCamPos_ = GetViewport().GetCamera().Translation;
                GetTree().SetInputAsHandled();
                return;
            }

            // wheel
            float AMOUNT = GetViewport().GetCamera().Translation.y / 10;
            if (mousePressed.ButtonIndex == (int)ButtonList.WheelDown)
            {
                GetViewport().GetCamera().Translation += new Vector3(0, AMOUNT, 0);
                GetTree().SetInputAsHandled();
                return;
            }
            else if (mousePressed.ButtonIndex == (int)ButtonList.WheelUp)
            {
                GetViewport().GetCamera().Translation += new Vector3(0, -AMOUNT, 0);
                GetTree().SetInputAsHandled();
                return;
            }
        }

        // unhandled mouse move
        if (@event is InputEventMouseMotion mouseMoved)
        {
            // while right button is down
            if ((mouseMoved.ButtonMask & (int)ButtonList.MaskRight) == (int)ButtonList.MaskRight)
            {
                Camera camera = GetViewport().GetCamera();

                Vector2 currentMousePos = mouseMoved.Position;
                Vector2 shiftVector = currentMousePos - cameraDragClickPos_;
                shiftVector /= 10;
                shiftVector *= camera.Translation.y / 35;
                shiftVector *= -1;

                camera.Translation = cameraDragCamPos_ + new Vector3(shiftVector.x, 0, shiftVector.y);

                GetTree().SetInputAsHandled();
                return;
            }
        }

        // unhandled keyboard
        if (@event is InputEventKey keyPressed)
        {
            // unhandled escape pressed
            if (keyPressed.Pressed && keyPressed.Scancode == (int)KeyList.Escape)
            {
                if (this.selectingCreature_){
                    this.selectingCreature_ = false;
                    Input.SetDefaultCursorShape();
                    this.GetTree().SetInputAsHandled();
                    return;
                }

                GetTree().SetInputAsHandled();
                Menu menu = this.GetNode<Menu>("Menu");
                menu.Visible = true;
                menu.FocusMode = Control.FocusModeEnum.All;
                menu.GrabFocus();
            }
        }
    }

    // executed when the "select creature" button was clicked.
    void OnSelectCreaturePressed()
    {
        this.selectingCreature_ = true;
        Input.SetDefaultCursorShape(Input.CursorShape.Cross);
    }

    void OnCreatureClicked(Creature creature)
    {
        if (this.selectingCreature_)
        {
            Panel panel = this.GetNode<GUI>("GUI").GetNode<Panel>("TabContainer/Stats/Panel/Panel2");

            panel.GetNode<Label>("Label7").Text = creature.mass.ToString();
            panel.GetNode<Label>("Label8").Text = creature.radius.ToString();
            panel.GetNode<Label>("Label9").Text = creature.movementForceMag.ToString();
            panel.GetNode<Label>("Label10").Text = creature.numCreaturesEaten.ToString();
            panel.GetNode<Label>("Label11").Text = creature.numChildrenSpanwed.ToString();

            this.selectingCreature_ = false;
            Input.SetDefaultCursorShape();
        }
    }

    // executed when the create creatures button is clicked.
    // will create the creatures specified.
    void OnCreateCreatures(float mass, float radius, float movementForceMag, int number)
    {
        for (int i = 0; i < number; i++)
        {
            Creature creature = (Creature)creatureGenerator.Instance();
            float randX = (float)GD.RandRange(0, this.width);
            float randZ = (float)GD.RandRange(0, this.height);
            Vector3 pos = new Vector3(randX, 0, randZ);
            creature.Translation = pos;
            creature.InitializeCreature(mass, radius, movementForceMag);
            this.AddCreature(creature);
        }
    }

    // executed when it is time to recalculate stats
    void OnCalculateStats()
    {
        Panel panel = this.GetNode<GUI>("GUI").GetNode<Panel>("TabContainer/Stats/Panel/Panel");

        float totalMass = 0;
        float totalRadius = 0;
        float totalMovementForce = 0;

        foreach (var creature in this.creatures)
        {
            totalMass += creature.mass;
            totalRadius += creature.radius;
            totalMovementForce += creature.movementForceMag;
        }

        int numberOfCreatures = this.creatures.Count;
        float averageMass = totalMass / numberOfCreatures;
        float averageRadius = totalRadius / numberOfCreatures;
        float averageMovementForce = totalMovementForce / numberOfCreatures;

        float FOOD_EXTENTS = 0.5f;
        float individualFoodArea = Mathf.Pow(FOOD_EXTENTS * 2, 2);
        int numberOfFood = this.foods.Count;
        float foodArea = numberOfFood * individualFoodArea;
        float area = this.width * this.height;
        float foodDensity = foodArea / area;

        panel.GetNode<Label>("Label5").Text = averageMass.ToString("0.##");
        panel.GetNode<Label>("Label6").Text = averageRadius.ToString("0.##");
        panel.GetNode<Label>("Label7").Text = averageMovementForce.ToString("0.##");
        panel.GetNode<Label>("Label9").Text = (foodDensity * 100).ToString() + " %";
    }

    // executed each time we decide to spawn some food
    void OnSpawnFood()
    {
        for (int i = 0; i < this.numFoodPerSpawn; i++)
        {
            Food food = (Food)foodGenerator_.Instance();
            this.AddChild(food);
            this.foods.Add(food);

            float randomX = (float)GD.RandRange(0, this.width);
            float randomZ = (float)GD.RandRange(0, this.height);
            food.Translation = new Vector3(randomX, 0, randomZ);
        }
    }

    void OnSetFoodSpawnRate(int numToCreate, float delay)
    {
        this.foodTimer_.WaitTime = delay;
        this.numFoodPerSpawn = numToCreate;
    }

    void OnSetTimeScale(float value)
    {
        Engine.TimeScale = value;
    }

    void OnGUIQuit()
    {
        this.GetTree().Quit();
    }

    void OnUpdateBounds(float width, float height)
    {
        this.width = width;
        this.height = height;
    }

    void OnSetFoodPaintMode()
    {
        FoodPainter foodPainter = this.GetNode<FoodPainter>("FoodPainter");
        foodPainter.On = true;
        Texture image = GD.Load<Texture>("res://Art/foodCursor.png");
        Vector2 imageCenter = new Vector2(image.GetWidth() / 2, image.GetHeight() / 2);
        Input.SetCustomMouseCursor(image, Input.CursorShape.Arrow, imageCenter);
    }

    void OnSetCreateCreatureMode()
    {
        CreatureSpawner creatureSpawner = this.GetNode<CreatureSpawner>("CreatureSpawner");
        creatureSpawner.On = true;
        Texture image = GD.Load<Texture>("res://Art/createCreatureCursor.png");
        Vector2 imageCenter = new Vector2(image.GetWidth() / 2, image.GetHeight() / 2);
        Input.SetCustomMouseCursor(image, Input.CursorShape.Arrow, imageCenter);
    }

    void OnFoodPainterSpawnFood(Vector3 pos)
    {
        Food food = (Food)foodGenerator_.Instance();
        food.Translation = pos;
        this.AddChild(food);
        this.foods.Add(food);
    }

    void OnCreateSingleCreature(Vector3 pos)
    {
        Creature creature = (Creature)creatureGenerator.Instance();

        Panel createCreaturePanel = this.GetNode<GUI>("GUI").GetNode<Panel>("TabContainer/Creature/Panel/Panel2");
        float mass = createCreaturePanel.GetNode<LineEdit>("LineEdit").Text.ToFloat();
        float radius = createCreaturePanel.GetNode<LineEdit>("LineEdit2").Text.ToFloat();
        float movementForce = createCreaturePanel.GetNode<LineEdit>("LineEdit4").Text.ToFloat();
        creature.Translation = pos;
        creature.InitializeCreature(mass, radius, movementForce);

        this.AddCreature(creature);
    }

    public List<Creature> GetCreaturesInRadius(Vector3 position, float radius)
    {
        List<Creature> results = new List<Creature>();
        foreach (var creature in this.creatures)
        {
            if (position.DistanceTo(creature.Translation) < radius)
            {
                results.Add(creature);
            }
        }
        return results;
    }

    /// Add a Creature to the World.
    public void AddCreature(Creature creature)
    {
        if (!creature.Initialized)
        {
            GD.Print("ERROR: Creature must be initialized before being added to the World.");
            this.GetTree().Quit();
        }

        creature.world = this;
        creatures.Add(creature);
        this.AddChild(creature);

        creature.Connect(nameof(Creature.CreatureClicked), this, nameof(OnCreatureClicked));
    }

    /// Remove a Creature from the World.
    public void RemoveCreature(Creature creature)
    {
        this.creatures.Remove(creature);
        creature.Disconnect(nameof(Creature.CreatureClicked),this,nameof(OnCreatureClicked));
    }

    /// Returns the Ray projecting from the specified screen position.
    /// The ray is in world space.
    /// The first item in the returned tuple is the origin of the ray, the second item is the direction.
    public Tuple<Vector3, Vector3> GetRay(Vector2 screenPos)
    {
        Camera currentCam = GetViewport().GetCamera();
        Vector3 origin = currentCam.ProjectRayOrigin(screenPos);
        Vector3 direction = currentCam.ProjectRayNormal(screenPos);
        return new Tuple<Vector3, Vector3>(origin, direction);
    }

    /// Project a screen position onto the world's XZ plane.
    public Vector3 ScreenPosToWorldPos(Vector2 screenPos)
    {
        Camera currentCam = GetViewport().GetCamera();
        Plane plane = Plane.PlaneXZ; // XZ plane (XZ plane is at y = 0 by definition)
        var ray = this.GetRay(screenPos);
        Vector3 worldPos = plane.IntersectRay(ray.Item1, ray.Item2);
        return worldPos;
    }

    /// Returns the Creature that is "under" the specified screen position, or null if there isn't one.
    /// In other words, projects a ray from the screen position and sees which creature it hits.
    public Creature GetCreatureAtScreenPos(Vector2 screenPos)
    {
        Vector3 worldPos = this.ScreenPosToWorldPos(screenPos);

        // create a small area at the world pos
        Area area = new Area();
        CollisionShape collisionShape = new CollisionShape();
        BoxShape boxShape = new BoxShape();
        boxShape.Extents = new Vector3(0.1f, 0.1f, 0.1f);
        collisionShape.Shape = boxShape;
        area.AddChild(collisionShape);
        this.AddChild(area);
        area.Translation = worldPos;

        // see if any creature is in this area
        Creature result = null;
        foreach (var item in area.GetOverlappingBodies())
        {
            if (item is Creature creature)
            {
                result = creature;
            }
        }

        // delete temp area
        this.RemoveChild(area);
        area.QueueFree();
        return result;
    }
}
