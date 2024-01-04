using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VR3D;

[CreateAssetMenu(fileName = "New Image Data", menuName = "Image Data")]
public class StereoscopicImageData : ScriptableObject
{
    public enum ObjectType
    {
        NONE,
        NEBULA,
        GALAXY,
        CONSTELLATION,
        COMET,
        CLUSTER_OF_STARS,
        SUPERNOVA
    }
    public enum ConstellationType
    {
        NONE,
        CYGNUS,
        CEPHEUS,
        KEFEUS,
        TAURUS
    }

    public Stereoscopic3DImage s3DImage;
    public int width;
    public int height;
    public string label;
    public string description;
    public ObjectType type = ObjectType.NONE;
    public ConstellationType constellation = ConstellationType.NONE;
}
