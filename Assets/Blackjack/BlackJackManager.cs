using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BlackJackManager : MonoBehaviour
{
    [Header("Animation Settings")]
    public Transform deckSpawnPoint; // Assign a UI transform or Empty Object representing the shoe/deck
    public float moveDuration = 0.3f;   // Seconds per card move
    
    
    [Header("Assets & Prefabs")]
    public CardAtlas cardAtlas;
    public GameObject cardPrefab;
    public Transform playerHandContainer;
    public Transform dealerHandContainer;

    [Header("UI Text & Buttons")]
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI dealerText;
    public TextMeshProUGUI statusText;
    public Button hitButton;
    public Button standButton;
    
    private List<Card> playerHand = new List<Card>();
    private List<Card> dealerHand =  new List<Card>();
    private List<Card> deck = new List<Card>();
    
    private CardDisplay dealerHoleCardDisplay;
    private Card dealerHoleCard;
    private bool isHoleCardHidden = false;

    private void Start()
    {
        StartCoroutine(StartNewGameSequence());
    }
    
    IEnumerator StartNewGameSequence()
    {
        CreateDeck();
        ShuffleDeck();
        ClearHandContainers();

        playerHand.Clear();
        dealerHand.Clear();

        statusText.text = "";
        hitButton.interactable = false;
        standButton.interactable = false;

        // Sequential deal with dramatic pauses
        yield return DealCardAnimated(playerHand, playerHandContainer);
        yield return new WaitForSeconds(0.15f);

        yield return DealCardAnimated(dealerHand, dealerHandContainer, isFaceDown: false);
        yield return new WaitForSeconds(0.15f);

        yield return DealCardAnimated(playerHand, playerHandContainer);
        yield return new WaitForSeconds(0.15f);

        yield return DealCardAnimated(dealerHand, dealerHandContainer, isFaceDown: true);

        UpdateUI();
        CheckBlackjack();
        hitButton.interactable = true;
        standButton.interactable = true;
    }

    private void CheckBlackjack()
    {
        bool playerBlackJack = false;
        bool dealerBlackJack = false;
        if (CalculateHandScore(playerHand) == 21) playerBlackJack = true;
        if (CalculateHandScore(dealerHand) == 21) dealerBlackJack = true;
        if (playerBlackJack && dealerBlackJack)
        {
            EndGame(EndState.Push);
        }
        else if (playerBlackJack)
        {
            EndGame(EndState.PlayerWin);
        }
        else
        {
            EndGame(EndState.DealerBlackJack);
        }
    }

    void ClearHandContainers()
    {
        foreach (Transform child in playerHandContainer) Destroy(child.gameObject);
        foreach (Transform child in dealerHandContainer) Destroy(child.gameObject);

        dealerHoleCardDisplay = null;
        dealerHoleCard = null;
        isHoleCardHidden = false;
    }

    void UpdateUI()
    {
        playerText.text = $"Player: {CalculateHandScore(playerHand)}";

        // While the hole card is hidden, display only the value of dealer's visible first card
        if (isHoleCardHidden && dealerHand.Count >= 1)
        {
            dealerText.text = $"Dealer: {dealerHand[0].GetValue()}";
        }
        else
        {
            dealerText.text = $"Dealer: {CalculateHandScore(dealerHand)}";
        }
    }

    private  int CalculateHandScore(List<Card> hand)
    {
        int aceCount = 0;
        int handScore = 0;
        foreach (Card card in hand)
        {
            handScore += card.GetValue();
            if(card.Rank == Rank.Ace) aceCount++;
        }
        while (handScore > 21 && aceCount > 0)
        {
            handScore -= 10;
            aceCount--;
        }

        return handScore;
    }

    
    
    void RevealDealerHoleCard()
    {
        if (isHoleCardHidden && dealerHoleCardDisplay != null)
        {
            // Flip the face-down card to face-up using its actual Card data
            dealerHoleCardDisplay.DisplayCard(dealerHoleCard, cardAtlas, isFaceDown: false);
            isHoleCardHidden = false;
        }
    }
    void CreateDeck()
    {
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                Card card = new Card(suit, rank);
                deck.Add(card);
            }
        }
    }
    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    Card DrawCard()
    {
        Card drawn = deck[0];
        deck.RemoveAt(0);
        return drawn;
    }

    public void OnHitButtonClicked()
    {
        StartCoroutine(DealCardToPlayer());
        if (CalculateHandScore(playerHand) > 21)
        {
            // Reveal hole card when player busts so they can see what dealer had
            StartCoroutine(RevealDealerHoleCardAnimated());
            UpdateUI();
            EndGame(EndState.PlayerBust);
        }
    }
    IEnumerator DealCardToPlayer()
    {
        yield return DealCardAnimated(playerHand, playerHandContainer);
        UpdateUI();
    }
    
    public IEnumerator RevealDealerHoleCardAnimated()
    {
        if (isHoleCardHidden && dealerHoleCardDisplay != null)
        {
            yield return dealerHoleCardDisplay.AnimateFlip(dealerHoleCard, cardAtlas, duration: 0.4f);
            isHoleCardHidden = false;
            UpdateUI();
        }
    }
    IEnumerator DealCardAnimated(List<Card> targetHand, Transform container, bool isFaceDown = false)
    {
        Card card = DrawCard();
        targetHand.Add(card);

        GameObject newCardObj = Instantiate(cardPrefab, container);
        CardDisplay cardDisplay = newCardObj.GetComponent<CardDisplay>();
        cardDisplay.DisplayCard(card, cardAtlas, isFaceDown);

        if (isFaceDown)
        {
            dealerHoleCard = card;
            dealerHoleCardDisplay = cardDisplay;
            isHoleCardHidden = true;
        }

        // Force layout update so container calculates final card position immediately
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        // Slide card from deckSpawnPoint to layout position
        Vector3 spawnPos;
        if (deckSpawnPoint != null)
            spawnPos = deckSpawnPoint.position;
        else
            spawnPos = container.position + new Vector3(0, 300, 0);
        yield return cardDisplay.AnimateMoveFrom(spawnPos, moveDuration);
    }
    public void OnStandButtonClicked()
    {
        StartCoroutine(DealerTurnSequence());
    }

    IEnumerator DealerTurnSequence()
    {
        hitButton.interactable = false;
        standButton.interactable = false;

        // 1. Reveal Hole Card with flip animation
        yield return RevealDealerHoleCardAnimated();
        yield return new WaitForSeconds(0.3f);

        // 2. Dealer draws step-by-step with delay
        while (CalculateHandScore(dealerHand) < 17)
        {
            yield return DealCardAnimated(dealerHand, dealerHandContainer, isFaceDown: false);
            UpdateUI();
            yield return new WaitForSeconds(0.3f);
        }

        EvaluateWinner();
    }

    public void OnDoubleDownButtonClicked()
    {
        
    }

    public void OnSplitButtonClicked()
    {
        
    }
    void EvaluateWinner()
    {
        int pScore = CalculateHandScore(playerHand);
        int dScore = CalculateHandScore(dealerHand);

        if (dScore > 21)
        {
            EndGame(EndState.DealerBust);
        }
        else if (pScore > dScore) EndGame(EndState.PlayerWin);
        else if (dScore > pScore) EndGame(EndState.DealerWin);
        else EndGame(EndState.Push);
    }

    void EndGame(EndState endState)
    {
        if (endState == EndState.DealerWin)
        {
            statusText.text = "Dealer has a higher score. You lose...";
            //Take player chips
        }
        else if (endState == EndState.PlayerWin)
        {
            statusText.text = "You win!";
            //Payout chips
        }
        
        
        hitButton.interactable = false;
        standButton.interactable = false;
    }
    
}

public enum EndState
{
    Push,
    PlayerBust,
    DealerBust,
    PlayerBlackJack,
    DealerBlackJack,
    PlayerWin,
    DealerWin
}
