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
            
            rankImage.gameObject.SetActive(true);
            suitImage.gameObject.SetActive(true);

            rankImage.sprite = atlas.GetRankSprite(card.Rank);
            suitImage.sprite = atlas.GetSuitSprite(card.Suit);
        }
    }
}