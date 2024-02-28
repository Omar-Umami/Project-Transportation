using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<eGameMode> OnChangeGameMode;
    
    [SerializeField] private PhotoCapture photoCapture;
    [SerializeField] private TextureComparer textureComparer;

    public static GameManager Instance;
    
    private Texture2D desiredPhoto;
    private Texture2D takenPhoto;

    private void OnEnable()
    {
        CameraController.Capture += OnCapture;
    }

    private void OnDisable()
    {
        CameraController.Capture -= OnCapture;
    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            await TakeRandomShot();
        }
    }


    private async void Start()
    {
        await TakeRandomShot();
    }

    private async Task TakeRandomShot()
    {
        ForceChangeGameMode(eGameMode.Polaroid);
        var tex = await photoCapture.CaptureRandomTexture();
        desiredPhoto = tex;
        NormalViewUI.Instance.ShowDesiredPhoto(desiredPhoto);
        ForceChangeGameMode(eGameMode.Normal);
    }

    private async void OnCapture()
    {
        var tex = await photoCapture.CapturePhoto();
        takenPhoto = tex;
        NormalViewUI.Instance.ShowPhoto(takenPhoto);
        var res = textureComparer.CompareTextures(desiredPhoto, takenPhoto);
        Debug.Log($"similarity percentage: {res}");
        if (res >= 0.7f)
        {
            await TakeRandomShot();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ForceChangeGameMode(eGameMode gameMode)
    {
        OnChangeGameMode?.Invoke(gameMode);
    }
}
