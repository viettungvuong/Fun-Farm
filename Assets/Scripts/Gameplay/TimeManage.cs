using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TimeManage : MonoBehaviour
{
    public int currentHour = 6, currentMinute = 0;
    public float updateIntervalSeconds;
    public Light2D globalLight;

    private DateTime lastUpdated;

    void Awake()
    {
        lastUpdated = DateTime.Now;
        UpdateLight();
    }

    // Update is called once per frame
    void Update()
    {
        double secondsDifference = (DateTime.Now - lastUpdated).TotalSeconds;
        if (secondsDifference >= updateIntervalSeconds)
        {
            lastUpdated = DateTime.Now;
            currentMinute += 1;
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour += 1;
                if (currentHour >= 24)
                {
                    currentHour = 0;
                }
            }
        }
        UpdateLight();
    }

    void UpdateLight()
    {
        float maxIntensity = 2.0f;
        float minIntensity = 1.0f;
        Color dayColor = Color.white;
        Color nightColor = new Color(0.1f, 0.1f, 0.35f); // dark blue

        if (currentHour >= 6 && currentHour <= 18)
        {
            Debug.Log("Day time");
            // Day time
            globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, (currentHour - 6) / 12f);
            globalLight.color = dayColor;
        }
        else
        {
            Debug.Log("Night time");
            // Night time
            if (currentHour > 18)
            {
                globalLight.intensity = Mathf.Lerp(maxIntensity, minIntensity, (currentHour - 18) / 12f);
                globalLight.color = nightColor;
            }
            else // midnight
            {
                globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, (currentHour + 6) / 12f);
                globalLight.color = nightColor;
            }
        }
    }
}
