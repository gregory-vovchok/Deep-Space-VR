using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableObject
{
    public void Select();

    public void Unselect();

    public void OnClick();
}
