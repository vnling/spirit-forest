using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

// Magic Window
// source: https://developers.google.com/vr/develop/unity/guides/magic-window

public class GyroController : MonoBehaviour
{
    // Optional, allows user to drag left/right to rotate the world.
    [Range(0.05f, 1f)]
    public const float DRAG_RATE = .2f;
    private float dragYawDegrees;

    /// <summary>
    /// Gets a value indicating whether the VR mode is enabled.
    /// </summary>
    private bool _isVrModeEnabled
    {
        get
        {
            return XRGeneralSettings.Instance.Manager.isInitializationComplete;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Make sure orientation sensor is enabled.
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_EDITOR
        if (_isVrModeEnabled)
        {
            // Unity takes care of updating camera transform in VR.
            return;
        }

        // Update `dragYawDegrees` based on user touch.
        CheckDrag();

        transform.localRotation =
          // Allow user to drag left/right to adjust direction they're facing.
          Quaternion.Euler(0f, -dragYawDegrees, 0f) *

          // Neutral position is phone held upright, not flat on a table.
          Quaternion.Euler(90f, 0f, 0f) *

          // Sensor reading, assuming default `Input.compensateSensors == true`.
          Input.gyro.attitude *

          // So image is not upside down.
          Quaternion.Euler(0f, 0f, 180f);
#endif
    }

    void CheckDrag()
    {
        if (Input.touchCount != 1)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Moved)
        {
            return;
        }

        dragYawDegrees += touch.deltaPosition.x * DRAG_RATE;
    }
}
