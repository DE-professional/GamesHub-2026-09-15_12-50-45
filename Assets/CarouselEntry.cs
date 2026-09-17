using UnityEngine;

[CreateAssetMenu(fileName = "New Carousel Entry", menuName = "UI/Carousel Entry", order = 0)]

public class CarouselEntry: ScriptableObject
{
    [field: SerializeField] public Sprite EntryGraphic {get; private set;}
    [field: SerializeField] public string Headline {get; private set;}
}
