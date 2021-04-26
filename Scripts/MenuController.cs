using Godot;
using System;

public class MenuController : Node
{
    [Export] NodePath customGameLabelPath;
    [Export] NodePath ControlPath;
    [Export] NodePath menuSoundPlayerPath;

    AudioStreamPlayer menuStreamPlayer;
    Control menuControl;
    Label customGameLabel;

    GameLogic gameLogic;

    public override void _Ready()
    {
        customGameLabel = GetNode(customGameLabelPath) as Label;
        gameLogic = GetNode("/root/GameScene/GameLogic") as GameLogic;
        menuControl = GetNode(ControlPath) as Control;
        menuStreamPlayer = GetNode(menuSoundPlayerPath) as AudioStreamPlayer;
    }

    void _onSliderChange(float value)
    {
        float val = value;
        customGameLabel.Text = val.ToString();
        gameLogic.CreateGame((uint)val);
    }

    //easy
    void _onEasyHover()
    {
        gameLogic.CreateGame(6);
    }

    //medium
    void _onMedHover()
    {
        gameLogic.CreateGame(20);
    }

    //hard
    void _onHardHover()
    {
        gameLogic.CreateGame(42);
    }

    void _onButtonPressed()
    {
        Random random = new Random();
        int id = (random.Next() % 6) + 1;

        menuStreamPlayer.Stream = ResourceLoader.Load($"Sounds/Menu/StartGame{id}.wav") as AudioStream;
        menuStreamPlayer.Play();

        menuControl.Hide();
    }
}
