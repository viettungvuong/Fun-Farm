using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu; // Assign in inspector
    public GameObject confirmMenu; // Assign in inspector
    private bool isShowing;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC click");
            isShowing = !isShowing;
            pauseMenu.SetActive(isShowing);
        }
    }

    public void Continue()
    {
        Debug.Log("Continue");
        pauseMenu.SetActive(false);
    }

    public void Exit()
    {
        Debug.Log("Exit");
        confirmMenu.SetActive(true);
    }

    public void Save()
    {
        Debug.Log("Save");
    }
}
