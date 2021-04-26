using Godot;
using System;

public class RotationHelper : Spatial
{
    [Export] float camera_sensitivity = 5.0f;
    [Export] float interpolation_speed = 3.0f;

    float offset = 0;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseEvent && Input.GetMouseMode() == Input.MouseMode.Captured)
        {
            offset += -mouseEvent.Relative.x * (camera_sensitivity / 1000.0f);
            return;
        }

        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            if (mouseButtonEvent.ButtonIndex == (int)ButtonList.Right)
            {
                if (Input.GetMouseMode() == Input.MouseMode.Captured) Input.SetMouseMode(Input.MouseMode.Visible);
                else Input.SetMouseMode(Input.MouseMode.Captured);
            }

            return;
        }
    }

    public override void _Process(float delta)
    {
        Interpolate(delta);
    }

    void Interpolate(float delta)
    {
        Vector3 temp_rotation = Rotation;
        temp_rotation.y = temp_rotation.y + (offset - temp_rotation.y) * (delta * interpolation_speed);
        Rotation = temp_rotation;
    }
}