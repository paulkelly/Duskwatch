using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ResourceDefinitionCollection", menuName = "Scriptable Objects/ResourceDefinitionCollection")]
public class ResourceDefinitionCollection : ScriptableObject
{
    [SerializeField] public List<ResourceDefinition> resources;

    #if UNITY_EDITOR
    [Button]
    public void FindAllResourceDefinitions()
    {
        var guids = AssetDatabase.FindAssets("t:ResourceDefinition");
        foreach (var guid in guids)
        {
            var def = AssetDatabase.LoadAssetAtPath<ResourceDefinition>(AssetDatabase.GUIDToAssetPath(guid));
            if(resources.Contains(def)) continue;
            resources.Add(def);
        }
    }
    #endif
}
