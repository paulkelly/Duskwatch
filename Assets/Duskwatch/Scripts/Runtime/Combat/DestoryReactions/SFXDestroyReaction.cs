using AudioSystem;
using UnityEngine;

public class SFXDestroyReaction : AbstractDestroyReaction
{
    [SerializeField] private SoundData _sfx;
    public override void OnDestroyed()
    {
        _sfx.Play();
    }

    public override void OnResurrect()
    {
    }
}
