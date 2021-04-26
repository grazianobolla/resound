using Godot;
using System;
using System.Collections.Generic;

public class GameLogic : Node
{
    [Export] NodePath cardNodeHolderPath;
    [Export] PackedScene cardModelScene;
    [Export] uint cards;
    [Export] Vector2 cardSize;
    [Export] NodePath deckNodePath;

    //logic
    List<Card> cardArray = new List<Card>();

    uint pointCounter = 0;
    Card lastSelectedCard = null;
    int cardCount = 0;

    void RestartGame()
    {
        //clear current game
        pointCounter = 0;

        foreach (Node cardModel in cardArray)
        {
            cardModel.QueueFree();
        }

        cardArray.Clear();
    }

    void CreateDeck(uint cardAmount)
    {
        Spatial cardNodeHolder = GetNode(cardNodeHolderPath) as Spatial;

        if (cardAmount < 2) cardAmount = 2;
        else if (cardAmount % 2 != 0) cardAmount++;

        //amount of cards in the X axis
        int deckWidth = (int)Mathf.Sqrt(cardAmount) + 1;

        //distribute cards on the deck
        Vector2 cardPosition = Vector2.Zero;
        int cardsY = 0;
        for (int i = 0; i < cardAmount; i++)
        {
            if (i % deckWidth == 0)
            {
                cardPosition.x = 0;
                cardPosition.y += cardSize.y;
                cardsY++;
            }
            cardPosition.x += cardSize.x;

            //instance card
            Card tempCard = cardModelScene.Instance() as Card;
            cardNodeHolder.AddChild(tempCard);

            //setup card
            tempCard.Translate(new Vector3(cardPosition.x, 0, cardPosition.y));
            tempCard.SetSound((uint)Mathf.PosMod(i, cardAmount / 2));

            //add card to array
            cardArray.Add(tempCard as Card);
        }

        //wtf how does this work
        cardNodeHolder.Translation = new Vector3(-(deckWidth * cardSize.x + 2) / 2 - ((cardSize.x / 2) - 1),
                                                0,
                                                -((cardsY * cardSize.y + 2) / 2) - ((cardSize.y / 2) - 1));

        ResizeDeck(new Vector2(deckWidth, cardsY));
    }

    void ResizeDeck(Vector2 cardDistribution)
    {
        MeshInstance deckMesh = GetNode(deckNodePath) as MeshInstance;
        deckMesh.Scale = new Vector3(cardDistribution.x * cardSize.x / 2, 1, (cardDistribution.y * cardSize.y / 2));
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
                currentCard.FleetCard();
                lastSelectedCard.FleetCard();

                GD.Print("Points: " + pointCounter);
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

    void _on_HSlider_value_changed(float value)
    {
        RestartGame();
        CreateDeck((uint)value);
    }
}
