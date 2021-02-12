//-----------------------------------------------------------------------
// <copyright file="VrModeController.cs" company="Google LLC">
// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using Google.XR.Cardboard;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

/// <summary>
/// Turns VR mode on and off.
/// </summary>
public class VrModeController : MonoBehaviour
{
    // AltRealities Update
    // For updating Camera's FoV & Aspect Ratio after build
    public Camera MainCamera;
    // For toggling NonVR GameObjects
    public GameObject[] nonVrObjects;

    /// <summary>
    /// Gets a value indicating whether the screen has been touched this frame.
    /// </summary>
    private bool _isScreenTouched
    {
        get
        {
            return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }

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

    private void Awake()
    {
        // Hide NonVR objects
        foreach (GameObject _object in nonVrObjects)
        {
            _object.SetActive(false);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    public void Start()
    {
        // Configures the app to not shut down the screen and sets the brightness to maximum.
        // Brightness control is expected to work only in iOS, see:
        // https://docs.unity3d.com/ScriptReference/Screen-brightness.html.
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1.0f;

        // Checks if the device parameters are stored and scans them if not.
        // This is only required if the XR plugin is initialized on startup,
        // otherwise these API calls can be removed and just be used when the XR
        // plugin is started.
        if (!Api.HasDeviceParams())
        {
            Api.ScanDeviceParams();
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
#if !UNITY_EDITOR
        if (_isVrModeEnabled)
        {
            if (Api.IsCloseButtonPressed)
            {
                ExitVR();
            }

            if (Api.IsGearButtonPressed)
            {
                Api.ScanDeviceParams();
            }

            Api.UpdateScreenParams();
        }
        else
        {
            // TODO(b/171727815): Add a button to switch to VR mode.
            //if (_isScreenTouched)
            //{
            //    EnterVR();
            //}
        }
#endif
    }

    /// <summary>
    /// Enters VR mode.
    /// </summary>
    public void EnterVR()
    {
        // Hide NonVR objects
        foreach (GameObject _object in nonVrObjects)
        {
            _object.SetActive(false);
        }

        StartCoroutine(StartXR());
        if (Api.HasNewDeviceParams())
        {
            Api.ReloadDeviceParams();
        }
    }

    /// <summary>
    /// Exits VR mode.
    /// </summary>
    private void ExitVR()
    {
        StopXR();

        // Show NonVR objects
        foreach (GameObject _object in nonVrObjects)
        {
            _object.SetActive(true);
        }
    }

    /// <summary>
    /// Initializes and starts the Cardboard XR plugin.
    /// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
    /// </summary>
    ///
    /// <returns>
    /// Returns result value of <c>InitializeLoader</c> method from the XR General Settings Manager.
    /// </returns>
    private IEnumerator StartXR()
    {
        Debug.Log("Initializing XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed.");
        }
        else
        {
            Debug.Log("XR initialized.");

            Debug.Log("Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            Debug.Log("XR started.");
        }
    }

    /// <summary>
    /// Stops and deinitializes the Cardboard XR plugin.
    /// See https://docs.unity3d.com/Packages/com.unity.xr.management@3.2/manual/index.html.
    /// </summary>
    private void StopXR()
    {
        Debug.Log("Stopping XR...");
        XRGeneralSettings.Instance.Manager.StopSubsystems();
        Debug.Log("XR stopped.");

        Debug.Log("Deinitializing XR...");
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        Debug.Log("XR deinitialized.");

        // AltRealities: Reset FoV & Aspect Ratio
        MainCamera.fieldOfView = 60.0f;
        MainCamera.ResetAspect();
        MainCamera.ResetProjectionMatrix();
        MainCamera.ResetWorldToCameraMatrix();
        Debug.Log("Camera is now set on default parameters");
    }
}
