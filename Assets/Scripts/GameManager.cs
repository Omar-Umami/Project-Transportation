using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool editorMode;
    public static event Action<eGameMode> OnChangeGameMode;
    
    [SerializeField] private PhotoCapture photoCapture;
    [SerializeField] private UI_Manager uiManager;

    private List<CameraTransform> cameraPositions;
    public static GameManager Instance;
    
    private Texture2D desiredPhoto;
    private Texture2D takenPhoto;

    private int score;
    private int numberOfTakenPictures;

    [SerializeField] private CameraPositionsSO cameraPositionsSo;

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
        
        if (Input.GetKeyDown(KeyCode.H) && editorMode)
        {
            AddToCameraPositions();
        }
        if (Input.GetKeyDown(KeyCode.F) && editorMode)
        {
            AddToCameraPositions(true);
        }
    }


    private void AddToCameraPositions(bool forced = false)
    {
        var cameraTransform = new CameraTransform(Camera.main.transform);
        if (forced)
            cameraPositionsSo.ForcedPositionsTransforms.Add(cameraTransform);
        else 
            cameraPositionsSo.CameraTransforms.Add(cameraTransform);
    }
    
    private void OnPlayerDie()
    {
        GameOver();
    }

    private async void Start()
    {
        if (!editorMode)
        {
            cameraPositions = cameraPositionsSo.GetRandomPositions(5);
            await TakeRandomShot();
        }
    }
    
    public void ChangeGameMode(eGameMode gameMode)
    {
        OnChangeGameMode?.Invoke(gameMode);
    }

    private async Task TakeRandomShot()
    {
        ChangeGameMode(eGameMode.Polaroid);
        var tex = await photoCapture.CaptureRandomTexture(cameraPositions[0]);
        cameraPositions.RemoveAt(0);
        desiredPhoto = tex;
        uiManager.NextPhoto(numberOfTakenPictures, desiredPhoto);
        ChangeGameMode(eGameMode.Normal);
    }

    private async void OnCapture()
    {
        var tex = await photoCapture.CapturePhoto();
        takenPhoto = tex;
        ChangeGameMode(eGameMode.Normal);
        var res = TextureComparer.CompareTextures(desiredPhoto, takenPhoto);
        if (res >= 0.60f)
        {
            numberOfTakenPictures++;
            score += (int)(res * 100);
            
                await uiManager.ComparingPhotosVisually(true, takenPhoto, res, score);
            if (!CheckForWin(numberOfTakenPictures))
            {
                await TakeRandomShot();
            }
            else
                Win();
        }
        else
        {
            await uiManager.ComparingPhotosVisually(false, takenPhoto, res, score);
        }
    }

    public void ShowWarning(WarningData warningData, bool show)
    {
        uiManager.UpdateWarning(warningData, show);
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

