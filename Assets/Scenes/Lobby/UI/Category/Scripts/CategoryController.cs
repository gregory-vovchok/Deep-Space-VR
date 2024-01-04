using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// preview image mast have 256x256 size
[RequireComponent(typeof(MeshRenderer))]
public class CategoryController : MonoBehaviour, ISelectableObject
{
    CategoryData categoryData;
    public string Name { get => categoryData.label; }
    [SerializeField] TextMeshPro label;
    MeshRenderer renderer;

    [SerializeField] TileFrame tileFramePrefab;
    TileFrame tileFrame;


    void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    public void SetData(CategoryData _categoryData)
    {
        categoryData = _categoryData;

        renderer.material = _categoryData.material;

        label.text = _categoryData.name;
    }

    public void OnClick()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    public void Select()
    {
        tileFrame = Instantiate(tileFramePrefab, transform);
        PlayerDataController.Instance.Select(categoryData.id);
    }

    public void Unselect()
    {
        Destroy(tileFrame.gameObject);
    }
}
