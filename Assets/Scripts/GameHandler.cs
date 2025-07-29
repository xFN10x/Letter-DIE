using System.Collections.Concurrent;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Menu
{
    Normal,
    Leveling
}
public class GameHandler : MonoBehaviour
{
    public bool Paused;
    public Menu CurrentMenu;

    public PlayerController Player;

    public GameObject PlatformPrefab;

    public int Graphite;
    public TextMeshProUGUI GraphiteCounter;

    public CanvasGroup LevelingMenu;
    private GameObject levelingMenuGO;
    private RectTransform levelingMenuRect;

    public float CurrentDistence;
    public float LoadingTriggerX;
    public float NextSceneLoadPosX;

    private static float SceneGap = 110f;

    public ConcurrentQueue<float> SceneLoadedPoses;

    public void GiveGraphite(int amount)
    {
        Graphite += amount;
    }
    private void Start()
    {
        SceneLoadedPoses = new();

        levelingMenuGO = LevelingMenu.gameObject;
        levelingMenuRect = levelingMenuGO.GetComponent<RectTransform>();

        GraphiteCounter.SetText($"Graphite: {Graphite}");
        LoadNext();
        LoadNext();
        LoadingTriggerX = SceneGap;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(LoadingTriggerX, 9999), new Vector3(LoadingTriggerX, -9999));
    }

    public float SnapToGrid(float input)
    {
        return math.round(input / 10) * 10;
    }

    public void LoadNext()
    {
        AsyncOperation AO = SceneManager.LoadSceneAsync("LevelPart_Start", LoadSceneMode.Additive);

        AO.completed += (v) =>
        {
            SceneLoadedPoses.TryPeek(out float x);

            GameObject.Find("Root").transform.position = new Vector3(x, 0);
            GameObject.Find("Root").name = "Loaded!";

            SceneLoadedPoses.TryDequeue(out _);
        };
        SceneLoadedPoses.Enqueue(NextSceneLoadPosX);

        NextSceneLoadPosX += SceneGap;

        LoadingTriggerX += SceneGap;
    }

    public void ExitLevelingMenu()
    {
        CurrentMenu = Menu.Normal;
    }
    private void Update()
    {
        if (CurrentDistence != Player.gameObject.transform.position.x)
        {
            CurrentDistence = Player.gameObject.transform.position.x;
            if (CurrentDistence > LoadingTriggerX)
            {
                LoadNext();
            }
        }
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
