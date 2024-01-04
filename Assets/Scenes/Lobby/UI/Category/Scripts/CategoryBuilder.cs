using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryBuilder : MonoBehaviour
{
    public static string CategoryTag = "Category";
    [SerializeField] CategoryController categoryPrefab;

    void Start()
    {
        CategoryData[] categories = PlayerDataController.Instance.Categories;

        for (int i = 0; i < categories.Length; i++)
        {
            CreateCategory(categories[i]);
        }
    }

    void CreateCategory(CategoryData category)
    {
        CategoryController item = Instantiate(categoryPrefab, transform);

        item.SetData(category);
    }
}
