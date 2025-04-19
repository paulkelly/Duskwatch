using System;
using Shapes;
using UnityEngine;

public class InteractIndicator : MonoBehaviour
{
    [SerializeField] private Disc _fill;
    [SerializeField, Range(0, 1)] private float _test;

    private void Update()
    {
        SetFill(_test);
    }

    public void SetFill(float value)
    {
        float start = Mathf.PI / 2f;
        float end = start + Mathf.PI * 2f;
        _fill.AngRadiansEnd = Mathf.Lerp(start, end, value);
    }
}
