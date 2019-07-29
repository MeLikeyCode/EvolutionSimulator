using Godot;
using System;

public class Creature : RigidBody
{
    public World world;
    public float radius;
    public float mass;
    public float movementForceMag;
    public float max_energy;
    public float current_energy;
    Timer replicationTimer_;
    PackedScene creatureGenerator_;

    public bool ateAFood = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (world == null)
        {
            GD.Print("ERROR: Creature's 'world' attribute must be set before it enters scene tree.");
        }

        float mass = 2;
        float radius = 2;
        float movementForceMag = 10;
        SetProperties(mass, radius, movementForceMag);

        current_energy = max_energy;

        replicationTimer_ = new Timer();
        this.AddChild(replicationTimer_);
        replicationTimer_.WaitTime = (float)GD.RandRange(7,10);
        replicationTimer_.Connect("timeout", this, "on_replicationTimer_Timeout_");
        replicationTimer_.Start();

        creatureGenerator_ = (PackedScene)GD.Load("res://Scenes/Creature.tscn");
    }

    public override void _InputEvent(Godot.Object camera, InputEvent @event, Vector3 clickPosition, Vector3 clickNormal, int shapeIdx){
        GD.Print("exe");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        move_(delta);

        // use some energy to live        
        float ENERGY_BURN_RATE = this.mass;
        current_energy -= ENERGY_BURN_RATE * delta;
        if (current_energy < 0)
            this.QueueFree();
    }

    public void SetProperties(float mass, float radius, float movementForceMag)
    {
        this.mass = mass;
        this.radius = radius;
        this.movementForceMag = movementForceMag;

        max_energy = mass * 50;

        // make looks match physical properties
        SphereMesh mesh = new SphereMesh();
        ((MeshInstance)this.GetNode("MeshInstance")).Mesh = mesh;
        mesh.Radius = radius;
        mesh.Height = radius * 2;

        SpatialMaterial material = new SpatialMaterial();

        float brightness = (this.mass / 2.0f) * 1.0f;
        material.AlbedoColor = new Color(brightness, brightness, brightness);
        mesh.Material = material;
    }

    private void move_(float delta)
    {
        // rotate randomly
        float rotation = this.Rotation.y + Mathf.Deg2Rad((float)GD.RandRange(-15, 15));
        this.Rotation = new Vector3(this.Rotation.x, rotation, this.Rotation.z);

        // move forward
        this.AddCentralForce(this.Transform.basis.z.Normalized() * -1 * movementForceMag); // TODO remove -1 and see if creatures still move properly
        this.current_energy -= movementForceMag / 2.0f;

        // if touching "wall", rotate towards center
        float H_BOUND = world.width / 2.0f;
        float V_BOUND = world.height / 2.0f;
        bool touchingHWall = this.Translation.x > H_BOUND || this.Translation.x < -H_BOUND;
        bool touchingVWall = this.Translation.z > V_BOUND || this.Translation.z < -V_BOUND;
        if (touchingHWall || touchingVWall)
        {
            this.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        }

    }

    void on_replicationTimer_Timeout_()
    {
        // if this creature hasn't eaten a single food, do not replicate
        if (!ateAFood){
            return;
        }

        Creature creature = (Creature)creatureGenerator_.Instance();
        creature.world = this.world;
        world.AddChild(creature);

        creature.Translation = this.Translation;
        creature.Rotation = this.Rotation;
        float childMass = Utilities.RandomizeValue(this.mass, 10);
        float childRadius = Utilities.RandomizeValue(this.radius, 10);
        float childMoveForceMag = Utilities.RandomizeValue(this.movementForceMag,10);
        creature.SetProperties(childMass, childRadius,childMoveForceMag);
        creature.current_energy = creature.max_energy;
    }
}
