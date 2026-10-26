using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Drop on any Button to make it load the GamesHub main menu.
[RequireComponent(typeof(Button))]
public class ReturnToMenuButton : MonoBehaviour
{
    // Full path, because PriceIsNice has its own (unused) MainMenu scene
    public string menuScenePath = "Assets/MainMenu/MainMenu.unity";

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(ReturnToMenu);
    }

    public void ReturnToMenu()
    {
        // Some games pause on game over
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScenePath);
    }
}
