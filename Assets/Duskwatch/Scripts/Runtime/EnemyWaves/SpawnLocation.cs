using System;
using System.Collections.Generic;
using DataBinding;
using UnityEngine;
using Random = UnityEngine.Random;

[Bindable]
public class SpawnLocation : MonoBehaviour
{
    [SerializeField] private int locationID;
    [SerializeField] private float spawnRadius = 5f;
    
    public BindableBool visile;
    public BindableInt enemyCount;
    public BindableTransform location;

    public Vector3 Position => transform.position;

    private List<EnemyGroup> toSpawn = new List<EnemyGroup>();
    
    public void SetNextWave(EnemyWaveDefinition wave)
    {
        int count = 0;
        toSpawn.Clear();
        foreach (var spawn in wave.enemySpawns)
        {
            if (spawn.loc != locationID) continue;
            toSpawn.Add(spawn.enemies);
            count += spawn.enemies.count;
        }
        enemyCount.SetValue(count);
        visile.SetValue(true);
    }

    public void SpawnWave()
    {
        Vector3 spawnPos = Position;
        foreach (var group in toSpawn)
        {
            for (int i = 0; i < group.count; i++)
            {
                Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
                var obj = Instantiate(group.enemyPrefab, spawnPos + new Vector3(randomPos.x, 0, randomPos.y), Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up));
                obj.AddComponent<WaveEnemy>();
            }
        }
    }

    private void OnEnable()
    {
        TimeManager.OnNightTime += NightTime;
        SceneReferences.Instance.WaveManager.RegisterSpawnLocation(this);
    }

    private void OnDisable()
    {
        TimeManager.OnNightTime -= NightTime;
        SceneReferences.Instance.WaveManager.DeregisterSpawnLocation(this);
    }
    

    private void NightTime()
    {
        visile.SetValue(false);
        SpawnWave();
    }
}
