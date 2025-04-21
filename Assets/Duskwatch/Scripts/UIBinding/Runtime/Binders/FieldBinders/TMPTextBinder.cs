using System;
using System.Collections;
using System.Collections.Generic;
using DataBinding;
using TMPro;
using UnityEngine;

namespace DataBinding
{
    [Serializable]
    public class TMPTextBinding : AbstractTextBinder
    {
        [SerializeField] private TMP_Text text;
        
        private bool _hasModifiers;
        private IBindingModifer<TMPTextBinding>[] _modifiers;

        protected override void OnValueChanged()
        {
            string newValue = GetBoundText();
            if(string.Equals(newValue, text.text)) return;
            text.text = newValue;
            
            if (!_hasModifiers) return;
            
            for (int i = 0; i < _modifiers.Length; i++)
            {
                _modifiers[i].OnBindingChanged(this);
            }
        }
        
        private void Awake()
        {
            if (text == null)
            {
                text = GetComponent<TMP_Text>();
            }
            
            _modifiers = GetComponentsInChildren<IBindingModifer<TMPTextBinding>>();
            _hasModifiers = _modifiers != null;
        }

#if UNITY_EDITOR
        public override void Reset()
        {
            base.Reset();
            text = GetComponent<TMP_Text>();
        }
#endif
    }
}