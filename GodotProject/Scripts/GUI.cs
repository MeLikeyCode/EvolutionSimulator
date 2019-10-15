using Godot;
using System;

public class GUI : Control
{
    // Emitted when the GUI requests that the keyboard/mouse should be ready to place creatures by clicking.
    [Signal]
    public delegate void SetCreatureCreatureMode();

    // Emitted when the GUI requests that some creatures be created.
    [Signal]
    public delegate void CreateCreatures(float mass, float radius, float movementForceMag, int number);

    // Emitted when the GUI requests that the food spawn rate be set.
    // 'numToCreate' - the number of food to create each food spawn time
    // 'delay' - the number of seconds between food spawn times
    [Signal]
    public delegate void SetFoodSpawnRate(int numToCreate, float delay);

    // Emitted when the GUI requests that the time scale be changed.
    [Signal]
    public delegate void SetTimeScale(float value);

    // Emitted when the GUI requests that the world bounds be changed.
    [Signal]
    public delegate void SetWorldBounds(float width, float height);

    // Emitted when the GUI requests that the keyboard/mouse should be ready to paint food.
    [Signal]
    public delegate void SetPaintFoodMode();

    // Emitted when the GUI's select creature button has been pressed.
    [Signal]
    public delegate void SelectCreature();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // when show gui button is clicked, show or hide gui
        CheckButton showGuiBtn = (CheckButton)this.GetNode("CheckButton");
        showGuiBtn.Connect("pressed", this, nameof(OnShowGuiPressed));

        // when create creatures button is clicked, emit signal
        Button createCreaturesBtn = (Button)this.GetNode("TabContainer/Creature/Panel/Panel/Button");
        createCreaturesBtn.Connect("pressed", this, nameof(OnCreateCreaturesPressed));

        // when update food button is clicked, emit signal
        Button updateFoodBtn = (Button)this.GetNode("TabContainer/World/Panel/Panel/Button");
        updateFoodBtn.Connect("pressed", this, nameof(OnUpdateFoodSpawnRatePressed));

        // when time scale slider is changed, emit signal
        HSlider timeSlider = (HSlider)this.GetNode("TabContainer/World/Panel/Panel2/HSlider");
        timeSlider.Connect("value_changed",this,nameof(OnTimeSliderValueChanged));

        // when bounds is changed, emit signal
        Button updateBoundsBtn = this.GetNode<Button>("TabContainer/World/Panel/Panel3/Button");
        updateBoundsBtn.Connect("pressed",this,nameof(OnUpdateBoundsPressed));

        // when paint food button is clicked, emit signal
        Button paintFoodBtn = this.GetNode<Button>("TabContainer/World/Panel/Panel/Button2");
        paintFoodBtn.Connect("pressed",this,nameof(OnPaintFoodClicked));

        // when create creature button is clicked, emit signal
        Button createCreatureBtn = this.GetNode<Button>("TabContainer/Creature/Panel/Panel2/Button");
        createCreatureBtn.Connect("pressed",this,nameof(OnCreateCreaturePressed));

        Button selectCreatureBtn = this.GetNode<Button>("TabContainer/Stats/Panel/Panel2/Button");
        selectCreatureBtn.Connect("pressed",this,nameof(OnSelectCreaturePressed));
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

    void OnCreateCreaturePressed(){
        this.EmitSignal(nameof(SetCreatureCreatureMode));
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
        LineEdit numToCreateInput = (LineEdit)this.GetNode("TabContainer/World/Panel/Panel/LineEdit");
        LineEdit delayInput = (LineEdit)this.GetNode("TabContainer/World/Panel/Panel/LineEdit2");
        int numToCreate = numToCreateInput.Text.ToInt();
        float delay = delayInput.Text.ToFloat();
        this.EmitSignal(nameof(SetFoodSpawnRate),numToCreate,delay);
    }

    void OnTimeSliderValueChanged(float value){
        Label timeScaleLabel = this.GetNode<Label>("TabContainer/World/Panel/Panel2/Label2");
        timeScaleLabel.Text = value.ToString() + "x";
        this.EmitSignal(nameof(SetTimeScale),value);
    }

    void OnUpdateBoundsPressed(){
        float width = this.GetNode<LineEdit>("TabContainer/World/Panel/Panel3/LineEdit").Text.ToFloat();
        float height = this.GetNode<LineEdit>("TabContainer/World/Panel/Panel3/LineEdit2").Text.ToFloat();
        this.EmitSignal(nameof(SetWorldBounds),width,height);
    }

    void OnPaintFoodClicked(){
        this.EmitSignal(nameof(SetPaintFoodMode));
    }

    void OnSelectCreaturePressed(){
        this.EmitSignal(nameof(SelectCreature));
    }
}
