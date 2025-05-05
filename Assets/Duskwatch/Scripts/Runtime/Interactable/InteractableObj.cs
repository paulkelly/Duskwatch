using System;
using System.Collections.Generic;
using DataBinding;
using UnityEngine;

[Bindable]
public abstract class InteractableObj : MonoBehaviour, IInteractable
{
    public BindableString displayString = new BindableString(string.Empty);
    public BindableTransform displayPosition;
    public BindableFloat normalisedProgress = new BindableFloat(0);
    public BindableBool interacting = new BindableBool(false);
    public BindableBool showProgress = new BindableBool(false);

    private HashSet<DuskwatchAgent> workers = new HashSet<DuskwatchAgent>();

    public virtual Vector3 GetClosestPosition(Vector3 fromPosition) => Position;
    
    public virtual float InteractTime => 2f;
    public float RemainingTime => InteractTime-Progress;
    public float Progress { get; private set; }
    public bool IsComplete => normalisedProgress >= 1;
    public bool Interacting
    {
        get => interacting.GetValue();
        set
        {
            if(interacting == value) return;
            interacting.SetValue(value);
            if (interacting)
            {
                OnInteractStart();
            }
            else
            {
                OnInteractEnd();
            }
        }
    }

    public void AddWorker(DuskwatchAgent worker)
    {
        workers.Add(worker);
        showProgress.SetValue(_isClosest || workers.Count > 0);
    }

    public void RemoveWorker(DuskwatchAgent worker)
    {
        workers.Remove(worker);
        showProgress.SetValue(_isClosest || workers.Count > 0);
    }

    private bool _isClosest;
    public bool IsClosest
    {
        get => _isClosest;
        set
        {
            _isClosest = value;
            showProgress.SetValue(_isClosest || workers.Count > 0);
        }
    }
    public Vector3 Position { get; private set; }
    public virtual void Interact() { }
    public virtual void OnProgressUpdated() { }

    protected virtual void OnInteractStart() { }

    protected virtual void OnInteractEnd() { }

    public void UpdateProgress(float time)
    {
        Progress += time;
        float normalProgress = Progress / InteractTime;
        normalisedProgress.SetValue(Mathf.Clamp01(normalProgress));
        OnProgressUpdated();
        if (normalProgress >= 1)
        {
            Interact();
        }
    }

    private void OnEnable()
    {
        Position = transform.position;
        if (displayPosition.GetValue() == null)
        {
            displayPosition.SetValue(transform);
        }
        
        UIReferences.Instance.InteractableTags.DisplayTag(this);
    }

    private void OnDisable()
    {
        UIReferences.Instance.InteractableTags.HideTag(this);
    }
}
