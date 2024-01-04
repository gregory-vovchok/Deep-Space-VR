using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] StereoscopicImageController stereoscopicImage;
    int currentImageIndex = 0;
    int currentMusicIndex = 0;
    [SerializeField] RayCursor leftCursor;
    [SerializeField] RayCursor rightCursor;
    CategoryData selectedCategory;
    bool isInitialized = false;
    [SerializeField] float scalerModifier = 0.001f;
    [SerializeField] float minScale = 0.04f;
    [SerializeField] float maxScale = 0.08f;
    [SerializeField] TipController tipController;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        selectedCategory = PlayerDataController.Instance.SelectedCategory;

        Init();

        UpdateTip();
    }

    void Init()
    {
        SetMusic(currentMusicIndex);
        SetImage(currentImageIndex);
        isInitialized = true;
    }

    void SetMusic(int index)
    {
        audioSource.clip = selectedCategory.audioClips[index];
        audioSource.Play();
    }

    void SetImage(int index)
    {
        stereoscopicImage.SetImage(selectedCategory.stereoscopicImages[index]);

        UpdateTip();
    }

    void UpdateTip()
    {
        var image = selectedCategory.stereoscopicImages[currentImageIndex];
        tipController.SetState(
            selectedCategory.stereoscopicImages.Length,
            currentImageIndex + 1,
            image.label,
            image.type.ToString(),
            image.description);
    }

    void Update()
    {
        if (isInitialized)
        {
            if (leftCursor.CheckClick(OVRInput.RawButton.Y))
            {
                SetImage(NextIndex());
            }
            else if(leftCursor.CheckClick(OVRInput.RawButton.X))
            {
                SetImage(PreviousIndex());
            }
            else if (rightCursor.CheckClick(OVRInput.RawButton.B))
            {
                SetImage(NextIndex());
            }
            else if (rightCursor.CheckClick(OVRInput.RawButton.A))
            {
                SetImage(PreviousIndex());
            }
            else if (OVRInput.GetDown(OVRInput.RawButton.Start))
            {
                LoadLobby();
            }

            Vector2 leftStick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            DisplaceZoom(leftStick.y);

            Vector2 rightStick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            DisplaceZoom(rightStick.y);
        }
    }

    public void DisplaceZoom(float offset)
    {
        Transform imageParent = stereoscopicImage.transform.parent;
        offset = offset * scalerModifier;

        if (offset < 0f)
        {
            if (imageParent.localScale.x <= minScale)
            {
                return;
            }
        }
        else if (offset > 0f)
        {
            if (imageParent.localScale.x >= maxScale)
            {
                return;
            }
        }

        float scale = imageParent.localScale.x + offset;
        
        imageParent.localScale = new Vector3(
            scale,
            scale,
            imageParent.localScale.z);
    }


    void LoadLobby()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    int NextIndex()
    {
        if (selectedCategory.stereoscopicImages.Length > 1)
        {
            if (currentImageIndex < (selectedCategory.stereoscopicImages.Length - 1))
            {
                currentImageIndex++;
            }
            else
            {
                currentImageIndex = 0;
            }
        }

        return currentImageIndex;
    }

    int PreviousIndex()
    {
        if (selectedCategory.stereoscopicImages.Length > 1)
        {
            if (currentImageIndex > 0)
            {
                currentImageIndex--;
            }
            else
            {
                currentImageIndex = selectedCategory.stereoscopicImages.Length - 1;
            }
        }

        return currentImageIndex;
    }
}
