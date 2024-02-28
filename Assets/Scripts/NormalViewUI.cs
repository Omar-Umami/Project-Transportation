using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NormalViewUI : MonoBehaviour
{
    private RenderTexture _screenTexture;

    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private Image desiredPhotoDisplay;


    public static NormalViewUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPhoto(Texture2D screenCapture)
    {
        var photoSprite = Sprite.Create(screenCapture,
            new Rect(0, 0, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        photoDisplayArea.sprite = photoSprite;
    }

    public void ShowDesiredPhoto(Texture2D desiredPhoto)
    {
        var photoSprite = Sprite.Create(desiredPhoto,
            new Rect(0, 0, desiredPhoto.width, desiredPhoto.height), new Vector2(0.5f, 0.5f), 100f);
        desiredPhotoDisplay.sprite = photoSprite;
    }

    // [ContextMenu("SaveImage")]
    // public void SaveTexture()
    // {
    //     var bytes = screenCapture.EncodeToPNG();
    //     File.WriteAllBytes(FilePath, bytes);
    //     Debug.Log("Texture saved to: " + FilePath);
    // }
}
