using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    #region ExposedElements
    [SerializeField]
    private Image batteryFill;

    [SerializeField] private Image takenPhotoFrame;
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

    [SerializeField] private Transform warningPanel;
    [SerializeField] private Image upWarningSign;
    [SerializeField] private Image downWarningSign;
    [SerializeField] private Image rightWarningSign;
    [SerializeField] private Image leftWarningSign;

    [SerializeField] private Timer bonusTimer;
    
    [SerializeField] private RectTransform photoGallery;
    [SerializeField] private GameObject hideButton, expandButton, showButton;
    private int currentGalleryViewMode;
    //0:Default, 1:Hidden, 2:Expanded

    [SerializeField] private GameObject hud;
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private GameObject menuItems;
    #endregion

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScore;
    [SerializeField] private TextMeshProUGUI endPanelText;
    [SerializeField] private Button restartButton;
    private Vector2 initialPosition = new(927f, -172);
    private int lastScore;


    #region TestingElements

    private float fakeAccuracy;
    private float fakeTimer;
    private Sprite fakeTaken;
    private Sprite fakeDesired;

    private int currentMode;
    #endregion
   
    private void OnEnable()
    {
        GameManager.OnChangeGameMode += OnChangeGameMode;
        startButton.onClick.AddListener(StartGame);
        Timer.timerEnd += TimerOnTimerEnd;
        restartButton.onClick.AddListener(RestartScene);
    }
    

    private void OnDisable()
    {
        GameManager.OnChangeGameMode -= OnChangeGameMode;
        startButton.onClick.RemoveListener(StartGame);
        Timer.timerEnd -= TimerOnTimerEnd;
        restartButton.onClick.RemoveAllListeners();
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(0);
    }

    private void StartGame()
    {
        warningPanel.gameObject.SetActive(true);
        hud.gameObject.SetActive(true);
        startGamePanel.gameObject.SetActive(false);
        menuItems.gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void StartBonusTime()
    {
        ShowBonus();
        bonusTimer.StartTimer(30);
    }

    public void ShowGameOver()
    {
        endPanelText.text = $"GameOver";
        gameOverPanel.gameObject.SetActive(true);
        gameOverScore.text = $"{lastScore}";
    }

    public void ShowWinPanel()
    {
        endPanelText.text = $"Congratulations";
        gameOverPanel.gameObject.SetActive(true);
        gameOverScore.text = $"{lastScore}";
    }
    
    private void TimerOnTimerEnd()
    {
        HideBonus();
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
        // if(Input.GetKeyDown(KeyCode.Space))
        //     SwitchMode();
        // if(Input.GetKeyDown(KeyCode.M))
        //     ComparingPhotosVisually();
        if (Input.GetKeyDown(KeyCode.P))
            ShowBonus();
        if (Input.GetKeyDown(KeyCode.O))
            HideBonus();
    }
    

    public async Task ComparingPhotosVisually(bool success, Texture2D newTexture, float accuracy, float scoreValue)
    {
        Sequence seq = DOTween.Sequence();
        var photoSprite = CreateSprite(newTexture);
        takenPhoto.sprite = photoSprite;
        takenPhotoFrame.rectTransform.anchoredPosition = initialPosition;
        float valFloat = 0f; 
        seq.AppendCallback(()=> takenPhotoFrame.gameObject.SetActive(true));
        seq.Join(takenPhotoFrame.transform.DOScale(Vector3.one, 0.3f).From(Vector3.zero));
        //revisit 54 & 55 when we have a world canvas photo
        //also add From because taken photo position is change after check result
        seq.AppendInterval(0.1f);
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(true));
        seq.Join(accuracyOutline.DOFillAmount(1, 0.4f).From(0));
        seq.AppendCallback(()=> accuracyFiller.gameObject.SetActive(true));
        seq.AppendCallback(() => AudioManager.Instance.Play("Recharge"));
        seq.Append(accuracyFiller.DOFillAmount(accuracy, 3).From(0).SetEase(Ease.OutQuad));
        //switch endvalue with actual percentage value
        seq.Join(DOTween.To(() => valFloat, x => valFloat = x, accuracy * 100, 3f)
                .SetEase(Ease.OutQuad))
            .OnUpdate(()=>accuracyText.text = ((int)valFloat)+"%")
            .OnComplete(()=>CaptureResult(success, scoreValue));
        await seq.AsyncWaitForCompletion();
        //change check result with actual boolean
    }

    void CaptureResult(bool result, float scoreValue)
    {
        Sequence seq = DOTween.Sequence();
        float valFloat = lastScore; 
        if(scoreValue < 50)
            AudioManager.Instance.Play(("Fail"));
        else if(scoreValue >= 50 && scoreValue <= 75)
            AudioManager.Instance.Play("Success1");
        else
            AudioManager.Instance.Play("Success2");
        
        seq.Append(accuracyOutline.DOFillAmount(0, 0.15f));
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(false));
        seq.Join(accuracyFiller.DOFillAmount(0, 0.15f));
        seq.AppendCallback(()=> accuracyOutline.gameObject.SetActive(false));
        if (result)
        {
            seq.Append(takenPhotoFrame.transform.DOMove(score.transform.position, 1));
            seq.Join(takenPhotoFrame.transform.DOScale(0, 1));
            seq.AppendCallback(() => takenPhotoFrame.gameObject.SetActive(false));
            seq.Join(DOTween.To(() => valFloat, x => valFloat = x, scoreValue, 1.5f)
                    .SetEase(Ease.OutQuad))
                .OnUpdate(() => score.text = ((int)valFloat) + " Pts");
            lastScore = (int)scoreValue;

        }
        else
        {
            seq.Join(takenPhotoFrame.transform.DOScale(0, 0.7f));
            seq.AppendCallback(() => takenPhotoFrame.gameObject.SetActive(false));
        }
        
    }
    public void NextPhoto(int currentPhotoIndex,Texture2D texture)
    {
        var photoSprite = CreateSprite(texture);
        
        Sequence seq = DOTween.Sequence();
        string photoNumber = GetCountTextValue(currentPhotoIndex + 1);

        seq.Append(desiredPhotoText.DOFade(0, 0.5f));
        seq.Join(desiredPhoto.DOFade(0, 0.5f));
        seq.AppendCallback(() => ChangePhoto(photoNumber, photoSprite));
        seq.Append(desiredPhotoText.DOFade(1, 0.5f));
        seq.Join(desiredPhoto.DOFade(1, 0.5f));

    }

    private static Sprite CreateSprite(Texture2D texture)
    {
        return Sprite.Create(texture,
            new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    void SwitchMode(eGameMode gameMode)
    {
        Sequence seq = DOTween.Sequence();
        if (gameMode == eGameMode.Polaroid)
        {
            seq.Append(leftHUD.DOMoveX(-175, 0.3f));
            seq.Join(switcher.DOFade(0.3f, 0.3f)).SetEase(Ease.InQuad);
            seq.Join(cameraHUD.DOFade(1, 0.2f));
            seq.Append(switcher.DOFade(0f, 0.3f));
            currentMode = 1;
        }
        else if(gameMode == eGameMode.Normal)
        {
            seq.Append(leftHUD.DOMoveX(238, 0.3f));
            seq.Join(switcher.DOFade(0.3f, 0.3f)).SetEase(Ease.InQuad);
            seq.Join(cameraHUD.DOFade(0, 0.2f));
            seq.Append(switcher.DOFade(0f, 0.3f));
            currentMode = 0;
        }
    }

    private void ShowBonus()
    {
        bonusLabel.rectTransform.DOAnchorPosX(0, 0.25f).SetEase(Ease.OutBack);
    }

    private void HideBonus()
    {
        bonusLabel.rectTransform.DOAnchorPosX(-200, 0.25f).SetEase(Ease.OutBack);
    }
    
    private void OnChangeGameMode(eGameMode gameMode)
    {
        SwitchMode(gameMode);
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

    public void UpdateWarning(WarningData warningData, bool show)
    {
        if (!show)
        {
            warningPanel.gameObject.SetActive(false);
            return;
        }
        warningPanel.gameObject.SetActive(true);
        Color color;
        switch (warningData.WarningLevel)
        {
            case eWarningLevel.Low:
                color = Color.yellow;
                break;
            case eWarningLevel.Medium:
                color = new Color(1f, 0.5f, 0f); ;
                break;
            case eWarningLevel.High:
                color = Color.red;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        switch (warningData.Direction)
        {
            case eDirection.Up:
                upWarningSign.gameObject.SetActive(true);
                upWarningSign.color = color;
                upWarningSign.transform.DOScale(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                downWarningSign.gameObject.SetActive(false);
                leftWarningSign.gameObject.SetActive(false);
                rightWarningSign.gameObject.SetActive(false);
                break;
            case eDirection.Down:
                upWarningSign.gameObject.SetActive(false);
                downWarningSign.gameObject.SetActive(true);
                leftWarningSign.gameObject.SetActive(false);
                rightWarningSign.gameObject.SetActive(false);
                downWarningSign.color = color;
                downWarningSign.transform.DOScale(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                break;
            case eDirection.Left:
                upWarningSign.gameObject.SetActive(false);
                downWarningSign.gameObject.SetActive(false);
                leftWarningSign.gameObject.SetActive(true);
                rightWarningSign.gameObject.SetActive(false);
                leftWarningSign.color = color;
                leftWarningSign.transform.DOScale(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                break;
            case eDirection.Right:
                upWarningSign.gameObject.SetActive(false);
                downWarningSign.gameObject.SetActive(false);
                leftWarningSign.gameObject.SetActive(false);
                rightWarningSign.gameObject.SetActive(true);
                rightWarningSign.color = color;
                rightWarningSign.transform.DOScale(0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
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
