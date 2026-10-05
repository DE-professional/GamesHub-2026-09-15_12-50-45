using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    [Header("UI Image Components")]
    public Image backgroundImage;
    public Image rankImage;
    public Image suitImage;

    /// <summary>
    /// Configures the card visual components using the provided data and atlas.
    /// </summary>
    public void DisplayCard(Card card, CardAtlas atlas, bool isFaceDown = false)
    {
        if (isFaceDown)
        {
            backgroundImage.sprite = atlas.cardBack;
            rankImage.gameObject.SetActive(false);
            suitImage.gameObject.SetActive(false);
        }
        else
        {
            backgroundImage.sprite = atlas.cardBackground;
            
            

            rankImage.sprite = atlas.GetRankSprite(card.Rank);
            suitImage.sprite = atlas.GetSuitSprite(card.Suit);
            rankImage.gameObject.SetActive(true);
            suitImage.gameObject.SetActive(true);
            Color suitColor = GetSuitColor(card.Suit);
            rankImage.color = suitColor;
            suitImage.color = suitColor;
        }
    }

    private Color GetSuitColor(Suit cardSuit)
    {
        if (cardSuit == Suit.Hearts || cardSuit == Suit.Diamonds)
        {
            return Color.red; // Or custom red: new Color(0.8f, 0.1f, 0.1f)
        }
        else
        {
            return Color.black; // Or custom dark charcoal/black
        }
    }
}