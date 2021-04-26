using Godot;
using System;

public class Card : Spatial
{
    //audio
    public int soundID = -1;
    AudioStreamPlayer audioPlayer;
    AudioStream cardSound;

    //animations
    AnimationPlayer animationPlayer;

    //logic
    GameLogic gameLogic;
    bool isSelected = false;

    public override void _Ready()
    {
        audioPlayer = GetNode("Audio") as AudioStreamPlayer;
        animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        gameLogic = GetNode("/root/GameScene/GameLogic") as GameLogic;
    }

    public void SetSound(uint id)
    {
        soundID = (int)id;
        cardSound = ResourceLoader.Load($"Sounds/Cards/CardReveal{id}.wav") as AudioStream;
    }

    public void HideCard()
    {
        animationPlayer.Play("Hide");
        isSelected = false;
    }

    public void RevealCard()
    {
        animationPlayer.Play("Reveal");
        isSelected = true;
    }

    public void FleetCard()
    {
        soundID = -1; //makes card invalid just in case
        animationPlayer.Play("Fleet");
        isSelected = true;

    }

    void _onAnimationFinished(string animation)
    {
        //when the reveal animation finishes, we call the Check() function, this is only for aesthetic
        switch (animation)
        {
            case "Reveal":
                audioPlayer.Stream = cardSound;
                audioPlayer.Play();
                break;
            case "Fleet":
                Visible = false;
                gameLogic.ResetCardCounter();
                break;
            case "Hide":
                gameLogic.ResetCardCounter();
                break;
            default:
                break;
        }
    }

    void _onAudioFinished()
    {
        gameLogic.ProcessCard(this);
    }

    void _onAreaInputEvent(Node camera, InputEvent evnt, Vector3 click_position, Vector3 click_normal, int shape_index)
    {
        if (evnt is InputEventMouseButton input_mouse_button && input_mouse_button.Pressed == true && input_mouse_button.ButtonIndex == (int)ButtonList.Left)
        {
            if (isSelected == false)
            {
                if (gameLogic.CheckCard())
                    RevealCard();
            }
        }
    }
}
