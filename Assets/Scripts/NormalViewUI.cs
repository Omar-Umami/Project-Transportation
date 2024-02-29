using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NormalViewUI : MonoBehaviour
{
    private RenderTexture _screenTexture;

    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private Image desiredPhotoDisplay;
    private readonly Vector2 initialSizeDelta = new Vector2(862, 478);
    private readonly Vector2 targetSizeDelta = new Vector2(431, 239);
    [SerializeField] private Image blackScreen;

    public async void ShowPhoto(Texture2D screenCapture)
    {
        photoDisplayArea.gameObject.SetActive(false);
        var photoSprite = Sprite.Create(screenCapture,
            new Rect(0, 0, screenCapture.width, screenCapture.height), new Vector2(0.5f, 0.5f), 100f);
        photoDisplayArea.sprite = photoSprite;
        
        photoDisplayArea.rectTransform.localPosition = Vector2.zero;

        photoDisplayArea.rectTransform.sizeDelta = initialSizeDelta;
        await ShowCameraEffect();
        photoDisplayArea.DOFade(1, 0.2f).From(0);
        photoDisplayArea.gameObject.SetActive(true);
        
        photoDisplayArea.rectTransform.DOAnchorPos(Vector2.zero, 1).SetDelay(1f);
        photoDisplayArea.rectTransform.DOSizeDelta(targetSizeDelta, 1f).SetDelay(1f);
    }


    private async Task ShowCameraEffect()
    {
        await blackScreen.DOFade(1f, 0.3f).AsyncWaitForCompletion();
        await blackScreen.DOFade(0f, 0.2f).AsyncWaitForCompletion();
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
