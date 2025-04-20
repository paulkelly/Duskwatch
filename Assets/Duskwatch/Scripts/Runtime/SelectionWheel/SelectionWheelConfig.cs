using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Changelist;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectionWheelConfig", menuName = "Scriptable Objects/SelectionWheelConfig")]
public class SelectionWheelConfig : ScriptableObject
{
    [RequiredListLength(12)] public SelectionWheelConfigOption[] options;

    private void Reset()
    {
        options = new SelectionWheelConfigOption[12];
    }
}
