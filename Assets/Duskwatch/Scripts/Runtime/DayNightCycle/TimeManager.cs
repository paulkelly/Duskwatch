using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float _dayLength;
    [SerializeField] private float _nightLength;
    [SerializeField] private DayNightCycle _dayNightCycle;

    public delegate void DayTime();
    public static DayTime OnDayTime;
    public delegate void NightTime();
    public static NightTime OnNightTime;
    
    
    public int day { get; private set; }
    public float time { get; private set; } = 10f;

    private void Update()
    {
        time += Time.deltaTime;

        if (_dayNightCycle.isTransitioning) return;

        if (_dayNightCycle.isDay)
        {
            _dayNightCycle.timeOfDay = Mathf.Clamp01(time / _dayLength);
            
            if (time < _dayLength) return;
            
            _dayNightCycle.SetToNighttime();
            OnNightTime?.Invoke();
            time = 0;
        }
        else
        {
            if (time < _nightLength) return;
            if (SceneReferences.Instance.WaveManager.EnemyCount > 0) return;
            
            _dayNightCycle.SetToDaytime();
            OnDayTime?.Invoke();

            time = 0;
            day++;
        }
    }
}
