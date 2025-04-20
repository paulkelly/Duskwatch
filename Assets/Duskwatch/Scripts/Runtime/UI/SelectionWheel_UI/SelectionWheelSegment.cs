using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelSegment : MonoBehaviour
{
    [SerializeField] private SelectionWheelColourConfig _colours;
    
    [SerializeField] private SelectionWheelUIOption _optionUI;
    [SerializeField] private Image _segmentImage;

    public bool HasOption => _optionUI.HasOption;
    public SelectionWheelConfigOption Option => _optionUI.Option;

    private bool _selected;
    public bool Selected
    {
        get => _selected;
        set
        {
            if(_selected == value) return;
            _selected = value;

            _optionUI.Selected = _selected;

            UpdateColour();
        }
    }
    
    public void SetOption(SelectionWheelConfigOption option)
    {
        _optionUI.SetOption(option);
        UpdateColour();
    }

    public void OnSelect()
    {
        _optionUI.OnSelect();
    }

    private void UpdateColour()
    {
        if (!_optionUI.HasOption)
        {
            _segmentImage.color = _selected ? _colours.segmentDisabledHighlightColour : _colours.segmentDisabledColour;
            return;
        }

        _segmentImage.color = _selected ? _colours.segmentHighlightColour : _colours.segmentDefaultColour;
    }
}
