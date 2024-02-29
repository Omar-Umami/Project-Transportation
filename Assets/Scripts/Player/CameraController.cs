using System;
using System.Threading.Tasks;
using StarterAssets;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs input;
    [SerializeField] private Animator animator;
    
    public static event Action Capture;
    private bool capturing = false;
    private static readonly int TakingPhoto = Animator.StringToHash("TakingPhoto");
    private static readonly int TakePhoto = Animator.StringToHash("TakePhoto");

    private void Update()
    {
        if (GameManager.Instance.gameState != eGameState.GamePlay) return;
        animator.SetBool(TakingPhoto, input.aim);

        if (!input.aim || !input.capture) return;
        if (!capturing)
        {
            capturing = true;
            // animator.SetTrigger(TakePhoto);
            Capture?.Invoke();
            input.capture = false;
            input.aim = false;
            capturing = false;
        }
        input.capture = false;

    }

    public void OnCameraMode()
    {
        GameManager.Instance.ChangeGameMode(eGameMode.Polaroid);
    }

    public void OnNormalMode()
    {
        GameManager.Instance.ChangeGameMode(eGameMode.Normal);
    }

    public void OnPrint()
    {
        
    }
    
}
