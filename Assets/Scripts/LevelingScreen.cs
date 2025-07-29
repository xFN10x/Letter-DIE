using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelingScreen : MonoBehaviour
{
    public PlayerController Player;

    public TextMeshProUGUI JokeNameGUI;
    public TextMeshProUGUI JokeDateGUI;

    public TextMeshProUGUI RobustGFRequirement;
    public TextMeshProUGUI EndurGFRequirement;
    public TextMeshProUGUI DexGFRequirement;
    public TextMeshProUGUI GripGFRequirement;
    public TextMeshProUGUI StrGFRequirement;

    public Button RobustLevelButton;
    public Button EndurLevelButton;
    public Button DexLevelButton;
    public Button GripLevelButton;
    public Button StrLevelButton;


    void Start()
    {
        JokeNameGUI.SetText($"Name: {Environment.UserName}");
        JokeDateGUI.SetText($"Date: {DateTime.Now:yyyy/MM/dd}");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
