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

    #endregion


    #region TestingElements

    private float fakeAccuracy;
    private float fakeTimer;
    private Sprite fakeTaken;
    private Sprite fakeDesired;
    

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Invoke("ComparingPhotosVisually",1); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ComparingPhotosVisually()
    {
        Sequence seq = DOTween.Sequence();
        float valFloat = 0f; 
        seq.AppendCallback(()=> takenPhoto.gameObject.SetActive(true));
        seq.Join(takenPhoto.transform.DOScale(Vector3.one, 0.3f).From(Vector3.zero));
        //revisit 54 & 55 when we have a world canvas photo
        //also add From because taken photo position is change after check result
        seq.AppendInterval(0.1f);
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(true));
        seq.Join(accuracyOutline.DOFillAmount(1, 0.4f).From(0));
        seq.AppendCallback(()=> accuracyFiller.gameObject.SetActive(true));
        seq.Append(accuracyFiller.DOFillAmount(1, 3).From(0).SetEase(Ease.OutQuad));
        //switch endvalue with actual percentage value
        seq.Join(DOTween.To(() => valFloat, x => valFloat = x, 100f, 3f)
                .SetEase(Ease.OutQuad))
            .OnUpdate(()=>accuracyText.text = ((int)valFloat)+"%")
            .OnComplete(()=>CheckResult(true));
    //change check result with actual boolean
    }

    void CheckResult(bool result)
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
    

    #endregion
}
