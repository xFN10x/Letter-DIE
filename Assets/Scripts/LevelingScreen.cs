using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LevelStat
{
    Robustness = 0,
    Endurance = 1,
    Dexterity = 2,
    Grip = 3,
    Strengh = 4
}
public class LevelingScreen : MonoBehaviour
{
    public GameHandler Game;
    public PlayerController Player;

    public TextMeshProUGUI JokeNameGUI;
    public TextMeshProUGUI JokeDateGUI;

    public TextMeshProUGUI MainBody;

    public TextMeshProUGUI RobustGFRequirement;
    public TextMeshProUGUI EndurGFRequirement;
    public TextMeshProUGUI DexGFRequirement;
    public TextMeshProUGUI GripGFRequirement;
    public TextMeshProUGUI StrGFRequirement;

    public int robustGFRequirement = 50;
    public int endurGFRequirement = 50;
    public int dexGFRequirement = 50;
    public int gripGFRequirement = 100;
    public int strGFRequirement = 50;

    public Button RobustLevelButton;
    public Button EndurLevelButton;
    public Button DexLevelButton;
    public Button GripLevelButton;
    public Button StrLevelButton;


    void Start()
    {
        JokeNameGUI.SetText($"Name: {Environment.UserName}");
        JokeDateGUI.SetText($"Date: {DateTime.Now:yyyy/MM/dd}");

        Refresh();
    }

    public void Level(int stat)
    {
        switch (stat)
        {
            case 0:
                if (Game.Graphite >= robustGFRequirement)
                {
                    Game.Graphite -= robustGFRequirement;
                    robustGFRequirement += 50;
                    Player.Robustness += 1;
                }
                break;
            case 1:
                if (Game.Graphite >= endurGFRequirement)
                {
                    Game.Graphite -= endurGFRequirement;
                    endurGFRequirement += 50;
                    Player.Endurance += 1;
                }
                break;
            case 2:
                if (Game.Graphite >= dexGFRequirement)
                {
                    Game.Graphite -= dexGFRequirement;
                    dexGFRequirement += 50;
                    Player.Dexterity += 1;
                }
                break;
            case 3:
                if (Game.Graphite >= gripGFRequirement)
                {
                    Game.Graphite -= gripGFRequirement;
                    gripGFRequirement += 100;
                    Player.Grip += 1;
                }
                break;
            case 4:
                if (Game.Graphite >= strGFRequirement)
                {
                    Game.Graphite -= strGFRequirement;
                    robustGFRequirement += 50;
                    Player.Strengh += 1;
                }
                break;
            default:
                break;
        }

    }

    public void Refresh()
    {
        RobustGFRequirement.SetText($"{robustGFRequirement} GF");
        EndurGFRequirement.SetText($"{endurGFRequirement} GF");
        DexGFRequirement.SetText($"{dexGFRequirement} GF");
        GripGFRequirement.SetText($"{gripGFRequirement} GF");
        StrGFRequirement.SetText($"{strGFRequirement} GF");

        MainBody.SetText($"Robustness: {Player.Robustness}\n" +
            $"Endurance: {Player.Endurance}\n" +
            $"Dexterity: {Player.Dexterity}\n" +
            $"Grip: {Player.Grip}\n" +
            $"Strengh: {Player.Strengh}");
    }
}
