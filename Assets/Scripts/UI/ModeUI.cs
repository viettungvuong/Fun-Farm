using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeUI : MonoBehaviour
{
    public RectTransform infoPanel;
    public GameObject fence, health, enemy;

    const float maxWidth = 1500;
    const float minWidth = 800;

    void Update()
    {
        if (PlayerUnit.playerMode == PlayerMode.SURVIVAL){
            fence.SetActive(true);
            health.SetActive(true);
            enemy.SetActive(true);

            infoPanel.sizeDelta = new Vector2(maxWidth, infoPanel.sizeDelta.y);
        }
        else{
            fence.SetActive(false);
            health.SetActive(false);
            enemy.SetActive(false);

            infoPanel.sizeDelta = new Vector2(minWidth, infoPanel.sizeDelta.y);
        }
    }

    // void Start()
    // {
    //     if (PlayerUnit.playerMode == PlayerMode.SURVIVAL){
    //         fence.SetActive(true);
    //         health.SetActive(true);
    //         enemy.SetActive(true);
    //     }
    //     else{
    //         fence.SetActive(false);
    //         health.SetActive(false);
    //         enemy.SetActive(false);
    //     }
    // }

}
