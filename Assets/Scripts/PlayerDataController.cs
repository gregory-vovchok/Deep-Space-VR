using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataController : MonoBehaviour
{
    public static PlayerDataController Instance;

    [SerializeField] CategoryData[] categories;
    public CategoryData[] Categories { get => categories; }
    public CategoryData SelectedCategory { get => categories[selectedCategoryId]; }
    int selectedCategoryId = -1;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Select(CategoryData.CategoryId id)
    {
        for (int i = 0; i < categories.Length; i++)
        {
            if (categories[i].id == id)
            {
                selectedCategoryId = i;
                break;
            }
        }
    }
}
