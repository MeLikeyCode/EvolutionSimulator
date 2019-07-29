using Godot;
using System;

public class GUI : Control
{
    // Emitted when the GUI requests that some creatures be created.
    [Signal]
    public delegate void CreateCreatures(float mass, float radius, float movementForceMag, int number);

    // Emitted when the GUI requests that the food spawn rate be set.
    [Signal]
    public delegate void SetFoodSpawnRate(float rate);

    // Emitted when the GUI requests that the time scale be changed.
    [Signal]
    public delegate void SetTimeScale(float value);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // when show gui button is clicked, show or hide gui
        CheckButton showGuiBtn = (CheckButton)this.GetNode("CheckButton");
        showGuiBtn.Connect("pressed", this, nameof(OnShowGuiPressed));

        // when create creatures button is clicked, emit signal
        Button createCreaturesBtn = (Button)this.GetNode("TabContainer/Creature/Panel/Panel/Button");
        createCreaturesBtn.Connect("pressed", this, nameof(OnCreateCreaturesPressed));

        // when update food spawn rate button is clicked, update food spawn rate
        Button updateFoodBtn = (Button)this.GetNode("TabContainer/World/Panel/Panel/Button");
        updateFoodBtn.Connect("pressed", this, nameof(OnUpdateFoodSpawnRatePressed));

        HSlider foodSpawnSlider = (HSlider)this.GetNode("TabContainer/World/Panel/Panel/HSlider");
        foodSpawnSlider.Connect("value_changed",this,nameof(OnFoodSliderValueChanged));

        // when time scale slider is changed, emit signal
        HSlider timeSlider = (HSlider)this.GetNode("TabContainer/World/Panel/Panel2/HSlider");
        timeSlider.Connect("value_changed",this,nameof(OnTimeSliderValueChanged));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }


    void OnCreateCreaturesPressed()
    {
        LineEdit massInput = (LineEdit)this.GetNode("TabContainer/Creature/Panel/Panel/LineEdit");
        LineEdit radiusInput = (LineEdit)this.GetNode("TabContainer/Creature/Panel/Panel/LineEdit2");
        LineEdit numberInput = (LineEdit)this.GetNode("TabContainer/Creature/Panel/Panel/LineEdit3");
        LineEdit moveForceInput = (LineEdit)this.GetNode("TabContainer/Creature/Panel/Panel/LineEdit4");

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
        float movementForceMag = moveForceInput.Text.ToFloat();
        int number = (int)numberInput.Text.ToFloat();

        this.EmitSignal(nameof(CreateCreatures), mass, radius, movementForceMag, number);
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
        LineEdit input = (LineEdit)this.GetNode("TabContainer/World/Panel/Panel/LineEdit");
        float rate = input.Text.ToFloat();
        this.EmitSignal(nameof(SetFoodSpawnRate),rate);
    }

    void OnFoodSliderValueChanged(float value){
        this.EmitSignal(nameof(SetFoodSpawnRate),value);
    }

    void OnTimeSliderValueChanged(float value){
        this.EmitSignal(nameof(SetTimeScale),value);
    }
}
