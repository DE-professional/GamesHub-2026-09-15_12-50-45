using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardAtlas", menuName = "Blackjack/Card Atlas")]
public class CardAtlas : ScriptableObject
{
    [Header("Base Assets")]
    public Sprite cardBackground;
    public Sprite cardBack;

    [System.Serializable]
    public struct SuitSprite
    {
        public Suit suit;
        public Sprite sprite;
    }

    [System.Serializable]
    public struct RankSprite
    {
        public Rank rank;
        public Sprite sprite;
    }

    [Header("Overlays")]
    public List<SuitSprite> suitSprites;
    public List<RankSprite> rankSprites;

    // Helper lookup methods
    public Sprite GetSuitSprite(Suit suit)
    {
        var match = suitSprites.Find(s => s.suit == suit);
        return match.sprite;
    }

    public Sprite GetRankSprite(Rank rank)
    {
        var match = rankSprites.Find(r => r.rank == rank);
        return match.sprite;
    }
}