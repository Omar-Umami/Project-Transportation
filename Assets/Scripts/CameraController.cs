using System;
using System.Threading.Tasks;
using StarterAssets;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs input;
    public static event Action Capture;
    private bool capturing = false;

    private void Update()
    {
        GameManager.Instance.ForceChangeGameMode(input.aim ? eGameMode.Polaroid : eGameMode.Normal);

        if (!input.aim || !input.capture) return;
        if (!capturing)
        {
            capturing = true;
            Capture?.Invoke();
            input.capture = false;
            input.aim = false;
            GameManager.Instance.ForceChangeGameMode(eGameMode.Normal);
            capturing = false;
        }
        input.capture = false;

    }
    
}
