using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectionWheelRequirementUI : MonoBehaviour
{
    [SerializeField] private Image _iconBG;
    [SerializeField] private Image _textBG;
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _text;

    [SerializeField] private Color _metColor;
    [SerializeField] private Color _notMetColor;

    private ISelectionWheelRequirement _requirement;
    public bool IsRequirementMet { get; private set; }

    public void SetRequirement(ISelectionWheelRequirement requirement)
    {
        _requirement = requirement;
        IsRequirementMet = _requirement.IsRequirementMet();
        _icon.sprite = requirement.icon;
        _text.text = requirement.amount.ToString();
        UpdateColor();
    }

    public void SetRequirementMet(bool isMet)
    {
        if (IsRequirementMet == isMet) return;
        IsRequirementMet = isMet;

        UpdateColor();
    }

    public void Update()
    {
        if(!_requirement.IsAlive()) return;

        SetRequirementMet(_requirement.IsRequirementMet());
    }

    private void UpdateColor()
    {
        _text.color = IsRequirementMet ? _metColor : _notMetColor;
    }
}
