using DataBinding;
using UnityEngine;

[Bindable]
public class Resource
{
    public BindableString name;
    public BindableSprite icon;
    public BindableInt amount;
    public BindableInt inUse;
    public bool nonDepleting;

    public bool hidden;

    public Resource(ResourceDefinition definition)
    {
        name = new BindableString(definition.displayName);
        icon = new BindableSprite(definition.icon);
        amount = new BindableInt(0);
        inUse = new BindableInt(0);
        nonDepleting = definition.nonDepleting;
        hidden = definition.hidden;
    }
}
