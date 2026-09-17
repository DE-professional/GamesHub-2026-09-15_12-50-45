using System.Linq;
using UnityEngine;
using FancyCarouselView.Runtime.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class DemoData
{
    public string SpriteResourceKey { get; }
    public string Text { get; }

    public DemoData(string spriteResourceKey, string text)
    {
        SpriteResourceKey = spriteResourceKey;
        Text = text;
    }
}


public class DemoCarouselCell : CarouselCell<DemoData, DemoCarouselCell>
{
    [SerializeField] private Image _image = default;
    [SerializeField] private Text _text = default;

    protected override void Refresh(DemoData data)
    {
        _image.sprite = Resources.Load<Sprite>(data.SpriteResourceKey);
        _text.text = data.Text;
    }
}
public class DemoCarouselView : CarouselView<DemoData, DemoCarouselCell>
{
}
namespace Demo.Scripts
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private DemoCarouselView _carouselView = default;
        [SerializeField, Range(1, 3)] private int _bannerCount = 3;

        private void Start()
        {
            var items = Enumerable.Range(0, _bannerCount)
                .Select(i =>
                {
                    var spriteResourceKey = $"tex_demo_banner_{i:D2}";
                    var text = $"Demo Banner {i:D2}";
                    return new DemoData(spriteResourceKey, text);
                })
                .ToArray();
            _carouselView.Setup(items);
        }
    }
}
public class ImageDisplay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
