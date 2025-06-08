using System;
using System.Collections.Generic;
using AudioSystem;
using PrimeTween;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private Transform sun;
    [SerializeField] private Light sunLight;
    [SerializeField] private Light moonLight;
    [SerializeField] private List<TimeOfDaySetting> timeOfDaySettings;
    [SerializeField] private AnimationCurve dayCycleCurve;

    [Header("Day Night Transition")] 
    [SerializeField] private float transitionTimeOut = 3f;
    [SerializeField] private float transitionTimeIn = 3f;
    [SerializeField] private Vector3 eveningSunAngle;
    [SerializeField] private SoundData dayTransitionSound;
    [SerializeField] private SoundData nightTransitionSound;
    
    [Range(0,1)] public float timeOfDay;

    public bool isDay => _isDay;
    public bool isTransitioning => _isTransitioning;
    public float transitionTime => transitionTimeIn+transitionTimeOut;

    private const float SunIntensityNight = 0;
    private const float MoonIntensityDay = 0.2f;
    
    private bool _isTransitioning = false;
    private bool _isDay = true;
    private float _sunIntensity;
    private float _moonIntensity;
    private Vector3 _sunPosition;
    
    public void SetToDaytime()
    {
        _isDay = true;

        Tween.LightIntensity(sunLight, _sunIntensity, transitionTimeIn, startDelay: transitionTimeOut);
        Tween.LightIntensity(moonLight, MoonIntensityDay, transitionTimeOut);
        
        dayTransitionSound.Play();
    }
    
    public void SetToNighttime()
    {
        _isDay = false;
        _isTransitioning = true;
        
        Tween.LightIntensity(sunLight, SunIntensityNight, transitionTimeOut);
        Tween.LightIntensity(moonLight, _moonIntensity, transitionTimeIn, startDelay: transitionTimeOut);
        Tween.LocalRotation(sun, _sunPosition, eveningSunAngle, transitionTimeOut);
        Tween.Delay(transitionTime).OnComplete(FinishTransition);
        
        nightTransitionSound.Play();
    }

    private void FinishTransition()
    {
        _isTransitioning = false;
    }

    private void Start()
    {
        _sunIntensity = sunLight.intensity;
        _moonIntensity = moonLight.intensity;

        _isDay = true;
        moonLight.intensity = MoonIntensityDay;
    }
    
    private void Update()
    {
        _sunPosition = timeOfDaySettings[0].sunAngle;
        Color sunColour = timeOfDaySettings[0].colour;
        float previousTime = 0;
        float currentTime = dayCycleCurve.Evaluate(timeOfDay);
        for (int i = 0; i < timeOfDaySettings.Count; i++)
        {
            if (timeOfDaySettings[i].timeOfDay < currentTime)
            {
                _sunPosition = timeOfDaySettings[i].sunAngle;
                sunColour = timeOfDaySettings[i].colour;
                previousTime = timeOfDaySettings[i].timeOfDay;
                continue;
            }

            float lerpValue = Mathf.InverseLerp(previousTime, timeOfDaySettings[i].timeOfDay, currentTime);
            _sunPosition = Vector3.Lerp(_sunPosition, timeOfDaySettings[i].sunAngle, lerpValue);
            sunColour = Color.Lerp(sunColour, timeOfDaySettings[i].colour, lerpValue);
            
            break;
        }

        if (_isTransitioning) return;
        
        sun.localEulerAngles = _sunPosition;
        sunLight.color = sunColour;
    }
    
}

[Serializable]
public struct TimeOfDaySetting
{
    public float timeOfDay;
    public Vector3 sunAngle;
    public Color colour;
}
