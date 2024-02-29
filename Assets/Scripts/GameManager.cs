using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarterAssets;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private bool editorMode;
    [SerializeField] private StarterAssetsInputs input;
    public static event Action<eGameMode> OnChangeGameMode;
    
    [SerializeField] private PhotoCapture photoCapture;
    [SerializeField] private UI_Manager uiManager;
    [SerializeField] private CameraManager cameraManager;

    private List<CameraTransform> cameraPositions;
    public static GameManager Instance;
    private readonly List<Texture2D> textures = new();
    
    private Texture2D desiredPhoto;
    private Texture2D takenPhoto;

    private int score;
    private int numberOfTakenPictures;

    private int bonusMultiplier = 1;
    

    [SerializeField] private CameraPositionsSO cameraPositionsSo;

    public eGameState gameState;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
    
    private void OnEnable()
    {
        CameraController.Capture += OnCapture;
        PlayerCollision.PlayerDie += OnPlayerDie;
        Timer.timerEnd += OnTimerEnd;
    }

    private void OnDisable()
    {
        CameraController.Capture -= OnCapture;
        PlayerCollision.PlayerDie -= OnPlayerDie;
        Timer.timerEnd -= OnTimerEnd;
    }
    
    private void OnTimerEnd()
    {
        bonusMultiplier = 1;
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
            // cameraPositions = cameraPositionsSo.GetRandomPositions(5);
            for (var index = 0; index < 10; index++)
            {
                textures.Add(await GetTexture());
            }

            await TakeRandomShot();
        }
    }
    
    public void ChangeGameMode(eGameMode gameMode)
    {
        OnChangeGameMode?.Invoke(gameMode);
    }

    private async Task TakeRandomShot(bool isLastOne = false)
    {
        ChangeGameMode(eGameMode.Polaroid);
        Texture2D tex;
        if (!isLastOne)
        {
            tex = textures[0];
            textures.RemoveAt(0);
        }
        else
        {
            tex = await photoCapture.CaptureRandomTexture(cameraPositionsSo.ForcedPositionsTransforms[0]);
        }
        desiredPhoto = tex;
        uiManager.NextPhoto(numberOfTakenPictures, tex);
        ChangeGameMode(eGameMode.Normal);
    }

    private async Task<Texture2D> GetTexture()
    {
        var tex = await photoCapture.CaptureRandomTexture();
        return tex;
    }

    private async void OnCapture()
    {
        AudioManager.Instance.Play("Capture");
        var tex = await photoCapture.CapturePhoto();
        takenPhoto = tex;
        ChangeGameMode(eGameMode.Normal);
        var res = TextureComparer.CompareTextures(desiredPhoto, takenPhoto);
        if (res >= 0.60f)
        {
            numberOfTakenPictures++;
            score += (int)(res * 100) * bonusMultiplier;

            await uiManager.ComparingPhotosVisually(true, takenPhoto, res, score);
            uiManager.StartBonusTime();
            bonusMultiplier = 2;
            if (!CheckForWin(numberOfTakenPictures))
            {
                await TakeRandomShot(numberOfTakenPictures == 4);
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
        gameState = eGameState.End;
        Cursor.lockState = CursorLockMode.None;
        uiManager.ShowWinPanel();
    }

    private void GameOver()
    {
        gameState = eGameState.End;
        Cursor.lockState = CursorLockMode.None;
        uiManager.ShowGameOver();
    }

    public void StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        input.capture = false;
        gameState = eGameState.GamePlay;
        cameraManager.SwitchToGamePlayCamera();
    }
    
}

public enum eGameState
{
    Start,
    GamePlay,
    End
}

