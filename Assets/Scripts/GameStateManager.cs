using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public int totalPeons;
    private bool gameOver;
    private bool stinger;

    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        stinger = false;
    }

    // Update is called once per frame
    void Update()
    {
        totalPeons = GameObject.FindGameObjectsWithTag("PlayerPeon").Length;

        if (totalPeons <= 2 && stinger == false)
        {
            stinger = true;
            FindObjectOfType<SoundManager>().PlaySound("LowUnitsStinger");
        }

        if (totalPeons > 2)
        {
            stinger = false;
        }

        if (totalPeons <= 0 && gameOver == false)
        {
            FindObjectOfType<SoundManager>().PlaySound("GameOverTheme");
            FindObjectOfType<SoundManager>().StopSound("MainTheme");
            gameOver = true;
        }
    }
}
