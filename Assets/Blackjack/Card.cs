using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum Suit
{
    Clubs,
    Diamonds,
    Hearts,
    Spades,
}

public enum Rank : int
{
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
    Ace = 14
    
}
public class Card
{
    [SerializeField] SpriteRenderer cardSprite;
    [SerializeField] public Rank Rank;
    [SerializeField] public Suit Suit;
    [SerializeField] int cardValue;

    
    public Card(Rank rank, Suit suit)
    {
        this.Rank = rank;
        this.Suit = suit;
        this.cardValue = GetValue();
    }
    public int GetValue()
    {
        // J, Q, K are worth 10
        if (Rank >= Rank.Jack && Rank <= Rank.King)
            return 10;
        
        // Ace defaults to 11
        if (Rank == Rank.Ace)
            return 11;

        // 2 through 10 match their enum numerical values
        return (int)Rank;
    }
    
}
