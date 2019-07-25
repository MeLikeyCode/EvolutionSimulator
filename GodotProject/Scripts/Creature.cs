using Godot;
using System;

public class Creature : Area
{
    public float radius;
    public float mass;
    public float max_energy;
    public float speed;
    public float current_energy;
    Timer replicationTimer_;
    PackedScene creatureGenerator_;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        float mass = 2;
        float radius = 2;
        SetProperties(mass, radius);

        current_energy = max_energy;

        replicationTimer_ = new Timer();
        this.AddChild(replicationTimer_);
        replicationTimer_.WaitTime = 10;
        replicationTimer_.Connect("timeout", this, "on_replicationTimer_Timeout_");
        replicationTimer_.Start();

        creatureGenerator_ = (PackedScene)GD.Load("res://Scenes/Creature.tscn");
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

    public void SetProperties(float mass, float radius)
    {
        this.mass = mass;
        this.radius = radius;

        max_energy = mass * 25;
        speed = 5 / mass;

        // make looks match physical properties
        SphereMesh mesh = new SphereMesh();
        ((MeshInstance)this.GetNode("MeshInstance")).Mesh = mesh;
        mesh.Radius = radius;
        mesh.Height = radius * 2;

        SpatialMaterial material = new SpatialMaterial();

        float brightness = this.mass * 0.5f;
        material.AlbedoColor = new Color(brightness, brightness, brightness);
        mesh.Material = material;
    }

    private void move_(float delta)
    {
        // rotate randomly
        float rotation = this.Rotation.y + Mathf.Deg2Rad((float)GD.RandRange(-15, 15));
        this.Rotation = new Vector3(this.Rotation.x, rotation, this.Rotation.z);

        // move forward
        Vector3 forwardVector = this.Transform.basis.z.Normalized() * -1;
        this.Translation += forwardVector * speed * delta; // move forward
        this.current_energy -= delta * mass * 2;

        // if touching "wall", rotate towards center
        float BOUND = 30;
        bool touchingWall = this.Translation.x > BOUND || this.Translation.x < -BOUND || this.Translation.z > BOUND || this.Translation.z < -BOUND;
        if (touchingWall){
            this.LookAt(new Vector3(0,0,0),new Vector3(0,1,0));
        }

    }

    void on_replicationTimer_Timeout_()
    {
        Spatial creatureRoot = (Spatial)creatureGenerator_.Instance();
        Creature childCreature = (Creature)creatureRoot.GetNode("Area");
        creatureRoot.RemoveChild(childCreature);
        creatureRoot.QueueFree();
        this.GetParent().AddChild(childCreature);

        childCreature.Translation = this.Translation;
        childCreature.Rotation = this.Rotation;
        float childMass = Utilities.RandomizeValue(this.mass, 10);
        float childRadius = Utilities.RandomizeValue(this.radius, 10);
        childCreature.SetProperties(childMass, childRadius);
        childCreature.current_energy = childCreature.max_energy;
    }
}