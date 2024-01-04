using UnityEngine;
using VR3D;

[CreateAssetMenu(fileName = "Data", menuName = "VR3D/VR3DSettings", order = 1)]
public class VR3DSettings : ScriptableObject
{
    public RenderingMethod RenderingMethod = RenderingMethod.Layered;    
}