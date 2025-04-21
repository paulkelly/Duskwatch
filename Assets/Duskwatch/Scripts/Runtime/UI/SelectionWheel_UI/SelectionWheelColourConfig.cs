using UnityEngine;

[CreateAssetMenu(fileName = "SelectionWheelColourConfig", menuName = "Scriptable Objects/SelectionWheelColourConfig")]
public class SelectionWheelColourConfig : ScriptableObject
{
    public Color baseColour;
    public Color baseColourFade;
    public Color errorColour;
    
    public Color segmentDisabledColour;
    public Color segmentDefaultColour;
    public Color segmentHighlightColour;
    public Color segmentDisabledHighlightColour;
    
    public Color optionDefaultColour;
    public Color optionHighlightColour;
    
    public Color optionFadeDefaultColour;
    public Color optionFadeHighlightColour;
    
    public Color optionRimDefaultColour;
    public Color optionRimHighlightColour;
}
