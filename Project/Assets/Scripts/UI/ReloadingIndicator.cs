using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadingIndicator : MonoBehaviour
{
    private GameObject player;
    private Camera cam;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIndicatorPosition();
    }

    private void UpdateIndicatorPosition()
    {

        if (GameController.HomeScene()==false){
            return;
        }

        Vector3 offset = new Vector3(0f, 1f);

        Vector3 screenPosition = cam.WorldToScreenPoint(player.transform.position + offset); // change to screen position
        transform.position = screenPosition;
    }
}
