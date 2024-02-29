using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<eGameMode> OnChangeGameMode;
    
    [SerializeField] private PhotoCapture photoCapture;
    [SerializeField] private UiManager uiManager;

    [SerializeField] private List<CameraTransform> cameraPositions;
    public static GameManager Instance;
    
    private Texture2D desiredPhoto;
    private Texture2D takenPhoto;

    private int score;
    private int numberOfTakenPictures;

    private void Awake()
    {
        Instance = this;
    }
    
    private void OnEnable()
    {
        CameraController.Capture += OnCapture;
        PlayerCollision.PlayerDie += OnPlayerDie;
    }

    private void OnDisable()
    {
        CameraController.Capture -= OnCapture;
        PlayerCollision.PlayerDie -= OnPlayerDie;
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            await TakeRandomShot();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            AddToCameraPositions();
        }
    }


    private void AddToCameraPositions()
    {
        var cameraTransform = new CameraTransform(Camera.main.transform);
        cameraPositions.Add(cameraTransform);
    }
    
    private void OnPlayerDie()
    {
        GameOver();
    }

    private async void Start()
    {
        await TakeRandomShot();
    }
    
    public void ChangeGameMode(eGameMode gameMode)
    {
        OnChangeGameMode?.Invoke(gameMode);
    }

    private async Task TakeRandomShot()
    {
        ChangeGameMode(eGameMode.Polaroid);
        var tex = await photoCapture.CaptureRandomTexture(cameraPositions[0]);
        desiredPhoto = tex;
        uiManager.NormalViewUI.ShowDesiredPhoto(desiredPhoto);
        ChangeGameMode(eGameMode.Normal);
    }

    private async void OnCapture()
    {
        var tex = await photoCapture.CapturePhoto();
        takenPhoto = tex;
        uiManager.NormalViewUI.ShowPhoto(takenPhoto);
        ChangeGameMode(eGameMode.Normal);
        var res = TextureComparer.CompareTextures(desiredPhoto, takenPhoto);
        Debug.Log($"similarity percentage: {res}");
        if (res >= 0.7f)
        {
            numberOfTakenPictures++;
            score += (int)res * 100;
            if (!CheckForWin(numberOfTakenPictures))
                await TakeRandomShot();
            else
                Win();
        }
    }

    public void ShowWarning(WarningData warningData, bool show)
    {
        if (warningData != null)
            Debug.Log("Warning: Car approaching from direction " + warningData.Direction + "  Level: " +
                      warningData.WarningLevel + " Show: " + show);
    }

    public void ShowPhoto()
    {
        uiManager.NormalViewUI.ShowPhoto(desiredPhoto);
    }

    private bool CheckForWin(int value)
    {
        return value >= 5;
    }

    private void Win()
    {
        Debug.Log("Win");
    }

    private void GameOver()
    {
        Debug.Log("Lose");
    }
    
    
    
}

