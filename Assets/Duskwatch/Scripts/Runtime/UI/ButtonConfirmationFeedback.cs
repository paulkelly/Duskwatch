using DataBinding;
using UnityEngine;

[Bindable]
public class ButtonConfirmationFeedback
{
    public BindableBool IsPressed = new BindableBool(false);
    public BindableFloat NormalConfirmationTime = new BindableFloat(0);
}
