using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConfirmMenu : MonoBehaviour
{
    public GameObject confirmMenu;

    public void YesClick()
    {
        // SceneManager.LoadSceneAsync(0); //Open Welcome Menu
        GameController.OpenMenu();
    }

    public void NoClick()
    {
        Time.timeScale = 1;
        confirmMenu.SetActive(false);
    }
}
