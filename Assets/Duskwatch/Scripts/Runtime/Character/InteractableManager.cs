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

    public void Interact()
    {
        if(_closestInteractable == null) return;
        
        _closestInteractable.Interact();
    }

    private void Update()
    {
        PopulateNearbyInteractables();

        if (_interactables.Count < 1)
        {
            _closestInteractable = null;
            return;
        }

        Vector3 pos = transform.position;
        float best = Mathf.Infinity;
        foreach (var interactable in _interactables)
        {
            float distance = Vector3.Distance(pos, interactable.Position);
            if(distance > best) continue;
            _closestInteractable = interactable;
            best = distance;
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
