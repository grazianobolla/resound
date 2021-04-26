using Godot;
using System;

public class MenuController : Node
{
    [Export] NodePath ControlPath;
    [Export] NodePath menuSoundPlayerPath;

    AudioStreamPlayer menuStreamPlayer;
    Control menuControl;

    GameLogic gameLogic;

    public override void _Ready()
    {
        gameLogic = GetNode("/root/GameScene/GameLogic") as GameLogic;
        menuControl = GetNode(ControlPath) as Control;
        menuStreamPlayer = GetNode(menuSoundPlayerPath) as AudioStreamPlayer;
    }

    void _onSliderChange(float value)
    {
        gameLogic.CreateGame((uint)value);
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

        AudioServer.SetBusMute(1, true);

        menuStreamPlayer.Stream = ResourceLoader.Load($"Sounds/Menu/StartGame{id}.wav") as AudioStream;
        menuStreamPlayer.Play();

        menuControl.Hide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey key_event)
        {
            if (key_event.Pressed)
            {
                if (key_event.Scancode == (int)KeyList.F11)
                {
                    if (OS.WindowFullscreen == true) OS.WindowFullscreen = false;
                    else OS.WindowFullscreen = true;
                }
                else if (key_event.Scancode == (int)KeyList.Escape)
                {
                    gameLogic.ReturnToMenu();
                }
            }

            return;
        }
    }
}
