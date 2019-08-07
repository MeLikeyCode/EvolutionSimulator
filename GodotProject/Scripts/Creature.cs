using Godot;
using System;

public class Creature : RigidBody
{
    public World world;
    public float radius;
    public float mass;
    public float movementForceMag;
    public float maxEnergy;
    public float currentEnergy;
    Timer replicationTimer_;
    Timer moveTimer_;
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
        float movementForceMag = 20;
        SetProperties(mass, radius, movementForceMag);

        currentEnergy = maxEnergy;

        replicationTimer_ = new Timer();
        this.AddChild(replicationTimer_);
        replicationTimer_.WaitTime = (float)GD.RandRange(7,10);
        replicationTimer_.Connect("timeout", this, "on_replicationTimer_Timeout_");
        replicationTimer_.Start();

        moveTimer_ = new Timer();
        this.AddChild(moveTimer_);
        moveTimer_.WaitTime = 1;
        moveTimer_.Connect("timeout",this,nameof(OnMoveTimerTimeout));
        moveTimer_.Start();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // use some energy to live        
        float ENERGY_BURN_RATE = this.mass;
        currentEnergy -= ENERGY_BURN_RATE * delta;
        if (currentEnergy < 0)
            this.QueueFree();
    }

    public void SetProperties(float mass, float radius, float movementForceMag)
    {
        this.mass = mass;
        this.radius = radius;
        this.movementForceMag = movementForceMag;

        maxEnergy = mass * 50;

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

    private void move_()
    {
        this.LinearVelocity = new Vector3(0,0,0);

        // if touching "wall", face towards center
        float H_BOUND = world.width;
        float V_BOUND = world.height;
        bool touchingHWall = this.Translation.x > H_BOUND || this.Translation.x < -H_BOUND;
        bool touchingVWall = this.Translation.z > V_BOUND || this.Translation.z < -V_BOUND;
        if (touchingHWall || touchingVWall)
        {
            this.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.RotateY(Mathf.Deg2Rad(180)); // rotate 180 degrees since LookAt makes *negative* z face desired point!
        }

        // rotate randomly
        float rotation = this.Rotation.y + Mathf.Deg2Rad((float)GD.RandRange(-40, 40));
        this.Rotation = new Vector3(this.Rotation.x, rotation, this.Rotation.z);

        // move forward
        this.AddCentralForce(this.Transform.basis.z.Normalized() * movementForceMag);
        float distance = movementForceMag / this.mass; // distance the creature will travel in 1 second as a result of this force being applied
        float energyCostToMove = movementForceMag / 20;
        this.currentEnergy -= energyCostToMove;
    }

    void on_replicationTimer_Timeout_()
    {
        // if this creature hasn't eaten a single food, do not replicate
        if (!ateAFood){
            return;
        }

        Creature creature = (Creature)this.world.creatureGenerator.Instance();
        creature.world = this.world;
        world.AddChild(creature);

        creature.Translation = this.Translation;
        creature.Rotation = this.Rotation;
        float childMass = Utilities.RandomizeValue(this.mass, 10);
        float childRadius = Utilities.RandomizeValue(this.radius, 10);
        float childMoveForceMag = Utilities.RandomizeValue(this.movementForceMag,10);
        creature.SetProperties(childMass, childRadius,childMoveForceMag);
        creature.currentEnergy = creature.maxEnergy;
    }

    void OnMoveTimerTimeout(){
        move_();
    }
}
