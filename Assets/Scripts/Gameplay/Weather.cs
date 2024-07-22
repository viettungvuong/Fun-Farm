using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum WeatherType
{
    Sunny,
    Rainy
}
public class Weather : MonoBehaviour
{

    public WeatherType currentWeather;
    public ParticleSystem rainParticleSystem;
    private static bool toggled = false;
    public static Weather instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        UpdateWeather();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWeather();
        if (TimeManage.instance.currentMinute == 30 && toggled == false){
            ToggleWeather();
            toggled = true;
        }
        else if (TimeManage.instance.currentMinute != 30){
            toggled = false;
        }
    }

    public void UpdateWeather()
    {
        if (currentWeather == WeatherType.Rainy)
        {
            rainParticleSystem.Play();
        }
        else
        {
            rainParticleSystem.Stop();
        }
    }

    void ToggleWeather()
    {
        currentWeather = (Random.value > 0.7f) ? WeatherType.Rainy : WeatherType.Sunny;
        UpdateWeather();
    }
}