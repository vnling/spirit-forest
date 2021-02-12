//-----------------------------------------------------------------------
// <copyright file="CameraPointer.cs" company="Google LLC">
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
using UnityEngine;

/// <summary>
/// Sends messages to gazed GameObject.
/// </summary>
public class CameraPointer : MonoBehaviour
{
    public LayerMask layerMask;
    public GvrReticlePointer reticlePointer;

    private const float _maxDistance = 20;
    private GameObject _gazedAtObject = null;

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    public void Update()
    {
        // Casts ray towards camera's forward direction, to detect if a GameObject is being gazed
        // at.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _maxDistance, layerMask))
        {
            // GameObject detected in front of the camera.
            if (_gazedAtObject != hit.transform.gameObject)
            {
                // New GameObject.
                _gazedAtObject?.SendMessage("OnPointerExit", SendMessageOptions.DontRequireReceiver);
                _gazedAtObject = hit.transform.gameObject;
                _gazedAtObject.SendMessage("OnPointerEnter", SendMessageOptions.DontRequireReceiver);
                if (reticlePointer)
                {
                    reticlePointer.OnPointerEnter(hit, true);
                }
            }
            else if (_gazedAtObject == hit.transform.gameObject)
            {
                _gazedAtObject.SendMessage("OnPointerHover", SendMessageOptions.DontRequireReceiver);
                if (reticlePointer)
                {
                    reticlePointer.OnPointerHover(hit, true);
                }
            }
        }
        else
        {
            // No GameObject detected in front of the camera.
            // AltReality: Set SendMessageOptions to be DontRequireReceiver, to get rid of error
            _gazedAtObject?.SendMessage("OnPointerExit", SendMessageOptions.DontRequireReceiver);
            _gazedAtObject = null;
            if (reticlePointer)
            {
                reticlePointer.OnPointerExit();
            }
        }

        // Checks for screen touches.
        //if (Google.XR.Cardboard.Api.IsTriggerPressed)
        //{
        //    _gazedAtObject?.SendMessage("OnPointerClick");
        //}

        // AltReality: use Input.GetTouch() so can detect touch
        // when both VR mode or non VR mode
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _gazedAtObject?.SendMessage("OnPointerClick", SendMessageOptions.DontRequireReceiver);
            }
        }

        // AltReality: add mouse click support
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            _gazedAtObject?.SendMessage("OnPointerClick", SendMessageOptions.DontRequireReceiver);
        }
#endif
    }
}
