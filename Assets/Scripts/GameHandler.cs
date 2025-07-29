using TMPro;
using UnityEngine;

public enum Menu
{
    Normal,
    Leveling
}
public class GameHandler : MonoBehaviour
{
    public bool Paused;
    public Menu CurrentMenu;

    public int Graphite;
    public TextMeshProUGUI GraphiteCounter;

    public CanvasGroup LevelingMenu;
    private GameObject levelingMenuGO;
    private RectTransform levelingMenuRect;

    private void Start()
    {
        levelingMenuGO = LevelingMenu.gameObject;
        levelingMenuRect = levelingMenuGO.GetComponent<RectTransform>();

        GraphiteCounter.SetText($"Graphite: {Graphite}");
    }

    public void ExitLevelingMenu()
    {
        CurrentMenu = Menu.Normal;
    }
    private void Update()
    {
        if (Paused || CurrentMenu == Menu.Leveling)
        {
            Time.timeScale = 0;
        }
        else if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }

        if (GraphiteCounter.text != $"Graphite: {Graphite}")
            GraphiteCounter.SetText($"Graphite: {Graphite}");

        if (CurrentMenu == Menu.Leveling)
        {
            levelingMenuRect.anchoredPosition = Vector2.Lerp(levelingMenuRect.anchoredPosition, new Vector2(0, 0), 0.2f);
            LevelingMenu.interactable = true;
        }
        else if (levelingMenuRect.anchoredPosition != new Vector2(0, 1500))
        {
            if (LevelingMenu.interactable == true)
                LevelingMenu.interactable = false;
            levelingMenuRect.anchoredPosition = Vector2.Lerp(levelingMenuRect.anchoredPosition, new Vector2(0, 1500), 0.2f);
        }
    }

}
