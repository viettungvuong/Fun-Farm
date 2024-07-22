using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
public enum WeatherType
{
    Sunny,
    Rainy
}
public class Weather : MonoBehaviour
{
    private Light2D globalLight;
    public WeatherType currentWeather;
    public ParticleSystem rainParticleSystem;
    private static bool toggled = false;
    public static Weather instance;

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

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoadManager.DontDestroyOnLoad(rainParticleSystem.gameObject);
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
        UpdateWeather();

    }

    // Update is called once per frame
    void Update()
    {
        // UpdateWeather();
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

            if (TimeManage.instance.IsDay()){
                globalLight.intensity = 0.6f; // make sky darker
            }
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