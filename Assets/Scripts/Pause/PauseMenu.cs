using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu; // Assign in inspector
    public GameObject confirmMenu; // Assign in inspector
   
    // Update is called once per frame

    public void Pause()
    {
        pauseMenu.SetActive(true);
    }
    public void Continue()
    {
        Debug.Log("Continue");
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        Debug.Log("Exit");
        confirmMenu.SetActive(true);
    }
}
