using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameLogic : Node
{
    [Export] NodePath cardNodeHolderPath;
    [Export] PackedScene cardModelScene;
    [Export] Vector2 cardSize;
    [Export] NodePath deckNodePath;
    [Export] NodePath controlGUIPath;

    List<Card> cardArray = new List<Card>();
    Card lastSelectedCard = null;

    int cardCount = 0;
    uint pointCounter = 0, currentCardAmount = 0;

    AudioManager audioManager;

    public override void _Ready()
    {
        audioManager = GetNode("/root/GameScene/AudioManager") as AudioManager;
        CreateGame(6);
    }

    public void CreateGame(uint cardAmount)
    {
        currentCardAmount = cardAmount;

        if (cardAmount < 2) cardAmount = 2;
        else if (cardAmount % 2 != 0)
        {
            cardAmount++;
        }

        AudioServer.SetBusSolo(2, false); //restores buses

        //cleans up things and starts a game
        RestartGame();
        GenerateCards(cardAmount);
        Vector2 deckDimensions = DistributeCards(cardAmount);
        ResizeDeck(deckDimensions);
    }

    void RestartGame()
    {
        //clear current game
        pointCounter = 0;
        ResetCardCounter();

        foreach (Node cardModel in cardArray)
        {
            cardModel.QueueFree();
        }

        cardArray.Clear();
    }

    public void ProcessCard(Card currentCard)
    {
        //make sure we dont select a invalid card
        if (currentCard.soundID == -1)
        {
            return;
        }

        //if we already selected a card
        if (lastSelectedCard != null)
        {
            //check for match
            if (currentCard.soundID == lastSelectedCard.soundID)
            {
                pointCounter++;

                //play special match sound
                currentCard.PlayMatchSound();

                currentCard.FleetCard();
                lastSelectedCard.FleetCard();
                ResetCardCounter();

                if (pointCounter >= currentCardAmount / 2)
                {
                    WinGame();
                }
            }
            else
            {
                //cards didnt match
                currentCard.HideCard();
                lastSelectedCard.HideCard();
            }
            lastSelectedCard = null;
            return;
        }

        //if we havent selected a card
        lastSelectedCard = currentCard;
    }

    public bool CheckCard()
    {
        cardCount++;
        if (cardCount > 2)
        {
            return false;
        }

        return true;
    }

    public void ResetCardCounter()
    {
        cardCount = 0;
    }

    Vector2 DistributeCards(uint cardAmount)
    {
        Spatial cardNodeHolder = GetNode(cardNodeHolderPath) as Spatial;

        //amount of cards in the X axis
        int deckWidth = (int)Mathf.Sqrt(cardAmount) + 1;

        //distribute cards on the deck
        Vector2 cardPosition = Vector2.Zero;
        int cardsY = 0;
        for (int i = 0; i < cardArray.Count; i++)
        {
            Card tempCard = cardArray[i];

            if (i % deckWidth == 0)
            {
                cardPosition.x = 0;
                cardPosition.y += cardSize.y;
                cardsY++;
            }
            cardPosition.x += cardSize.x;

            //instance card
            cardNodeHolder.AddChild(tempCard);

            //setup card
            tempCard.Translate(new Vector3(cardPosition.x, 0, cardPosition.y));
        }

        //adjust position of the cards and the deck
        cardNodeHolder.Translation = new Vector3(-(deckWidth * cardSize.x + 2) / 2 - ((cardSize.x / 2) - 1),
                                                0,  //wtf how does this work
                                                -((cardsY * cardSize.y + 2) / 2) - ((cardSize.y / 2) - 1));

        return new Vector2(deckWidth, cardsY);
    }

    void GenerateCards(uint cardAmount)
    {
        for (int i = 0; i < cardAmount; i++)
        {
            Card tempCard = cardModelScene.Instance() as Card;
            tempCard.SetSound((uint)Mathf.PosMod(i, cardAmount / 2));
            cardArray.Add(tempCard);
        }

        //randomize cards
        RandomNumberGenerator random = new RandomNumberGenerator();
        random.Randomize();
        cardArray = cardArray.OrderBy(x => random.Randi()).ToList();
    }

    void ResizeDeck(Vector2 cardDistribution)
    {
        MeshInstance deckMesh = GetNode(deckNodePath) as MeshInstance;
        float deckX = cardDistribution.x * cardSize.x / 2;
        float deckY = cardDistribution.y * cardSize.y / 2;
        deckMesh.Scale = new Vector3(deckX, 1, deckY);

        //place the palm trees randomly, there are only 3 so no big deal
        RandomNumberGenerator ranGen = new RandomNumberGenerator();

        Spatial[] palms = new Spatial[3];
        palms[0] = GetNode("/root/GameScene/Palm") as Spatial;
        palms[1] = GetNode("/root/GameScene/Palm2") as Spatial;
        palms[2] = GetNode("/root/GameScene/Palm3") as Spatial;

        foreach (Spatial p in palms)
        {
            ranGen.Randomize();
            uint side = (uint)ranGen.RandiRange(0, 3);

            float deckXMargin = deckX + 5;
            float deckYMargin = deckY + 5;

            if (side == 0 || side == 1)
            {
                p.Translation = new Vector3((side == 0) ? deckXMargin : -deckXMargin, 1,
                                            ranGen.RandfRange(-deckYMargin, deckYMargin));
            }
            else if (side == 2 || side == 3)
            {
                p.Translation = new Vector3(ranGen.RandfRange(-deckXMargin, deckXMargin), 1,
                                            (side == 2) ? deckYMargin : -deckYMargin);
            }
        }
    }

    void WinGame()
    {
        audioManager.PlayVictorySoundEffect();
    }

    public void ReturnToMenu()
    {
        Control controlGUI = GetNode(controlGUIPath) as Control;
        controlGUI.Show();

        audioManager.RestartMusic();

        CreateGame(6); //set easy game as preview default
    }

    void _onVictoryAudioPlayerFinish()
    {
        ReturnToMenu();
    }
}
