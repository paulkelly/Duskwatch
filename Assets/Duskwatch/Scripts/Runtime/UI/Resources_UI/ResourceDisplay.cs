using DataBinding;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _amountText;
    [SerializeField] private RectTransform _textRectTransform;

    private Resource _boundResource;

    public void Bind(Resource resource)
    {
        _boundResource = resource;
        _icon.sprite = resource.icon;

        _boundResource.amount.onValueChanged += OnValueChanged;
        if (_boundResource.nonDepleting)
        {
            _boundResource.inUse.onValueChanged += OnValueChanged;
        }

        _amountText.text = GetTextValue();
    }

    private void OnValueChanged()
    {
        string newValue = GetTextValue();
        if(string.Equals(newValue, _amountText.text)) return;

        _amountText.text = newValue;
        Tween.PunchScale(_textRectTransform, Vector3.one * 0.5f, 0.3f);
    }

    private string GetTextValue()
    {
        if (_boundResource.nonDepleting)
        {
            return $"{_boundResource.inUse.GetValue()} / {_boundResource.amount.GetValue()}";
        }
        
        return $"{_boundResource.amount.GetValue()}";
    }
}
