using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    public static bool ManualOpen = false;
    public GameObject canvas;
    public Camera cam;
    void Start()
    {
        CheckFirstTimeOpen();
    }

    void CheckFirstTimeOpen()
    {
        // check if the game has been opened before
        if (PlayerPrefs.HasKey("GameOpened")&&ManualOpen==false)
        {
            SceneManager.LoadScene("SceneWelcome");
        }
        else
        {
            //PlayerPrefs.SetInt("GameOpened", 1);
            //PlayerPrefs.Save();
            canvas.SetActive(true);
            cam.enabled = true;

        }
        ManualOpen = true;
    }
}