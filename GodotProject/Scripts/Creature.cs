using Godot;
using System;
using System.Collections.Generic;

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
        replicationTimer_.WaitTime = (float)GD.RandRange(7, 10);
        replicationTimer_.Connect("timeout", this, "on_replicationTimer_Timeout_");
        replicationTimer_.Start();

        moveTimer_ = new Timer();
        this.AddChild(moveTimer_);
        moveTimer_.WaitTime = 1;
        moveTimer_.Connect("timeout", this, nameof(OnMoveTimerTimeout));
        moveTimer_.Start();

        this.Connect("body_entered", this, nameof(OnCollisionWithCreature));
        this.ContactMonitor = true;
        this.ContactsReported = 10;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // use some energy to live        
        float ENERGY_BURN_RATE = this.mass;
        currentEnergy -= ENERGY_BURN_RATE * delta;
        if (currentEnergy < 0){
            this.QueueFree();
            this.world.creatures.Remove(this);
        }
    }

    // Use this callback to modify physics related variables.
    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        // this.LinearVelocity = new Vector3(this.LinearVelocity.x,0,this.LinearVelocity.z); // TODO look here
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
        bool touchingHWall = this.Translation.x > world.width || this.Translation.x < 0;
        bool touchingVWall = this.Translation.z > world.height || this.Translation.z < 0;
        if (touchingHWall || touchingVWall)
        {
            this.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.RotateY(Mathf.Deg2Rad(180)); // rotate 180 degrees since LookAt makes *negative* z face desired point!
        }

        // calculate movement vector (take into account all nearby creatures)
        var nearbyBiggerCreatures = this.world.GetCreaturesInRadius(this.Translation,10);
        foreach (var creature in nearbyBiggerCreatures.ToArray())
        {
            if (creature.mass < this.mass){
                nearbyBiggerCreatures.Remove(creature);
            }
        }
        List<Vector3> allVectors = new List<Vector3>();
        foreach (var creature in nearbyBiggerCreatures)
        {
            Vector3 vector = this.Translation - creature.Translation;
            allVectors.Add(vector);
        }
        Vector3 overallMoveVector = new Vector3();
        foreach (var vector in allVectors)
        {
            overallMoveVector += vector;
        }
        this.LookAt(this.Translation + -1 * overallMoveVector,new Vector3(0,1,0));
        if (allVectors.Count <= 1){
            float rotation = this.Rotation.y + Mathf.Deg2Rad((float)GD.RandRange(-40, 40));
            this.Rotation = new Vector3(this.Rotation.x, rotation, this.Rotation.z);
        }

        // move forward
        this.AddCentralForce(this.Transform.basis.z.Normalized() * movementForceMag);
        float distance = movementForceMag / this.mass; // distance the creature will travel in 1 second as a result of this force being applied
        float energyCostToMove = movementForceMag / 40;
        this.currentEnergy -= energyCostToMove;
    }

    void on_replicationTimer_Timeout_()
    {
        // if this creature hasn't eaten a single food, do not replicate
        if (!ateAFood)
        {
            return;
        }

        // if this creature doesn't have enough energy to give birth, do not replicate
        float costForBaby = this.mass * 15;
        if (this.currentEnergy < costForBaby)
            return;

        // replicate
        Creature creature = (Creature)this.world.creatureGenerator.Instance();
        creature.world = this.world;
        world.AddChild(creature);
        world.creatures.Add(creature);

        creature.Translation = this.Translation;
        creature.Rotation = this.Rotation;
        float childMass = Utilities.RandomizeValue(this.mass, 10);
        float childRadius = Utilities.RandomizeValue(this.radius, 10);
        float childMoveForceMag = Utilities.RandomizeValue(this.movementForceMag, 10);
        creature.SetProperties(childMass, childRadius, childMoveForceMag);
        creature.currentEnergy = creature.maxEnergy;

        this.currentEnergy -= costForBaby;
    }

    void OnMoveTimerTimeout()
    {
        move_();
    }

    // Executed when this creature collides with another.
    void OnCollisionWithCreature(Node otherCreature)
    {
        if (otherCreature is Creature asCreature)
        {
            if (this.mass != asCreature.mass)
            {
                Creature biggerCreature = this.mass > asCreature.mass ? this : asCreature;
                Creature smallerCreature = biggerCreature == this ? asCreature : this;
                biggerCreature.currentEnergy += smallerCreature.mass * 100;
                smallerCreature.QueueFree();
                smallerCreature.world.creatures.Remove(smallerCreature);
            }
        }
    }
}
