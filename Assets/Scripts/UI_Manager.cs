using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    #region ExposedElements
    [SerializeField]
    private Image batteryFill;

    [SerializeField] private Image takenPhoto;
    [SerializeField] private Image desiredPhoto;
    [SerializeField] private Image accuracyOutline;
    [SerializeField] private Image accuracyFiller;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI desiredPhotoText;

    [SerializeField] private Image bonusLabel;
    [SerializeField] private TextMeshProUGUI bonusMultiplier;
    [SerializeField] private TextMeshProUGUI bonusMultiplayerTimer;

    [SerializeField] private TextMeshProUGUI score;

    [SerializeField] private Transform leftHUD;
    [SerializeField] private Image switcher;
    [SerializeField] private Image cameraHUD;
    
    
    [SerializeField] private RectTransform photoGallery;
    [SerializeField] private GameObject hideButton, expandButton, showButton;
    private int currentGalleryViewMode;
    //0:Default, 1:Hidden, 2:Expanded

    #endregion


    #region TestingElements

    private float fakeAccuracy;
    private float fakeTimer;
    private Sprite fakeTaken;
    private Sprite fakeDesired;

    private int currentMode;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (currentGalleryViewMode == 0)
                HideGallery();
            if (currentGalleryViewMode == 1)
                ShowGallery();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentGalleryViewMode == 0)
                ExpandGallery();
            if (currentGalleryViewMode == 2)
                CollapseGallery();
        }
        
        
        //For testing purpose only
        if(Input.GetKeyDown(KeyCode.Space))
            SwitchMode();
        if(Input.GetKeyDown(KeyCode.M))
            ComparingPhotosVisually();
        if (Input.GetKeyDown(KeyCode.P))
            ShowBonus();
        if (Input.GetKeyDown(KeyCode.O))
            HideBonus();
        



            
            
    }
    

    void ComparingPhotosVisually()
    {
        Sequence seq = DOTween.Sequence();
        float valFloat = 0f; 
        seq.AppendCallback(()=> takenPhoto.gameObject.SetActive(true));
        seq.Join(takenPhoto.transform.DOScale(Vector3.one, 0.3f).From(Vector3.zero));
        //revisit 97 & 98 when we have a world canvas photo
        //also add From because taken photo position is change after check result
        seq.AppendInterval(0.1f);
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(true));
        seq.Join(accuracyOutline.DOFillAmount(1, 0.4f).From(0));
        seq.AppendCallback(()=> accuracyFiller.gameObject.SetActive(true));
        seq.Append(accuracyFiller.DOFillAmount(1, 3).From(0).SetEase(Ease.OutQuad));
        //switch endvalue with actual percentage value
        seq.Join(DOTween.To(() => valFloat, x => valFloat = x, 20f, 3f)
                .SetEase(Ease.OutQuad))
            .OnUpdate(()=>accuracyText.text = ((int)valFloat)+"%")
            .OnComplete(()=>CaptureResult(false));
    //change check result with actual boolean
    }

    void CaptureResult(bool result)
    {
        Sequence seq = DOTween.Sequence();
        float valFloat = 0f; 
        seq.Append(accuracyOutline.DOFillAmount(0, 0.15f));
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(false));
        seq.Join(accuracyFiller.DOFillAmount(0, 0.15f));
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(false));
        if (result)
        {
            seq.Append(takenPhoto.transform.DOMove(score.transform.position, 1));
            seq.Join(takenPhoto.transform.DOScale(0, 1));
            seq.AppendCallback(() => takenPhoto.gameObject.SetActive(false));
            seq.Join(DOTween.To(() => valFloat, x => valFloat = x, 1500, 1.5f)
                    .SetEase(Ease.OutQuad))
                .OnUpdate(() => score.text = ((int)valFloat) + " Pts")
                .OnComplete(() => NextPhoto(1, desiredPhoto.sprite));

        }
        else
        {
            seq.Join(takenPhoto.transform.DOScale(0, 0.7f));
            seq.AppendCallback(() => takenPhoto.gameObject.SetActive(false));
        }
        
    }
    void NextPhoto(int currentPhotoIndex,Sprite nextPhoto)
    {
        Sequence seq = DOTween.Sequence();
        string photoNumber = GetCountTextValue(currentPhotoIndex + 1);

        seq.Append(desiredPhotoText.DOFade(0, 0.5f));
        seq.Join(desiredPhoto.DOFade(0, 0.5f));
        seq.AppendCallback(() => ChangePhoto(photoNumber, nextPhoto));
        seq.Append(desiredPhotoText.DOFade(1, 0.5f));
        seq.Join(desiredPhoto.DOFade(1, 0.5f));

    }

    void SwitchMode()
    {
        Sequence seq = DOTween.Sequence();
        if (currentMode == 0)
        {
            seq.Append(leftHUD.DOMoveX(-175, 0.3f));
            seq.Join(switcher.DOFade(0.3f, 0.3f)).SetEase(Ease.InQuad);
            seq.Join(cameraHUD.DOFade(1, 0.2f));
            seq.Append(switcher.DOFade(0f, 0.3f));
            currentMode = 1;
        }
        else if(currentMode == 1)
        {
            seq.Append(leftHUD.DOMoveX(238, 0.3f));
            seq.Join(switcher.DOFade(0.3f, 0.3f)).SetEase(Ease.InQuad);
            seq.Join(cameraHUD.DOFade(0, 0.2f));
            seq.Append(switcher.DOFade(0f, 0.3f));
            currentMode = 0;
        }
    }

    void ShowBonus()
    {
        bonusLabel.transform.DOMoveX(80, 0.25f);
    }

    void HideBonus()
    {
        bonusLabel.transform.DOMoveX(-88, 0.25f);
    }

    #region Gallery Methods

    public void ExpandGallery()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(photoGallery.DOLocalMove(new Vector3(500, 75, 0), 0.25f));
        seq.Join(photoGallery.DOScale(new Vector3(2.5f, 2.5f, 0),0.25f));
        seq.AppendCallback(() => currentGalleryViewMode = 2);
        AdjustGalleryButtons(2);
    }
    public void CollapseGallery()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(photoGallery.DOLocalMove(new Vector3(730, 350, 0), 0.25f));
        seq.Join(photoGallery.DOScale(new Vector3(1f, 1f, 0),0.25f));
        seq.AppendCallback(() => currentGalleryViewMode = 0);
        AdjustGalleryButtons(0);
    }

    public void HideGallery()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(photoGallery.DOLocalMove(new Vector3(730, 640, 0), 0.25f));
        seq.AppendCallback(() => currentGalleryViewMode = 1);
        AdjustGalleryButtons(1);

    }
    public void ShowGallery()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(photoGallery.DOLocalMove(new Vector3(730, 350, 0), 0.25f));
        seq.AppendCallback(() => currentGalleryViewMode = 0);
        AdjustGalleryButtons(0);
    }

    #endregion
   
    
    
    #region Helpers

    void ChangePhoto(string photoText, Sprite photoSprite)
    {
        desiredPhotoText.text = photoText;
        desiredPhoto.sprite = photoSprite;
    }
    string GetCountTextValue(int number)
    {
        string result;
        switch (number)
        {
            case 2: 
                result = "Photo Two";
                break;
            case 3: 
                result = "Photo Three";
                break;
            case 4: 
                result = "Photo Four";
                break;
            case 5: 
                result = "Photo Five";
                break;
            default:
                result = "Photo One";
                break;
                
            
        }

        return result;
    }

    void AdjustGalleryButtons(int currentView)
    {
        switch (currentView)
        {
            case 0:
                if(hideButton != null)
                    hideButton.SetActive(true);
                if(expandButton != null)
                    expandButton.SetActive(true);
                if(showButton != null)
                    showButton.SetActive(false);
                break;
            case 1:
                if(hideButton != null)
                    hideButton.SetActive(false);
                if(expandButton != null)
                    expandButton.SetActive(false);
                if(showButton != null)
                    showButton.SetActive(true);
                break;
            case 2:
                if(hideButton != null)
                    hideButton.SetActive(false);
                if(expandButton != null)
                    expandButton.SetActive(true);
                if(showButton != null)
                    showButton.SetActive(false);
                break;
            default:
                if(hideButton != null)
                    hideButton.SetActive(false);
                if(expandButton != null)
                    expandButton.SetActive(true);
                if(showButton != null)
                    showButton.SetActive(true);
                break;
        }
    }
    

    #endregion
}
