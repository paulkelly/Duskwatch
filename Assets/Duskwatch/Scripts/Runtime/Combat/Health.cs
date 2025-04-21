using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    [FormerlySerializedAs("_reactions")] [SerializeField] private List<AbstractHitReaction> _hitReactions;
    [SerializeField] private List<AbstractDestroyReaction> _destroyReactions;
    
    private int _currentHealth;
    private bool _alive;

    public bool CanBeHit => _alive;

    public void Damage(DamageType damageType, int damage, Vector3 position)
    {
        if(!_alive) return;
        damage = UpdateHealth(_currentHealth - damage);
        
        foreach (var hitReaction in _hitReactions)
        {
            hitReaction.OnHit(damageType, damage, position);
        }

        if (_currentHealth <= 0)
        {
            foreach (var destroyReaction in _destroyReactions)
            {
                destroyReaction.OnDestroyed();
            }

            _alive = false;
        }
    }

    private int UpdateHealth(int newValue)
    {
        int oldValue = _currentHealth;
        _currentHealth = Mathf.Clamp(newValue, 0, _maxHealth);
        return oldValue - _currentHealth;
    }

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
        _alive = true;
    }

    private void Reset()
    {
        FindReactions();
    }
    
    public void RegisterHitReaction(AbstractHitReaction hitReaction)
    {
        _hitReactions.Add(hitReaction);
    }
    
    public void RegisterDestroyReaction(AbstractDestroyReaction hitReaction)
    {
        _destroyReactions.Add(hitReaction);
    }

    [Button]
    private void FindReactions()
    {
        if(_hitReactions == null) _hitReactions = new List<AbstractHitReaction>();
        if(_destroyReactions == null) _destroyReactions = new List<AbstractDestroyReaction>();

        _hitReactions.Clear();
        _destroyReactions.Clear();
        
        _hitReactions.AddRange(GetComponentsInChildren<AbstractHitReaction>());
        _destroyReactions.AddRange(GetComponentsInChildren<AbstractDestroyReaction>());
    }
}
