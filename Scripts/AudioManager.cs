using Godot;
using System;

public class AudioManager : Node
{
    AudioStreamPlayer victoryPlayer, menuPlayer, musicPlayer;
    const int BUS_MUSIC = 1, BUS_VICTORY = 2;

    public override void _Ready()
    {
        victoryPlayer = GetNode("/root/GameScene/VictoryPlayer") as AudioStreamPlayer;
        menuPlayer = GetNode("/root/GameScene/MenuPlayer") as AudioStreamPlayer;
        musicPlayer = GetNode("/root/GameScene/MusicPlayer") as AudioStreamPlayer;
    }

    public void RestartMusic()
    {
        AudioServer.SetBusMute(BUS_MUSIC, false); //unmutes the music bus
        musicPlayer.VolumeDb = -50; //creates the "fade" effect
    }

    public void StopMusic()
    {
        AudioServer.SetBusMute(BUS_MUSIC, true); //mutes the music bus
    }

    public void PlayMenuButtonSound()
    {
        //useless rn but useful if you want different sounds for each click
        menuPlayer.Play();
    }

    public void PlayVictorySoundEffect()
    {
        AudioServer.SetBusSolo(BUS_VICTORY, true); //mutes all buses except for this one so you can only hear the win sound effect
        victoryPlayer.Play();
    }
}
