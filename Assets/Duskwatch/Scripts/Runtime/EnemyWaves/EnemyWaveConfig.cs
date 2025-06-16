using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveConfig", menuName = "Scriptable Objects/EnemyWaveConfig")]
public class EnemyWaveConfig : ScriptableObject
{
    [SerializeField] private List<EnemyWaveDefinition> waveDefinitions;

    public EnemyWaveDefinition GetWave(int level)
    {
        if (level < waveDefinitions.Count)
        {
            return waveDefinitions[level];
        }

        return new EnemyWaveDefinition();
    }
}

[Serializable]
public struct EnemyWaveDefinition
{
    [TableList] public List<EnemySpawn> enemySpawns;
}

[Serializable]
public struct EnemySpawn
{
    [TableColumnWidth(50, Resizable = false)] public int loc;
    public EnemyGroup enemies;
}

[Serializable]
public struct EnemyGroup
{
    [HorizontalGroup, HideLabel, AssetsOnly] public GameObject enemyPrefab;
    [HorizontalGroup, HideLabel] public int count;
}