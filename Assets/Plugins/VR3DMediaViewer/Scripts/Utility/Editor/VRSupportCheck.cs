/*
 * VRSupportCheck v1.0
 * 
 * This script was made by Jason Peterson (DarkAkuma) of http://darkakuma.z-net.us/
*/

/*
 * This is just a helper script to help relay Editor only variables at runtime.
*/
#if !UNITY_2018_2_OR_NEWER

using UnityEngine;
using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class VRSupportCheck
{
    private const string ClassName = "_PlayerSettings";
    private static string TargetCodeFile = string.Empty;

    static VRSupportCheck()
    {
        // We get the asset path this way, in case the user moves the folder path.
        string[] assetGUIDs = AssetDatabase.FindAssets(ClassName);

        if (assetGUIDs.Length > 0)
            TargetCodeFile = AssetDatabase.GUIDToAssetPath(assetGUIDs[0]);
        else
        {
            Debug.LogWarning("[VR3DMediaViewer] Unable to find \"_PlayerSettings.cs\" shader. Did you delete it?");
            return;
        }

        bool virtualRealitySupported = PlayerSettings.virtualRealitySupported;
        bool lastMode = _PlayerSettings.virtualRealitySupported;
        VR3D.StereoRenderingPath stereoRenderingPath = (VR3D.StereoRenderingPath)PlayerSettings.stereoRenderingPath;
        VR3D.StereoRenderingPath lastPath = _PlayerSettings.stereoRenderingPath;

        if (lastMode != virtualRealitySupported ||
            lastPath != stereoRenderingPath)
        {
            CreateNewClassFile(virtualRealitySupported, stereoRenderingPath);
        }
    }

    static void CreateNewClassFile(bool virtualRealitySupported, VR3D.StereoRenderingPath stereoRenderingPath)
    {
        using (StreamWriter writer = new StreamWriter(TargetCodeFile, false))
        {
            try
            {
                string code = GenerateCode(virtualRealitySupported, stereoRenderingPath);
                writer.WriteLine("{0}", code);
            }
            catch (System.Exception ex)
            {
                string msg = " threw:\n" + ex.ToString();
                Debug.LogError(msg);
                EditorUtility.DisplayDialog("Error when trying to regenrate class", msg, "OK");
            }
        }
    }

    static string GenerateCode(bool virtualRealitySupported, VR3D.StereoRenderingPath stereoRenderingPath)
    {
        string code = "";
        code += "// This script is automatically generated. It's just to provide access to an Editor variable at runtime.\n\n";
        code += "public static class " + ClassName + "\n{\n";
        code += System.String.Format("\tpublic static readonly bool virtualRealitySupported = {0};\n", virtualRealitySupported ? "true" : "false");
        code += System.String.Format("\tpublic static readonly VR3D.StereoRenderingPath stereoRenderingPath = VR3D.StereoRenderingPath.{0};\n", stereoRenderingPath.ToString());
        code += "}";
        return code;
    }
}
#endif