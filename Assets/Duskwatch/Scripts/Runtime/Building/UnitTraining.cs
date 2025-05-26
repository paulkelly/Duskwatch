using System;
using DataBinding;
using UnityEngine;

[Bindable]
public class UnitTraining : MonoBehaviour, IBuildingActiveFunctions
{
    [SerializeField] private GameObject _unitPrefab; 
    [SerializeField] private Transform _spawnPoint; 
    [SerializeField] private ResourceDefinition _housing;
    
    public BindableTransform Position;
    public BindableSprite InProgressIcon;
    [NonSerialized] public BindableSprite MissingRequirementIcon = new BindableSprite(null);
    [NonSerialized] public BindableBool InProgress = new BindableBool(false);
    [NonSerialized] public BindableFloat Progress = new BindableFloat(0);
    
    private float _timeUntilNextSpawn;
    private bool _isActive;
    [SerializeField] private float _spawnTime;

    private bool _tagRegistered;

    private void OnEnable()
    {
        //TODO: Use requirements
        MissingRequirementIcon.SetValue(_housing.icon);
        
        if(!UIReferences.Instance) return;
        
        UIReferences.Instance.UnitTrainingTags.DisplayTag(this);
        _tagRegistered = true;
    }
    private void OnDisable()
    {
        UIReferences.Instance.UnitTrainingTags.HideTag(this);
    }

    private void Start()
    {
        if(_tagRegistered) return;
        UIReferences.Instance.UnitTrainingTags.DisplayTag(this);
        _tagRegistered = true;
    }

    public void OnBuildingActive()
    {
        _isActive = true;
    }

    public void OnBuildingInactive()
    {
        _isActive = false;
    }
    
    private void Update()
    {
        if (!_isActive)
        {
            if(InProgress) InProgress.SetValue(false);
            if(Progress != 0) Progress.SetValue(0);
            return;
        }
        
        if (SceneReferences.Instance.ResourceManager.HasResources(_housing, 1))
        {
            if(!InProgress) InProgress.SetValue(true);
            _timeUntilNextSpawn += Time.deltaTime;

            if (_timeUntilNextSpawn >= _spawnTime)
            {
                _timeUntilNextSpawn = 0f;
                SpawnUnit();
            }
            
            Progress.SetValue(Mathf.Clamp01(_timeUntilNextSpawn/_spawnTime));
        }
        else
        {
            if(InProgress) InProgress.SetValue(false);
            if(Progress != 0) Progress.SetValue(0);
        }
    }
    
    private void SpawnUnit()
    {
        Instantiate(_unitPrefab, _spawnPoint.position, Quaternion.identity);
    }
}
