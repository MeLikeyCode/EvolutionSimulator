using Godot;
using System;

public class GUI : Control
{
    // Emitted when the create creatures button is pressed.
    [Signal]
    public delegate void CreateCreatures(float mass, float radius, int number);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // when create creatures button is clicked, emit signal
        Button createCreaturesBtn = (Button)this.GetNode("Panel/Panel2/Button2");
        createCreaturesBtn.Connect("pressed", this, "OnCreateCreaturesPressed");

        // when show gui button is clicked, show or hide gui
        CheckButton showGuiBtn = (CheckButton)this.GetNode("CheckButton");
        showGuiBtn.Connect("pressed", this, "OnShowGuiPressed");
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
        Panel panelToHide = (Panel)this.GetNode("Panel");

        if (showGuiBtn.Pressed)
        {
            panelToHide.Visible = true;
        }
        else
        {
            panelToHide.Visible = false;
        }
    }

}
