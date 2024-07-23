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
        timeData.weatherType = Weather.instance.currentWeather;


        return timeData;
    }

    public void Reload(TimeData timeData)
    {
        currentHour = timeData.currentHour;
        currentMinute = timeData.currentMinute;
        currentDay = timeData.currentDay;

        Weather.instance.currentWeather = timeData.weatherType;
        Weather.instance.UpdateWeather();

        string minString = currentMinute < 10 ? "0" + currentMinute.ToString() : currentMinute.ToString();
        string hourString = currentHour < 10 ? "0" + currentHour.ToString() : currentHour.ToString();
        if (timeText != null)
        {
            timeText.text = hourString + ":" + minString;
        }

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeLight();
    }

    private void InitializeLight()
    {
        globalLight = GameObject.Find("Global Light 2D").GetComponent<Light2D>();
    }

    void Awake()
    {
        DontDestroyOnLoadManager.DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeLight();

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

            if (Weather.instance.currentWeather == WeatherType.Rainy)
            {
                Weather.rainDuration++;
            }

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
            string minString = currentMinute < 10 ? "0" + currentMinute.ToString() : currentMinute.ToString();
            string hourString = currentHour < 10 ? "0" + currentHour.ToString() : currentHour.ToString();
            if (timeText != null)
            {
                timeText.text = hourString + ":" + minString;
            }

            if (IsDay()&&Weather.instance.currentWeather!=WeatherType.Rainy){
                UpdateLight();
            }

        }
    }

    public bool IsDay()
    {
        return currentHour >= 6 && currentHour < 19;
    }

    void UpdateLight()
    {
        float maxIntensityDay = 1.5f;
        float maxIntensityNight = 0.65f;
        float minIntensityDay = 0.8f;
        float minIntensityNight = 0.1f;

        Color dayColor = Color.white;
        Color nightColor = new Color(0.1f, 0.1f, 0.35f); // dark blue

        // Dawn (5 AM to 6 AM)
        if (currentHour >= 5 && currentHour < 6)
        {
            float t = (currentHour - 5f) / 1f;
            globalLight.intensity = Mathf.Lerp(minIntensityNight, minIntensityDay, t);
            globalLight.color = Color.Lerp(nightColor, dayColor, t);
        }
        // Day time (6 AM to 4 PM)
        else if (currentHour >= 6 && currentHour < 16)
        {
            if (currentHour < 12) // Morning transition from 6 AM to 12 PM
            {
                float t = (currentHour - 6f) / 6f;
                globalLight.intensity = Mathf.Lerp(minIntensityDay, maxIntensityDay, t);
            }
            else // Afternoon transition from 12 PM to 4 PM
            {
                float t = (currentHour - 12f) / 4f;
                globalLight.intensity = Mathf.Lerp(maxIntensityDay, minIntensityDay, t);
            }
            globalLight.color = dayColor;
        }
        // Evening (4 PM to 8 PM)
        else if (currentHour >= 16 && currentHour < 20)
        {
            float t = (currentHour - 16f) / 4f;
            globalLight.intensity = Mathf.Lerp(minIntensityDay, maxIntensityNight, t);
            globalLight.color = Color.Lerp(dayColor, nightColor, t);
        }
        // Night time (8 PM to 5 AM)
        else
        {
            if (currentHour >= 20)
            {
                float t = (currentHour - 20f) / 4f;
                globalLight.intensity = Mathf.Lerp(maxIntensityNight, minIntensityNight, t);
            }
            else
            {
                float t = (currentHour + 4f) / 5f; // From midnight to dawn
                globalLight.intensity = Mathf.Lerp(minIntensityNight, maxIntensityNight, t);
            }
            globalLight.color = nightColor;
        }
    }
}