using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenu; // Assign in inspector
    private bool isShowing;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC click");
            isShowing = !isShowing;
            pauseMenu.SetActive(isShowing);
        }
    }
}
