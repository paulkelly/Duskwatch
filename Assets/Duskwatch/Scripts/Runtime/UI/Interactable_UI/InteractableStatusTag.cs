using DataBinding;
using PrimeTween;
using UnityEngine;

public class InteractableStatusTag : MonoBehaviour
{
    private const float TweenTime = 0.2f;
    [SerializeField] private AbstractBinder _binder;
    private InteractableObj _interactable;

    public void Display(InteractableObj obj)
    {
        _interactable = obj;
        gameObject.SetActive(true);
        Tween.Scale(transform, 1, TweenTime);
        
        _binder.Bind(obj);
    }

    public void Hide()
    {
        if(this == null) return;

        if (_interactable != null && _interactable.Progress >= 1)
        {
            Tween.Scale(transform, 1.3f, TweenTime).Chain(Tween.Scale(transform, 0, TweenTime).OnComplete(ReturnToPool));
        }
        else
        {
            Tween.Scale(transform, 0, TweenTime).OnComplete(ReturnToPool);
        }
    }
    
    public void SetInactive()
    {
        gameObject.SetActive(false);
    }

    private void ReturnToPool()
    {
        UIReferences.Instance.InteractableTags.ReturnTag(this);
    }
}
