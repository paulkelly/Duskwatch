using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace DataBinding
{
    public class BoolColourBinder : AbstractBinder<bool>
    {
        [SerializeField] private Graphic graphic;
        [BindingType(typeof(bool))] public BindingField target;

        [SerializeField] private Color _falseValue;
        [SerializeField] private Color _trueValue;
        [SerializeField] private float _tweenTime = 0.2f;
        
        protected override BindingField BindingField => target;

        protected override void OnBind(object obj)
        {
            if (bindableVariable.GetValue())
            {
                graphic.color = _trueValue;
            }
            else
            {
                graphic.color = _falseValue;
            }
        }
    
        protected override void OnBindingValueChanged()
        {
            if (bindableVariable.GetValue())
            {
                Tween.Color(graphic, _trueValue, _tweenTime);
            }
            else
            {
                Tween.Color(graphic, _falseValue, _tweenTime);
            }
        }


        private void Awake()
        {
            if (graphic == null)
            {
                graphic = GetComponent<Graphic>();
            }
        }

#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();
            graphic = GetComponent<Graphic>();
        }
#endif
    }
}
