using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TimeManage : MonoBehaviour
{
    public int currentHour = 6, currentMinute = 0, currentDay = 1;
    public float updateIntervalSeconds;
    private Light2D globalLight;

    private DateTime lastUpdated;

    public TextMeshProUGUI timeText, dayText;

    public static TimeManage instance;

    public TimeData Serialize()
    {
        TimeData timeData = new TimeData();
        timeData.currentHour = currentHour;
        timeData.currentMinute = currentMinute;
        timeData.currentDay = currentDay;


        return timeData;
    }

    public void Reload(TimeData timeData)
    {
        currentHour = timeData.currentHour;
        currentMinute = timeData.currentMinute;
        currentDay = timeData.currentDay;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeMap();
    }

    private void InitializeMap()
    {
        globalLight = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeMap();

        lastUpdated = DateTime.Now;
        UpdateLight();
    }



    void Update()
    {
        double secondsDifference = (DateTime.Now - lastUpdated).TotalSeconds;
        if (secondsDifference >= updateIntervalSeconds)
        {
            lastUpdated = DateTime.Now;
            currentMinute += 1;
            string minString = currentMinute < 10 ? "0" + currentMinute.ToString() : currentMinute.ToString();
            if (currentMinute >= 60)
            {
                currentMinute = 0;
                currentHour += 1;
                if (currentHour >= 24)
                {
                    currentHour = 0; // new day
                    currentDay++;
                    dayText.text = "Day " + currentDay.ToString();
                }
            }
            string hourString = currentHour < 10 ? "0" + currentHour.ToString() : currentHour.ToString();
            if (timeText != null)
            {
                timeText.text = hourString + ":" + minString;
            }
            UpdateLight();
        }
    }

    public bool IsDay()
    {
        return currentHour >= 6 && currentHour <= 18;
    }

    void UpdateLight()
    {
        float maxIntensityDay = 2.0f;
        float maxIntensityNight = 0.85f;
        float minIntensityDay = 1.0f;
        float minIntensityNight = 0.1f;
        Color dayColor = Color.white;
        Color nightColor = new Color(0.1f, 0.1f, 0.35f); // dark blue

        if (IsDay())
        {
            // Day time (from 6 AM to 6 PM)
            globalLight.intensity = Mathf.Lerp(minIntensityDay, maxIntensityDay, (currentHour - 6) / 12f);
            globalLight.color = dayColor;
        }
        else
        {
            // evening
            if (currentHour >= 18 && currentHour < 24)
            {
                globalLight.intensity = Mathf.Lerp(maxIntensityNight, minIntensityNight, (currentHour - 18) / 6f);
            }
            else
            {
                globalLight.intensity = Mathf.Lerp(minIntensityNight, maxIntensityNight, currentHour / 6f);
            }
            globalLight.color = nightColor;
        }
    }
}