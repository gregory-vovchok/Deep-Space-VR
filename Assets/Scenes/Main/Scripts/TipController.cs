using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentPage;
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] TextMeshProUGUI type;
    [SerializeField] TextMeshProUGUI description;

    public void SetState(int _totalSize, int _currentPage, string _label, string _type, string _description)
    {
        currentPage.text = _currentPage.ToString() + "/" + _totalSize.ToString();
        label.text = _label;
        type.text = _type;
        description.text = _description;
    }
}
