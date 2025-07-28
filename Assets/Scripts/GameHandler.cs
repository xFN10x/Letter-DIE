using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public bool Paused;

    private void Update()
    {
        if (Paused)
        {
            Time.timeScale = 0;
        }
        else if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
    }

}
