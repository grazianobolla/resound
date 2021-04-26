using Godot;
using System;

public class MenuController : Node
{
    void _onSliderChange(float value)
    {
        GD.Print(value);
    }
}
