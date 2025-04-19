using UnityEngine;
using UnityEngine.UI;

namespace DataBinding
{
    public class BoolSpriteBinder : AbstractBinder<bool>
    {
        [SerializeField] private Image graphic;
        [BindingType(typeof(bool))] public BindingField target;

        [SerializeField] private Sprite _falseValue;
        [SerializeField] private Sprite _trueValue;
        
        protected override BindingField BindingField => target;

    
        protected override void OnBindingValueChanged()
        {
            if (bindableVariable.GetValue())
            {
                graphic.sprite = _trueValue;
            }
            else
            {
                graphic.sprite = _falseValue;
            }
        }


        private void Awake()
        {
            if (graphic == null)
            {
                graphic = GetComponent<Image>();
            }
        }
        
#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();
            graphic = GetComponent<Image>();
        }
#endif
    }
}
