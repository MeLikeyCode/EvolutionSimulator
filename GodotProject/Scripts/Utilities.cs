using Godot;
public class Utilities
{
    // Randomize a value by a certain percentage.
    // Returns value +- percentage * value.
    public static float RandomizeValue(float value, float percentage){
        float fraction = percentage / 100;
        return value * (float)GD.RandRange(1 - fraction,1 + fraction);
    }

}