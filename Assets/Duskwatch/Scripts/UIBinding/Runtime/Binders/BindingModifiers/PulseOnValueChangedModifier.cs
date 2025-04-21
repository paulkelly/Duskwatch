using System;
using PrimeTween;
using UnityEngine;

namespace DataBinding
{
    public class PulseOnValueChangedModifier : MonoBehaviour, IBindingModifer<TMPTextBinding>
    {
        [SerializeField] private float _strength = 1;
        [SerializeField] private float _duration = 0.3f;

        public void OnBindingChanged(TMPTextBinding binder)
        {
            Tween.PunchScale(binder.transform, Vector3.one * _strength, _duration);
        }
    }
}
