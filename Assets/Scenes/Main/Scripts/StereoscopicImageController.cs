using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VR3D;


public class StereoscopicImageController : MonoBehaviour, ISelectableObject
{
    VR3DMediaViewer vr3dMedia;
    
    void Awake()
    {
        vr3dMedia = GetComponent<VR3DMediaViewer>();
    }

    public void SetImage(StereoscopicImageData _image)
    {
        vr3dMedia.SetNewImage(_image.s3DImage);

        SetSize(_image.width / 2, _image.height);
    }

    void SetSize(float _width, float _height)
    {
        transform.localScale = new Vector3(_width, transform.localScale.y, _height);
    }

    public void OnClick()
    { }

    public void Select()
    { }

    public void Unselect()
    { }
}
