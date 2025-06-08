using System;
using System.Collections.Generic;
using DataBinding;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Bindable]
public class Health : MonoBehaviour
{
    public BindableInt maxHealth;
    [NonSerialized] public BindableInt currentHealth = new BindableInt(0);
    [NonSerialized] public BindableBool alive = new BindableBool(true);

    [SerializeField] private bool _immuneToDamage;
    [SerializeField] private List<IHitReaction> _hitReactions;
    [SerializeField] private List<IDestroyReaction> _destroyReactions;

    public void Resurrect()
    {
        Resurrect(maxHealth);
    }

    public void Resurrect(int health)
    {
        if (health <= 0) return;

        UpdateHealth(health);
        alive.SetValue(true);
        foreach (var destroyReaction in _destroyReactions)
        {
            destroyReaction.OnResurrect();
        }
    }

    public void Damage(DamageType damageType, int damage, bool isPlayer, Vector3 position)
    {
        if (!alive) return;

        if (!_immuneToDamage)
        {
            damage = UpdateHealth(currentHealth - damage);
        }

        foreach (var hitReaction in _hitReactions)
        {
            if (hitReaction.PlayerOnly && !isPlayer) continue;
            hitReaction.OnHit(damageType, damage, position);
        }

        if (currentHealth <= 0 && !_immuneToDamage)
        {
            foreach (var destroyReaction in _destroyReactions)
            {
                destroyReaction.OnDestroyed();
            }

            alive.SetValue(false);
        }
    }

    public void Heal(int health)
    {
        if (!alive) return;

        UpdateHealth(currentHealth + health);
    }

    private int UpdateHealth(int newValue)
    {
        int oldValue = currentHealth;
        currentHealth.SetValue(Mathf.Clamp(newValue, 0, maxHealth));
        return oldValue - currentHealth;
    }

    private void OnEnable()
    {
        FindReactions();
        
        currentHealth.SetValue(maxHealth);
        alive.SetValue(true);
    }
    
    private void FindReactions()
    {
        if (_hitReactions == null) _hitReactions = new List<IHitReaction>();
        if (_destroyReactions == null) _destroyReactions = new List<IDestroyReaction>();

        _hitReactions.Clear();
        _destroyReactions.Clear();

        _hitReactions.AddRange(GetComponentsInChildren<IHitReaction>(true));
        _destroyReactions.AddRange(GetComponentsInChildren<IDestroyReaction>(true));
    }
}
