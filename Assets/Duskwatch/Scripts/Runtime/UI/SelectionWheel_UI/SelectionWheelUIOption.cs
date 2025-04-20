using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelUIOption : MonoBehaviour
{
    private const float HighlightScale = 1.5f;
    private const float TweenTime = 0.2f;
    
    [SerializeField] private SelectionWheelColourConfig _colours;
    
    [SerializeField] private Image _rim;
    [SerializeField] private Image _background;
    [SerializeField] private Image _icon;
    
    [SerializeField] private RectTransform _scaleTransform;

    private SelectionWheelConfigOption _option;

    public bool HasOption => _option != null;
    public SelectionWheelConfigOption Option => _option;

    private bool _selected;
    public bool Selected
    {
        get => _selected;
        set
        {
            if(_selected == value) return;
            _selected = value;
            
            if(_option == null) return;

            if (_selected)
            {
                Tween.Scale(_scaleTransform, 1, HighlightScale, TweenTime);
            }
            else
            {
                Tween.Scale(_scaleTransform, HighlightScale, 1, TweenTime);
            }
            
            UpdateColour();
        }
    }

    public void SetOption(SelectionWheelConfigOption option)
    {
        _option = option;
        gameObject.SetActive(_option != null);
        _icon.sprite = _option == null ? null : option.icon;

        UpdateColour();
    }

    public void OnSelect()
    {
        if (_option == null) return;
        
        _option.OnSelect();
    }
    
    private void UpdateColour()
    {
        if (_selected)
        {
            _rim.color = _colours.optionRimHighlightColour;
            _background.color = _colours.optionHighlightColour;
        }
        else
        {
            _rim.color = _colours.optionRimDefaultColour;
            _background.color = _colours.optionDefaultColour;
        }
    }
}
