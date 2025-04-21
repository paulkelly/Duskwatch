using System;
using UnityEngine;

public class SelectionWheelRequirementsPanel : MonoBehaviour
{
    [SerializeField] private SelectionWheelRequirementUI[] _requirementUI;
    [SerializeField] private GameObject confirmButtonFeedback;
    [SerializeField] private GameObject requirementsNotMetFeedback;
    private int _count = 0;
    
    public void AddRequirement(ISelectionWheelRequirement requirement)
    {
        if(_count > _requirementUI.Length) return;
        
        _requirementUI[_count].gameObject.SetActive(true);
        _requirementUI[_count].SetRequirement(requirement);
        _count++;
    }
    
    public void ClearRequirements()
    {
        for (int i = 0; i < _requirementUI.Length; i++)
        {
            _requirementUI[i].gameObject.SetActive(false);
        }

        _count = 0;
    }

    private void Update()
    {
        if(_count <= 0) return;

        bool allMet = true;
        for (int i = 0; i < _count; i++)
        {
            if (!_requirementUI[i].IsRequirementMet)
            {
                allMet = false;
                break;
            }
        }

        confirmButtonFeedback.SetActive(allMet);
        requirementsNotMetFeedback.SetActive(!allMet);
    }
}
