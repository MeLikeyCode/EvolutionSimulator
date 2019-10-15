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
    public bool ateAFood = false;
    public int numChildrenSpanwed = 0;
    public int numCreaturesEaten = 0;
    
    Timer replicationTimer_;
    Timer moveTimer_;
    bool initialized_ = false;

    public bool Initialized{
        get {return initialized_;}
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        // use some energy to live        
        float ENERGY_BURN_RATE = this.mass;
        currentEnergy -= ENERGY_BURN_RATE * delta;
        if (currentEnergy < 0)
        {
            this.DeleteCreature();
        }
    }

    public void InitializeCreature(float mass, float radius, float movementForceMag)
    {
        if (this.Initialized){
            GD.Print("ERROR: Creature is already initialized. Can only be initialized once.");
            GetTree().Quit();
        }

        // calculate/set physical properties
        this.mass = mass;
        this.radius = radius;
        this.movementForceMag = movementForceMag;

        this.maxEnergy = this.mass * 50;
        this.currentEnergy = this.maxEnergy;

        // make looks match physical properties
        SphereMesh mesh = new SphereMesh();
        ((MeshInstance)this.GetNode("MeshInstance")).Mesh = mesh;
        mesh.Radius = radius;
        mesh.Height = radius * 2;

        SpatialMaterial material = new SpatialMaterial();
        float brightness = (this.mass / 2.0f) * 1.0f;
        material.AlbedoColor = new Color(brightness, brightness, brightness);
        mesh.Material = material;

        // create timers/connect signals
        replicationTimer_ = new Timer();
        this.AddChild(replicationTimer_);
        replicationTimer_.WaitTime = (float)GD.RandRange(7, 10);
        replicationTimer_.Connect("timeout", this, "OnReplicationTimerTimeout");
        replicationTimer_.Start();

        moveTimer_ = new Timer();
        this.AddChild(moveTimer_);
        moveTimer_.WaitTime = 1;
        moveTimer_.Connect("timeout", this, nameof(OnMoveTimerTimeout));
        moveTimer_.Start();

        this.Connect("body_entered", this, nameof(OnCollisionWithCreature));
        this.ContactMonitor = true;
        this.ContactsReported = 10;

        this.initialized_ = true;
    }

    private void move_()
    {
        this.LinearVelocity = new Vector3(0, 0, 0);

        // rotate
        // - if there are bigger creatures (preditors) nearby, rotate to face away from all of them
        // - otherwise, face a random direction
        var nearbyBiggerCreatures = new List<Creature>();
        foreach (var creature in this.world.GetCreaturesInRadius(this.Translation, 10))
            if (creature.mass > this.mass && creature != this)
                nearbyBiggerCreatures.Add(creature);

        if (nearbyBiggerCreatures.Count > 0)
        {
            List<Vector3> creatureVectors = new List<Vector3>(); // a list of vectors, each going from this creature to a nearby big creature
            foreach (var creature in nearbyBiggerCreatures)
            {
                Vector3 vector = this.Translation - creature.Translation;
                creatureVectors.Add(vector);
            }
            Vector3 overallMoveVector = new Vector3();
            foreach (var vector in creatureVectors)
                overallMoveVector += vector;
            overallMoveVector *= -1;

            // look away from all creatures
            this.LookAt(this.Translation + overallMoveVector, new Vector3(0, 1, 0));
        }
        else
        {
            // no big creatures nearby - look at a random direction
            float rotation = this.Rotation.y + Mathf.Deg2Rad((float)GD.RandRange(-40, 40));
            this.Rotation = new Vector3(this.Rotation.x, rotation, this.Rotation.z);
        }

        // if touching "wall", face towards center
        bool touchingHWall = this.Translation.x > world.width || this.Translation.x < 0;
        bool touchingVWall = this.Translation.z > world.height || this.Translation.z < 0;
        if (touchingHWall || touchingVWall)
        {
            this.LookAt(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            this.RotateY(Mathf.Deg2Rad(180)); // rotate 180 degrees since LookAt makes *negative* z face desired point!
        }

        // move forward
        this.ApplyCentralImpulse(this.Transform.basis.z.Normalized() * movementForceMag);
        float energyCostToMove = Mathf.Pow(movementForceMag, 2); // energy cost to move = impulse applied ** 2
        this.currentEnergy -= energyCostToMove;
    }

    // Executed when it is time for the creature to replicate.
    void OnReplicationTimerTimeout()
    {
        // if this creature hasn't eaten a single food, do not replicate
        if (!ateAFood)
            return;

        // if this creature doesn't have enough energy to give birth, do not replicate
        float costForBaby = this.mass * 15;
        if (this.currentEnergy < costForBaby)
            return;

        // replicate
        Creature creature = (Creature)this.world.creatureGenerator.Instance();

        float offset = ((BoxShape)(this.GetNode<CollisionShape>("CollisionShape").Shape)).Extents.x;
        creature.Translation = this.Translation + new Vector3(offset * 3, 0, 0);
        creature.Rotation = this.Rotation;
        float childMass = Utilities.RandomizeValue(this.mass, 10);
        float childRadius = Utilities.RandomizeValue(this.radius, 10);
        float childMoveForceMag = Utilities.RandomizeValue(this.movementForceMag, 10);
        creature.InitializeCreature(childMass, childRadius, childMoveForceMag);

        this.currentEnergy -= costForBaby;
        this.numChildrenSpanwed += 1;

        this.world.AddCreature(creature);
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
                biggerCreature.numCreaturesEaten += 1;
                this.DeleteCreature();
            }
        }
    }

    /// Removes the creature from the World and then QueueFrees it.
    public void DeleteCreature()
    {
        this.world.RemoveCreature(this);
        this.QueueFree();
    }
}
