using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TorchControl : MonoBehaviour
{
    Light2D torchLight;
    int intervalBetweenRelight = 20, nextMinuteRefill = 0;    
    bool isLightDimmed = false;
    Torch torch;

    void Start()
    {
        torchLight = transform.GetChild(0).GetComponent<Light2D>();
        torch = GetComponent<Torch>();
    }

    // Update is called once per frame
    void Update()
    {
        // only dimmed once
        if (isLightDimmed)
        {
        
            // time to relight
            if (TimeManage.instance.currentMinute == nextMinuteRefill)
            {
                torchLight.intensity = 1f;
                isLightDimmed = false;
                torch.sabotaged = false;
                nextMinuteRefill+= intervalBetweenRelight;
                if (nextMinuteRefill>=60){
                    nextMinuteRefill -= 60;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Dim the light
            torchLight.intensity = 0.1f;
            isLightDimmed = true;
            torch.sabotaged = true;
        }
    }
}
