using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour
{
    [SerializeField] private LayerMask _interactionLayers;
    [SerializeField] private float _interactionRange;
    
    private HashSet<Collider> _nearbyColliders = new HashSet<Collider>();
    private HashSet<InteractableObj> _interactables = new HashSet<InteractableObj>();

    private InteractableObj _closestInteractable;
    
    private Collider[] _sphereCastResults = new Collider[10];
    private HashSet<Collider> _toRemove = new HashSet<Collider>();
    
    public bool InteractHeld { get; set; }

    private void LateUpdate()
    {
        if(_closestInteractable == null) return;
        
        _closestInteractable.Interacting = InteractHeld;
        if (InteractHeld)
        {
            _closestInteractable.UpdateProgress(Time.deltaTime);
        }
    }

    private void Update()
    {
        PopulateNearbyInteractables();

        if (_interactables.Count < 1)
        {
            SetClosestInteractable(null);
            return;
        }

        InteractableObj toSelect = null;
        Vector3 pos = transform.position;
        float best = Mathf.Infinity;
        foreach (var interactable in _interactables)
        {
            float distance = Vector3.Distance(pos, interactable.Position);
            if(distance > best) continue;
            toSelect = interactable;
            best = distance;
        }

        SetClosestInteractable(toSelect);
    }

    private void SetClosestInteractable(InteractableObj obj)
    {
        if(_closestInteractable == obj) return;

        if (_closestInteractable != null)
        {
            _closestInteractable.Interacting = false;
            _closestInteractable.IsClosest = false;
        }
        _closestInteractable = obj;
        if (_closestInteractable != null)
        {
            _closestInteractable.IsClosest = true;
        }
    }

    private void PopulateNearbyInteractables()
    {
        _toRemove.Clear();
        foreach (var col in _nearbyColliders)
        {
            _toRemove.Add(col);
        }
        
        int nearbyInteractables = Physics.OverlapSphereNonAlloc(transform.position, _interactionRange, _sphereCastResults, _interactionLayers);
        for (int i = 0; i < nearbyInteractables; i++)
        {
            if (_nearbyColliders.Add(_sphereCastResults[i]))
            {
                var interactable = _sphereCastResults[i].GetComponent<InteractableObj>();
                if(interactable == null) continue;
                
                _interactables.Add(interactable);
            }
            else
            {
                _toRemove.Remove(_sphereCastResults[i]);
            }
        }

        foreach (var col in _toRemove)
        {
            _nearbyColliders.Remove(col);
            var interactable = col.GetComponent<InteractableObj>();
            if(interactable == null) continue;
            
            _interactables.Remove(interactable);
        }
    }
}
