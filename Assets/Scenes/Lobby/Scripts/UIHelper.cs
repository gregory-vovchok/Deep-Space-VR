using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHelper : MonoBehaviour
{
    ISelectableObject selectedObject;


    public ISelectableObject SelectedObject  { get => selectedObject; }
    
    public static UIHelper Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Unselect()
    {
        Select(null);
    }

    public void Select(ISelectableObject obj)
    {
        if (selectedObject == obj) 
            return;

        if (obj != null)
        {
            if (selectedObject == null)
            {
                selectedObject = obj;
                selectedObject.Select();
            }
            else
            {
                selectedObject.Unselect();
                Select(obj);
            }
        }
        else
        {
            if (selectedObject != null)
            {
                selectedObject.Unselect();
                selectedObject = null;
            }
        }
    }
}
