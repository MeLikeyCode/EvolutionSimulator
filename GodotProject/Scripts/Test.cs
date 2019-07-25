using Godot;
using System;

public class Test : Button
{
    Label label_;

    // TODO delete this script and associated scene
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        label_ = (Label)this.GetNode("../Label");
        this.Connect("pressed",this,"on_button_pressed");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    void on_button_pressed(){
        label_.Text = "you changed me!";
    }
}
