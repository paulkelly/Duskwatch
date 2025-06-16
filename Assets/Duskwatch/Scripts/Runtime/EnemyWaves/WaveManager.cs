using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private EnemyWaveConfig levelWaveConfig;
    private HashSet<SpawnLocation> spawnLocations = new HashSet<SpawnLocation>();
    private HashSet<WaveEnemy> livingEnemies = new HashSet<WaveEnemy>();
    private int _currentWave = 0;
    
    public int EnemyCount => livingEnemies.Count;

    public void RegisterSpawnLocation(SpawnLocation spawnLocation)
    {
        spawnLocations.Add(spawnLocation);
        spawnLocation.SetNextWave(levelWaveConfig.GetWave(_currentWave));
    }

    public void DeregisterSpawnLocation(SpawnLocation spawnLocation)
    {
        spawnLocations.Remove(spawnLocation);
    }

    public void RegisterWaveEnemy(WaveEnemy waveEnemy)
    {
        livingEnemies.Add(waveEnemy);
    }

    public void WaveEnemyKilled(WaveEnemy waveEnemy)
    {
        livingEnemies.Remove(waveEnemy);
    }
    
    private void OnEnable()
    {
        TimeManager.OnNightTime += NightTime;
        TimeManager.OnDayTime += DayTime;
    }

    private void OnDisable()
    {
        TimeManager.OnNightTime -= NightTime;
        TimeManager.OnDayTime -= DayTime;
    }
    
    private void DayTime()
    {
        _currentWave++;
        foreach (var spawnLocation in spawnLocations)
        {
            spawnLocation.SetNextWave(levelWaveConfig.GetWave(_currentWave));
        }
    }

    private void NightTime()
    {
    }
}
