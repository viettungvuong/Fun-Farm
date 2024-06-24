using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform player; 

    void LateUpdate()
    {
        if (player != null)
        {
            // rotate camera
            Vector3 newRotation = new Vector3(90f, player.eulerAngles.y, 0f);
            transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}
