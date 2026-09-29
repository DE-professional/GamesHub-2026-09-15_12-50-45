using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class BlackJackManager : MonoBehaviour
{
    [SerializeField] CardAtlas cardAtlas;
    [SerializeField] GameObject cardPrefab;
    [SerializeField] private Transform playerHandContainer;
    [SerializeField] private Transform dealerHandContainer;
    
    [SerializeField] TextMeshProUGUI _playerHandText;
    [SerializeField] TextMeshProUGUI _dealerHandText;
    [SerializeField] private Button _hitButton;
    [SerializeField] private Button _standButton;
    [SerializeField] private Button _doubleDownButton;
    [SerializeField] private Button _splitButton;
    
    private List<Card> playerHand = new List<Card>();
    private List<Card> dealerHand =  new List<Card>();
    private List<Card> deck = new List<Card>();

    private void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        CreateDeck();
        print(deck.Count);
    }
    void CreateCardUI(Card card, Transform container, bool isFaceDown = false)
    {
        GameObject newCardObj = Instantiate(cardPrefab, container);
        CardDisplay cardDisplay = newCardObj.GetComponent<CardDisplay>();

        cardDisplay.DisplayCard(card, cardAtlas, isFaceDown);
    }
    void CreateDeck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank face in Enum.GetValues(typeof(Rank)))
            {
                Card card = new Card(face, suit);
                deck.Add(card);
            }
        }
    }

    void CalculateHandValue(List<Card> hand)
    {
        
    }

    public void OnHitButtonClicked()
    {
        
    }

    public void OnStandButtonClicked()
    {
        
    }

    public void OnDoubleDownButtonClicked()
    {
        
    }

    public void OnSplitButtonClicked()
    {
        
    }
    
}
