using Godot;
using System;

public class GUI : Control
{
    // Emitted when the GUI requests that some creatures be created.
    [Signal]
    public delegate void CreateCreatures(float mass, float radius, int number);

    // Emitted when the GUI requests that the food spawn rate be set.
    [Signal]
    public delegate void SetFoodSpawnRate(float rate);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // when show gui button is clicked, show or hide gui
        CheckButton showGuiBtn = (CheckButton)this.GetNode("CheckButton");
        showGuiBtn.Connect("pressed", this, "OnShowGuiPressed");

        // when create creatures button is clicked, emit signal
        Button createCreaturesBtn = (Button)this.GetNode("TabContainer/Creature/Panel/Button");
        createCreaturesBtn.Connect("pressed", this, "OnCreateCreaturesPressed");

        // when update food spawn rate button is clicked, update food spawn rate
        Button updateFoodBtn = (Button)this.GetNode("TabContainer/World/Panel/Button");
        updateFoodBtn.Connect("pressed", this, "OnUpdateFoodSpawnRatePressed");

        HSlider foodSpawnSlider = (HSlider)this.GetNode("TabContainer/World/Panel/HSlider");
        foodSpawnSlider.Connect("value_changed",this,"OnFoodSliderValueChanged");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }


    void OnCreateCreaturesPressed()
    {
        LineEdit massInput = (LineEdit)this.GetNode("Panel/Panel2/LineEdit");
        LineEdit radiusInput = (LineEdit)this.GetNode("Panel/Panel2/LineEdit2");
        LineEdit numberInput = (LineEdit)this.GetNode("Panel/Panel2/LineEdit3");

        float temp;
        if (string.IsNullOrEmpty(massInput.Text) || !float.TryParse(massInput.Text, out temp))
        {
            return;
        }
        if (string.IsNullOrEmpty(radiusInput.Text) || !float.TryParse(radiusInput.Text, out temp))
        {
            return;
        }
        int tempInt;
        if (string.IsNullOrEmpty(numberInput.Text) || !int.TryParse(numberInput.Text, out tempInt))
        {
            return;
        }


        float mass = massInput.Text.ToFloat();
        float radius = radiusInput.Text.ToFloat();
        int number = (int)numberInput.Text.ToFloat();

        this.EmitSignal("CreateCreatures", mass, radius, number);
    }

    // Executed when the show gui button is toggled.
    void OnShowGuiPressed()
    {
        CheckButton showGuiBtn = (CheckButton)this.GetNode("CheckButton");
        TabContainer controlToHide = (TabContainer)this.GetNode("TabContainer");

        if (showGuiBtn.Pressed)
        {
            controlToHide.Visible = true;
        }
        else
        {
            controlToHide.Visible = false;
        }
    }

    void OnUpdateFoodSpawnRatePressed()
    {
        LineEdit input = (LineEdit)this.GetNode("TabContainer/World/Panel/LineEdit");
        float rate = input.Text.ToFloat();
        this.EmitSignal("UpdateFoodSpawnRate",rate);
    }

    void OnFoodSliderValueChanged(float value){
        this.EmitSignal("UpdateFoodSpawnRate",value);
    }
}
