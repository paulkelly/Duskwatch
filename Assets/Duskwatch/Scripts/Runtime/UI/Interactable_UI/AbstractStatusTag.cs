using PrimeTween;
using UnityEngine;

public abstract class AbstractStatusTag : MonoBehaviour
{
    private const float TweenTime = 0.2f;
    public abstract void Bind(object obj);
    public abstract void Hide();
    public void SetInactive()
    {
        gameObject.SetActive(false);
    }

    protected abstract void ReturnToPool();

    protected void PunchThenHideTween()
    {
        Tween.Scale(transform, 1.3f, TweenTime).Chain(Tween.Scale(transform, 0, TweenTime).OnComplete(ReturnToPool));
    }
    protected void HideTween()
    {
        if(transform == null) return;
        Tween.Scale(transform, 0, TweenTime).OnComplete(ReturnToPool);
    }
}
