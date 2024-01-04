/*
 * VR3DCameraSetup
 * 
 * This script was made by Jason Peterson (DarkAkuma) of http://darkakumadev.z-net.us/
*/

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VR3D
{
    /// <summary>
    /// This adds context menus to Camera components, whoes purpose is to turn a stock VR camera into 2 in which each are deticated to each eye, and thus can have different settings.
    /// </summary>
    public class VR3DCameraSetup : MonoBehaviour
    {
        private static string[] ignoreList = { "UnityEngine.Camera", "UnityEngine.GUILayer", "UnityEngine.FlareLayer" };

        private const string STEAMVR_COMPONENT = "SteamVR_Camera";

        private const string OVRCAMERARIG_COMPONENT = "OVRCameraRig";
        private const string OVR_LEFT_CAMERA_OBJECT_NAME = "LeftEyeAnchor";
        private const string OVR_RIGHT_CAMERA_OBJECT_NAME = "RightEyeAnchor";
        private const string OVR_CENTER_CAMERA_OBJECT_NAME = "CenterEyeAnchor";

        /// <summary>
        /// The guts of the above functions.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="copyScripts">Copy scripts to both cameras or just one?</param>
        static public void SetupCameras(Camera camera, bool copyScripts)
        {
            //Camera leftCamera = (Camera)command.context;
            Camera leftCamera = camera;
            string cameraSourceName = leftCamera.name; 
            GameObject leftCameraObject = leftCamera.gameObject;

#if UNITY_EDITOR      
            Undo.RecordObject(leftCameraObject, leftCameraObject.name + " Changed");
            Undo.RecordObject(leftCamera, leftCamera.name + " Changed");
#endif

            // The SteamVR camera rig uses a wierd script that maintains an camera hierarchy that doesnt work well with a split camera.
            // So we call its own collapse method, and remove the problem script first.
            if (leftCameraObject.GetComponent(STEAMVR_COMPONENT))
            {
                leftCameraObject.GetComponent(STEAMVR_COMPONENT).SendMessage("Collapse");
                DestroyImmediate(leftCameraObject.GetComponent(STEAMVR_COMPONENT));
            }

            Component OVRCameraRigCom = null;

            // We make a copy of the camera, and this new copy will be for the right eye.
            GameObject rightCameraObject = null;

            if (leftCameraObject.transform.parent != null &&
                leftCameraObject.transform.parent.parent != null)
            {
                OVRCameraRigCom = leftCameraObject.transform.parent.parent.GetComponent(OVRCAMERARIG_COMPONENT);

                if (OVRCameraRigCom)
                {
                    // With the OVR Prefab we need to find its own split cameras.
                    OVRCameraRigCom.GetType().GetField("usePerEyeCameras").SetValue(OVRCameraRigCom, true);
                    GameObject centerCameraObject = null;

                    if (leftCameraObject.name == OVR_LEFT_CAMERA_OBJECT_NAME)
                    {
                        rightCameraObject = leftCameraObject.transform.parent.Find(OVR_RIGHT_CAMERA_OBJECT_NAME).gameObject;
                        centerCameraObject = leftCameraObject.transform.parent.Find(OVR_CENTER_CAMERA_OBJECT_NAME).gameObject;

                        if (centerCameraObject == null || rightCameraObject == null)
                        {
                            Debug.LogError("[VR3DMediaViewer] Camera Setup: Unable to locate Camera objects in OVRCameraRig. Setup can not be completed.");
                            return;
                        }
                    }
                    else if (leftCameraObject.name == OVR_CENTER_CAMERA_OBJECT_NAME)
                    {
                        centerCameraObject = leftCameraObject;
                        leftCameraObject = leftCameraObject.transform.parent.Find(OVR_LEFT_CAMERA_OBJECT_NAME).gameObject;
                        rightCameraObject = leftCameraObject.transform.parent.Find(OVR_CENTER_CAMERA_OBJECT_NAME).gameObject;

                        if (leftCameraObject == null || rightCameraObject == null)
                        {
                            Debug.LogError("[VR3DMediaViewer] Camera Setup: Unable to locate Camera objects in OVRCameraRig. Setup can not be completed.");
                            return;
                        }

                        leftCamera = leftCameraObject.GetComponent<Camera>();
                        cameraSourceName = leftCamera.name;
                    }
                    else if (leftCameraObject.name == OVR_CENTER_CAMERA_OBJECT_NAME)
                    {
                        rightCameraObject = leftCameraObject;
                        leftCameraObject = leftCameraObject.transform.parent.Find(OVR_LEFT_CAMERA_OBJECT_NAME).gameObject;
                        centerCameraObject = leftCameraObject.transform.parent.Find(OVR_CENTER_CAMERA_OBJECT_NAME).gameObject;

                        if (leftCameraObject == null || centerCameraObject == null)
                        {
                            Debug.LogError("[VR3DMediaViewer] Camera Setup: Unable to locate Camera objects in OVRCameraRig. Setup can not be completed.");
                            return;
                        }

                        leftCamera = leftCameraObject.GetComponent<Camera>();
                        cameraSourceName = leftCamera.name;
                    }

                    // Setting "usePerEyeCameras" does this, but it does not update right away. So we manually do it to prevent issues.
                    if (centerCameraObject != null && centerCameraObject.GetComponent<Camera>() != null)
                        centerCameraObject.GetComponent<Camera>().enabled = false;
                }
            }

            if (OVRCameraRigCom == null)
                rightCameraObject = GameObject.Instantiate(leftCameraObject, leftCameraObject.transform.parent);

            Camera rightCamera = rightCameraObject.GetComponent<Camera>();

#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(rightCameraObject, "Create " + rightCameraObject);
#endif

            if (OVRCameraRigCom == null)
            {
                // Name these cameras for their purposes.
                leftCamera.name = cameraSourceName + "-Left";
                rightCamera.name = cameraSourceName + "-Right";
            }
            else
            {
                // Setting "usePerEyeCameras" does this, but it does not update right away. So we manually do it to prevent issues.
                leftCamera.enabled = true;
                rightCamera.enabled = true;
            }

            // Set the camera to only render for their designated eyes.
            leftCamera.stereoTargetEye = StereoTargetEyeMask.Left;
            rightCamera.stereoTargetEye = StereoTargetEyeMask.Right;

            // Set these cameras to exclude seeing the other eyes images.
            leftCamera.cullingMask &= ~(1 << LayerManager.RightLayerIndex); // Everything except the right layer. 
            rightCamera.cullingMask &= ~(1 << LayerManager.LeftLayerIndex); // Everything except the left layer.

            if (!copyScripts) ClearBehaviors(rightCameraObject);

            // Dont need more then one audio listener.
            if (rightCameraObject.GetComponent<AudioListener>())
                DestroyImmediate(rightCameraObject.GetComponent<AudioListener>());

            // If the source game object had any children, we remove their copys from the right camera.
            ClearChildren(rightCameraObject);
        }

        /// <summary>
        /// Checks if a given behaviour is in a ignore list.
        /// </summary>
        /// <param name="behaviour">A behaviour you want to see if is suposed to be ignored.</param>
        /// <returns>True if the behaviour is ignored.</returns>
        private static bool BehaviourIgnore(Behaviour behaviour)
        {
            foreach (string ignoredBehavior in ignoreList)
                if (behaviour.GetType().ToString() == ignoredBehavior) return true;

            return false;
        }

        /// <summary>
        /// Removes all non-ignored behaviors from the given GameObject.
        /// </summary>
        /// <param name="targetGameObject">The GameObject to scan.</param>
        private static void ClearBehaviors(GameObject targetGameObject)
        {
            // We don't need it to have any scripts.            
            Behaviour[] behaviours = targetGameObject.GetComponents<Behaviour>();

            foreach (Behaviour behaviour in behaviours)
                if (!BehaviourIgnore(behaviour)) DestroyImmediate(behaviour);
        }

        /// <summary>
        /// Removes all children from the given GameObject.
        /// </summary>
        /// <param name="targetGameObject">The GameObject to scan.</param>
        private static void ClearChildren(GameObject targetGameObject)
        {
            foreach (Transform child in targetGameObject.transform)
                DestroyImmediate(child.gameObject);
        }        
    }
}