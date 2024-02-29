using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] private CinemachineVirtualCamera polaroidCamera;
    [SerializeField] private CinemachineVirtualCamera menuStartCamera;

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
        SwitchCamera(gameMode);
    }

    private void SwitchCamera(eGameMode cameraMode)
    {
        switch (cameraMode)
        {
            case eGameMode.Normal:
                playerFollowCamera.Priority = 10;
                polaroidCamera.Priority = 5;
                break;
            case eGameMode.Polaroid:
                playerFollowCamera.Priority = 5;
                polaroidCamera.Priority = 10;
                break;
        }
    }

    public void SwitchToGamePlayCamera()
    {
        menuStartCamera.Priority = 0;
    }
    
}

public enum eGameMode
{
    Normal,
    Polaroid
}
