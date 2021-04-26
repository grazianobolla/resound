using Godot;
using System;

public class Card : Spatial
{
    //audio
    public int soundID = -1;
    AudioStreamPlayer audioPlayer;
    AudioStream cardSound, matchCardSound;

    //animations
    AnimationPlayer animationPlayer;

    //logic
    GameLogic gameLogic;
    bool isSelected = false;
    bool isFleeting = false;

    float fleetAngle;
    [Export] float fleetingSpeed = 16.0f;

    public override void _Ready()
    {
        animationPlayer = GetNode("AnimationPlayer") as AnimationPlayer;
        gameLogic = GetNode("/root/GameScene/GameLogic") as GameLogic;
    }

    public override void _Process(float delta)
    {
        if (isFleeting == true)
        {
            Translate(new Vector3(Mathf.Cos(fleetAngle), 0, Mathf.Sin(fleetAngle)) * delta * fleetingSpeed);
        }
    }

    public void SetSound(uint id)
    {
        soundID = (int)id + 1; //TODO: rename sounds start from 0
        cardSound = ResourceLoader.Load($"Sounds/CardReveal/CardReveal{soundID}.wav") as AudioStream;
        matchCardSound = ResourceLoader.Load($"Sounds/CardRevealWin/CardRevealWin{soundID}.wav") as AudioStream;
        audioPlayer = GetNode("Audio") as AudioStreamPlayer;
        audioPlayer.Stream = cardSound;
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

        //set fleeting coordinates
        isFleeting = true;

        //sets a random angle for the fleeting card
        RandomNumberGenerator rand = new RandomNumberGenerator();
        rand.Randomize();
        fleetAngle = rand.RandfRange(0, Mathf.Pi * 2);

        GD.Print("Fleeting with angle: " + fleetAngle);
    }

    public void PlayMatchSound()
    {
        audioPlayer.Stream = matchCardSound;
        audioPlayer.Play();
    }

    void _onAnimationFinished(string animation)
    {
        //when the reveal animation finishes, we call the Check() function, this is only for aesthetic
        switch (animation)
        {
            case "Reveal":
                animationPlayer.Play("Hover");

                if (audioPlayer.Stream != matchCardSound)
                {
                    audioPlayer.Stream = cardSound;
                    audioPlayer.Play();
                }

                gameLogic.ProcessCard(this);
                break;
            case "Fleet":
                isFleeting = false;
                Visible = false;
                break;
            case "Hide":
                gameLogic.ResetCardCounter();
                break;
            default:
                break;
        }
    }

    void _onAreaInputEvent(Node camera, InputEvent evnt, Vector3 click_position, Vector3 click_normal, int shape_index)
    {
        if (evnt is InputEventMouseButton input_mouse_button && input_mouse_button.Pressed == true && input_mouse_button.ButtonIndex == (int)ButtonList.Left)
        {
            GD.Print("CardID: " + soundID);
            if (isSelected == false && gameLogic.CheckCard())
            {
                RevealCard();
            }
        }
    }
}
