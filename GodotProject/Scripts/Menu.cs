using Godot;
using System;

public class Menu : Control
{
    [Signal]
    // Emitted when the quit button is pressed.
    public delegate void QuitApp();


    public override void _GuiInput(InputEvent @event){
        if (@event is InputEventKey asKeyEvent){
            if (asKeyEvent.Pressed && asKeyEvent.Scancode == (int)KeyList.Escape){
                this.Visible = false;
                this.AcceptEvent();
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var quitButton = this.GetNode<Button>("Panel/Button");
        quitButton.Connect("pressed",this,nameof(OnQuitPressed));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    void OnQuitPressed(){
        this.EmitSignal(nameof(QuitApp));
    }
}
