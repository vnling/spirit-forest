using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AltReality: add editor support
// - to rorate camera with mouse or touchpad while pressing down Left Alt key

[RequireComponent(typeof(Camera))]
public class VrEditorEmulator : MonoBehaviour
{
    [Range(.05f, 2)]
    public float dragRate = .2f;

    private Quaternion initialRotation;
    private Quaternion attitude;
    private Vector2 dragDegrees;
    private Vector3 lastMousePos;

    void Awake()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
#if UNITY_EDITOR
        SimulateVR();

        attitude = initialRotation * Quaternion.Euler(dragDegrees.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, -dragDegrees.y, 0) * attitude;
#endif
    }

    void SimulateVR()
    {
        var mousePos = Input.mousePosition;
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            var delta = mousePos - lastMousePos;
            dragDegrees.x -= delta.y * dragRate;
            dragDegrees.y -= delta.x * dragRate;
        }
        lastMousePos = mousePos;
    }
}
