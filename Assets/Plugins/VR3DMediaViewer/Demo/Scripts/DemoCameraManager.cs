/*
 * DemoCameraManager
 * 
 * This script was made by Jason Peterson (DarkAkuma) of http://darkakumadev.z-net.us/
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VR3D
{
    public class DemoCameraManager : MonoBehaviour
    {
        public Camera _camera;

        public bool preserveComponents = false;

        void OnEnable()
        {
            if (VR3DMediaViewer.RenderingMethod == RenderingMethod.Layered)
                VR3DCameraSetup.SetupCameras(_camera, preserveComponents);
        }
    }
}