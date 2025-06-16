using System;
using UnityEngine;

public class WaveEnemy : MonoBehaviour, IDestroyReaction
{
    private void OnEnable()
    {
        var health = GetComponent<Health>();
        if(!health) return;
        
        health.AddReaction(this);
        SceneReferences.Instance.WaveManager.RegisterWaveEnemy(this);
    }

    public void OnDestroyed()
    {
        SceneReferences.Instance.WaveManager.WaveEnemyKilled(this);
    }

    public void OnResurrect()
    {
    }
}
