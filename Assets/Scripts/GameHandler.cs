using DG.Tweening;
using System.Collections;
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

    public GameObject PencilEraserTipPrefab;
    public GameObject PencilDrawTipPrefab;

    public int Graphite;
    public TextMeshProUGUI GraphiteCounter;

    public CanvasGroup LevelingMenu;
    private GameObject levelingMenuGO;
    private RectTransform levelingMenuRect;

    public float CurrentDistence;
    public float LoadingTriggerX;
    public float NextSceneLoadPosX;

    private int NextPencilMovement = 1;

    private static readonly float SceneGap = 110f;

    public ConcurrentQueue<float> SceneLoadedPoses;

    public void PencilEraseEffect(Vector2 Pos, GameObject Subject, SpriteRenderer Subject2)
    {
        StartCoroutine(PencilErase(Pos, Subject, Subject2));
    }

    protected IEnumerator PencilErase(Vector2 Pos, GameObject Subject, SpriteRenderer Subject2)
    {
        Rigidbody2D proposedRigid = Subject.GetComponent<Rigidbody2D>();
        if (proposedRigid)
        {
            proposedRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        GameObject currentClone = GameObject.Instantiate(PencilEraserTipPrefab);
        Vector3 transPos = new(Pos.x, Pos.y, -10);
        currentClone.transform.localPosition = (transPos - (Vector3.down * 100)) + (Vector3.right * 10);

        currentClone.transform.DORotate(new Vector3(0, 0, -170), 1f);
        yield return currentClone.transform.DOMove(transPos, 1f).WaitForCompletion();
        currentClone.GetComponent<AudioSource>().Play();
        while (Subject2.color.a > 0f)
        {

            if (!DOTween.IsTweening(Subject2))
                Subject2.DOColor(new Color(0, 0, 0, -0.01f), 2f);
            if (!DOTween.IsTweening(currentClone.transform))
                currentClone.transform.DOMove(currentClone.transform.position + (GetNextPencilMovement() == 1 ? Vector3.right : Vector3.left), 0.1f);
            yield return new WaitForFixedUpdate();
        }
        currentClone.GetComponent<AudioSource>().Stop();

        var targetEnd = currentClone.transform.position + (Vector3.down * 100);
        yield return currentClone.transform.DOMove(targetEnd, 1f).WaitForCompletion();

        Destroy(Subject);
        Destroy(currentClone);
    }

    public int GetNextPencilMovement()
    {
        var toReturn = NextPencilMovement;
        NextPencilMovement = (toReturn == 0 ? 1 : 0);

        return toReturn;
    }
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
            GameObject root = GameObject.Find("Root");
            root.transform.position = new Vector3(x, 0);
            LevelPart part = root.GetComponent<LevelPart>();
            part.gameHandler = this;
            part.player = Player;
            root.name = "Loaded!";

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
