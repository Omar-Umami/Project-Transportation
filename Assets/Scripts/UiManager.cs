using System;
using System.Threading.Tasks;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private NormalViewUI normalViewUI;
    [SerializeField] private CameraViewUI polaroidCameraViewUI;

    public NormalViewUI NormalViewUI => normalViewUI;

    public CameraViewUI PolaroidCameraViewUI => polaroidCameraViewUI;

    private void OnEnable()
    {
        GameManager.OnChangeGameMode += OnChangeGameMode;
    }

    private void OnDisable()
    {
        GameManager.OnChangeGameMode -= OnChangeGameMode;
    }

    private void OnChangeGameMode(eGameMode gameMode)
    {
        SwitchMode(gameMode);
    }

    private void SwitchMode(eGameMode gameMode)
    {
        switch (gameMode)
        {
            case eGameMode.Normal:
                normalViewUI.gameObject.SetActive(true);
                polaroidCameraViewUI.gameObject.SetActive(false);
                break;
            case eGameMode.Polaroid:
                normalViewUI.gameObject.SetActive(false);
                polaroidCameraViewUI.gameObject.SetActive(true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }
    
    
}
