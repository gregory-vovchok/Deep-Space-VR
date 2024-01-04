using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VR3D;

[CreateAssetMenu(fileName = "New Category Data", menuName = "Category Data")]
public class CategoryData : ScriptableObject
{
    public enum CategoryId
    {
        NONE,
        SPACE,
        FANTASY,
        NATURE,
        EXTRA,
        ABSTRACT,
        PHOTO
    }

    public Material material;
    public string label;
    public CategoryId id;
    public AudioClip[] audioClips;
    public StereoscopicImageData[] stereoscopicImages;
}
