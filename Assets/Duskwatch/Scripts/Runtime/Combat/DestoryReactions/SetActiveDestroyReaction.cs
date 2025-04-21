using UnityEngine;

public class SetActiveDestroyReaction : AbstractDestroyReaction
{
    [SerializeField] private bool _setEnabled; 
    public override void OnDestroyed()
    {
        gameObject.SetActive(_setEnabled);
    }

    public override void OnResurrect()
    {
        gameObject.SetActive(!_setEnabled);
    }
}
