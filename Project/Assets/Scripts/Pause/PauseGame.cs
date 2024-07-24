using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenu; // Assign in inspector
    private bool isShowing;

    void Update()
    {
        if (GameController.HomeScene() == false)
        {
            return;
        }
        isShowing = pauseMenu.activeSelf;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC click");
            Time.timeScale = 0;
            isShowing = !isShowing;
            pauseMenu.SetActive(isShowing);
        }
    }
}
