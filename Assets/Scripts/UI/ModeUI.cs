using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeUI : MonoBehaviour
{
    public static ModeUI Instance { get; private set; }

    public RectTransform infoPanel;
    public GameObject fence, health, enemy;

    const float maxWidth = 1500;
    const float minWidth = 800;



    public void Update()
    {

        if (PlayerUnit.playerMode == PlayerMode.SURVIVAL)
        {
            fence.SetActive(true);
            health.SetActive(true);
            enemy.SetActive(true);
            infoPanel.sizeDelta = new Vector2(maxWidth, infoPanel.sizeDelta.y);
        }
        else
        {
            fence.SetActive(false);
            health.SetActive(false);
            enemy.SetActive(false);
            infoPanel.sizeDelta = new Vector2(minWidth, infoPanel.sizeDelta.y);
        }
    }
}
        