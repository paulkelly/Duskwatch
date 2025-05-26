using DataBinding;
using UnityEngine;

public class UnitTrainingStatusTag : AbstractStatusTag
{
    [SerializeField] private UIBinding _binder;
    
    public override void Bind(object obj)
    {
        _binder.Bind(obj);
    }

    public override void Hide()
    {
        HideTween();
    }

    protected override void ReturnToPool()
    {
        UIReferences.Instance.UnitTrainingTags.ReturnTag(this);
    }
}
