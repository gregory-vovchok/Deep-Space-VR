/*
 * VR3DCameraSetup_Editor
 * 
 * This script was made by Jason Peterson (DarkAkuma) of http://darkakumadev.z-net.us/
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace VR3D
{
    /// <summary>
    /// This adds context menus to Camera components, whoes purpose is to turn a stock VR camera into 2 in which each are deticated to each eye, and thus can have different settings.
    /// </summary>
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Camera), true)]
    public class VR3DCameraSetup_Editor : Editor
    {
        private Camera m_camera;
        private Editor m_editor;

        public bool preserveComponents = false;

        private bool useSinglePass = false;

        void OnEnable()
        {
            if (VR3DMediaViewer.RenderingMethod == RenderingMethod.Shader)
            {
                // We only care if at least one canvas is in scene and set to single pass. One if enough.
                useSinglePass = true;
            }
        }

        public override void OnInspectorGUI()
        {
            if (m_camera == null)
                m_camera = (Camera)target;

            if (m_editor == null)
            {
                Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

                foreach (Assembly assembly in assemblies)
                {
                    System.Type type = assembly.GetType("UnityEditor.CameraEditor");

                    if (type != null)
                    {
                        m_editor = Editor.CreateEditor(target, type);
                        break;
                    }
                }
            }

            if (m_editor != null)
            {
                m_editor.OnInspectorGUI();

                if (!useSinglePass && !CheckCameras())
                {
                    // This is just a visual sperator.
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    // We color this area of the inspector different to draw attention to it.
                    GUIStyle areaStyle = new GUIStyle();

                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.0f, 1.0f, 1.0f, 0.5f)); // Cyan with half opacity.
                    tex.Apply();

                    areaStyle.normal.background = tex;

                    GUILayout.BeginVertical(areaStyle);

                    // We make the areas text label red to also help draw attention to it.
                    GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
                    labelStyle.normal.textColor = Color.red;
                    GUILayout.Label("VR3DMediaViewer Camera Setup", labelStyle);

                    GUILayout.Space(10);

                    preserveComponents = GUILayout.Toggle(preserveComponents, "Perserve components for both eyes");

                    if (GUILayout.Button("Make 3D Camera"))
                    {
                        VR3DCameraSetup.SetupCameras((Camera)target, preserveComponents);
                    }

                    GUILayout.EndVertical();
                }
            }
        }
        
        /// <summary>
        /// Takes a single camera and splits it into 2 cameras, each deticated to a single eye. Both cameras are automatically set up to work with VR3DMediaViewer. Any components that are non-standard for a camera are placed on the left eyes camera.
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Camera/VR3DMediaViewer Camera Setup")]
        static void SetupCamera(MenuCommand command)
        {
            Camera camera = (Camera)command.context;
                        
            VR3DCameraSetup.SetupCameras(camera, false);
                        
            Debug.LogWarning("VR3DMediaViewer: Any non-standard camera components like scripts, that were on the original camera are now on the \"-Left\" camera. You may need to check over each component to make sure it's where it makes the most sence.");
        }

        /// <summary>
        /// Takes a single camera and splits it into 2 cameras, each deticated to a single eye. Both cameras are automatically set up to work with VR3DMediaViewer. Any components that are non-standard for a camera are placed on both eyes cameras.
        /// </summary>
        /// <param name="command"></param>
        [MenuItem("CONTEXT/Camera/VR3DMediaViewer Camera Setup - Perserve Components for Both Eyes")]
        static void SetupCamera2(MenuCommand command)
        {
            Camera camera = (Camera)command.context;
            VR3DCameraSetup.SetupCameras(camera, true);

            Debug.LogWarning("VR3DMediaViewer: Any non-standard camera components like scripts, that were on the original camera are now on the \"-Left\" & \"-Right\" cameras. You may need to check over each component to make sure it's where it makes the most sence.");
        }

        
        /// <summary>
        /// Check the camera to see if its already set for 3D.
        /// </summary>
        bool CheckCameras()
        {
            if ((m_camera.stereoTargetEye == StereoTargetEyeMask.Left &&
                (~(m_camera.cullingMask) & (1 << LayerManager.RightLayerIndex)) != 0) ||
                (m_camera.stereoTargetEye == StereoTargetEyeMask.Right &&
                (~(m_camera.cullingMask) & (1 << LayerManager.LeftLayerIndex)) != 0) ||
                m_camera.gameObject.name.Contains("Example Cross-eyed Camera Rig") ||
                (m_camera.transform.parent != null && m_camera.transform.parent.name.Contains("Example Cross-eyed Camera Rig")) || // Exclude our assets example cross-eyed camera rigs.
                (m_camera.transform.parent != null && m_camera.transform.parent.GetComponent<DemoCameraManager>() != null)) // 
                return true;

            return false;
        }        
    }
}