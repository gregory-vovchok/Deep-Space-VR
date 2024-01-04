using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayCursor : MonoBehaviour
{
    public enum HandId
    {
        LEFT,
        RIGHT
    }

    LineRenderer line;
    float rayLength = 10f;

    // player input
    [SerializeField] HandId handId;
    [SerializeField] bool isEnable = false;


    void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.enabled = isEnable;
    }

    void Start()
    {
        Vector3[] points = new Vector3[] { Vector3.zero, Vector3.zero };
        line.SetPositions(points);
    }


    void Update()
    {
        if (isEnable)
        {
            UpdateRay(transform.position, transform.forward, rayLength);
        }
    }

    void UpdateRay(Vector3 startPos, Vector3 dir, float rayLength)
    {
        Ray ray = new Ray(startPos, dir);
        Vector3 endPos;

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            endPos = hitInfo.point;
           
            if (hitInfo.collider != null)
            {
                if (hitInfo.collider.gameObject.tag == CategoryBuilder.CategoryTag)
                {
                    CategoryController category = hitInfo.collider.gameObject.GetComponent<CategoryController>();

                    if (category != null)
                    {
                        UIHelper.Instance.Select(category.GetComponent<ISelectableObject>());
                        Debug.Log(category.Name);
                    }
                }
                else
                {
                    UIHelper.Instance.Unselect();
                }
            }
            else
            {
                UIHelper.Instance.Unselect();
            }
        }
        else
        {
            endPos = startPos + (dir * rayLength);
            UIHelper.Instance.Unselect();
        }

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

        if (UIHelper.Instance.SelectedObject != null)
        {
            if (CheckClick(OVRInput.RawButton.LIndexTrigger))
            {
                UIHelper.Instance.SelectedObject.OnClick();
            }
            else if (CheckClick(OVRInput.RawButton.RIndexTrigger))
            {
                UIHelper.Instance.SelectedObject.OnClick();
            }
        }
    }

    public bool CheckClick(OVRInput.RawButton button)
    {
        if (OVRInput.GetDown(button))
        {
            return true;
        }
        return false;
    }
}
