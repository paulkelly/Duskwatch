using System;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _camera;
    private void OnEnable()
    {
        _camera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(_camera.position-transform.position, _camera.up);
    }
}
