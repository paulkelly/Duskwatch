using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class DuskwatchTree : MonoBehaviour
{
    [SerializeField] private GameObject[] _variations;
    [SerializeField, MinMaxSlider(0.5f, 3)] private Vector2 _minMaxSize;


    private void Reset()
    {
        GenerateRandom();
    }

    [Button]
    private void GenerateRandom()
    {
        if(_variations == null) return;
        if (_variations.Length > 0)
        {
            GameObject obj = PrefabUtility.InstantiatePrefab(_variations[Random.Range(0, _variations.Length)], transform.parent) as GameObject;
            obj.transform.position = transform.position;
            obj.transform.eulerAngles = new Vector3(Random.Range(-1f, 1f), Random.Range(-180f, 180f), Random.Range(-1f, 1f));
            obj.transform.localScale = Vector3.one * Random.Range(_minMaxSize.x, _minMaxSize.y);
        }
    }
}
