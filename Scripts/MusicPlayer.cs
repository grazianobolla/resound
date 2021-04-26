using Godot;
using System;

public class MusicPlayer : AudioStreamPlayer
{
    [Export] float fade_speed = 1;
    public override void _Process(float delta)
    {
        if (VolumeDb >= -6) return;

        VolumeDb += delta * fade_speed;
    }
}