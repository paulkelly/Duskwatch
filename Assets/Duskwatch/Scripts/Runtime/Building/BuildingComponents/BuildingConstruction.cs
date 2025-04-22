using System;
using AudioSystem;
using UnityEngine;

public class BuildingConstruction : MonoBehaviour, IBuildingPlacementFunctions
{
    private const float AudioStopDelay = 0.3f;
    
    [SerializeField] private BuildingConstructionInteractable _interactable;
    [SerializeField] private SoundData _constructionLoop;
    [SerializeField] private SoundData _constructionComplete;

    private bool _audioPlaying;
    private float _lastConstructionProgressTime;

    private SoundEmitter _sound;
    
    public void OnBeginPlacement()
    {
    }

    public void OnCancelPlacement()
    {
    }

    public void OnFinishPlacement()
    {
        _interactable.gameObject.SetActive(true);
    }

    public void ConstructionProgressUpdated(float progress)
    {
        if (_sound == null)
        {
            _sound = _constructionLoop.PlayLooping(transform.position);
            _audioPlaying = true;
        }
        
        _lastConstructionProgressTime = Time.time;
    }

    public void OnFinishConstruction()
    {
        if (_sound != null)
        {
            _sound.Stop();
            _sound = null;
        }

        _constructionComplete.Play(transform.position);
        _interactable.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!_audioPlaying) return;

        if (!(Time.time - _lastConstructionProgressTime > AudioStopDelay)) return;
        if (_sound == null) return;
        
        _sound.Stop();
        _sound = null;
    }
}
