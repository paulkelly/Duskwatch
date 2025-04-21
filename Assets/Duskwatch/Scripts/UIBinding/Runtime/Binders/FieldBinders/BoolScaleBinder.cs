using System;
using PrimeTween;
using UnityEngine;

namespace DataBinding
{
    public class BoolScaleBinder : AbstractBinder<bool>
    {
        [SerializeField] private RectTransform rectTransform;
        [BindingType(typeof(bool))] public BindingField target;

        [SerializeField] private float _falseValue = 1;
        [SerializeField] private float _trueValue = 1.3f;
        [SerializeField] private float _tweenTime = 0.2f;
        
        protected override BindingField BindingField => target;

        protected override void OnBind(object obj)
        {
            if (bindableVariable.GetValue())
            {
                rectTransform.localScale = Vector3.one * _trueValue;   
            }
            else
            {
                rectTransform.localScale = Vector3.one * _falseValue;
            }
        }
    
        protected override void OnBindingValueChanged()
        {
            if (bindableVariable.GetValue())
            {
                Tween.Scale(rectTransform, _trueValue, _tweenTime);
            }
            else
            {
                Tween.Scale(rectTransform, _falseValue, _tweenTime);
            }
        }


        private void Awake()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }

#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();
            rectTransform = GetComponent<RectTransform>();
        }
#endif
    }
}
