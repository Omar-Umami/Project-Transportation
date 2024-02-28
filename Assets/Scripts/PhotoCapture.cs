using System;
using System.IO;
using System.Threading.Tasks;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;


public class PhotoCapture : MonoBehaviour
{
    private Camera cameraToCapture;
    
    private void Awake()
    {
        cameraToCapture = Camera.main;
    }
    
    public async Task<Texture2D> CapturePhoto()
    {
        var screenCapture = new Texture2D(cameraToCapture.pixelWidth, cameraToCapture.pixelHeight, TextureFormat.RGBA32, false);
        var screenTexture = new RenderTexture(Screen.width, Screen.height, 16);
        var currentRT = RenderTexture.active;
        cameraToCapture.targetTexture = screenTexture;
        RenderTexture.active = screenTexture;
        cameraToCapture.Render();
        RenderTexture.active = cameraToCapture.targetTexture;

        screenCapture.ReadPixels(new Rect(0, 0, cameraToCapture.pixelWidth, cameraToCapture.pixelHeight), 0, 0);
        screenCapture.Apply();

        RenderTexture.active = currentRT;
        cameraToCapture.targetTexture = null;
        
        // var photoData = new PhotoData(new CameraTransform(cameraToCapture.transform), screenCapture);
        return screenCapture;
    }
    
    [ContextMenu("TakeRandomPhoto")]
    public async Task<Texture2D> CaptureRandomTexture()
    {
        const float captureRadius = 20f;
        var transform1 = cameraToCapture.transform;
        var position = transform1.position;
        var height = position.y;
        
        var randomPosition = position + Random.insideUnitSphere * captureRadius;
        randomPosition.y = height; 
        
        var randomRotation = Quaternion.Euler(Random.Range(-20f, 20f), Random.Range(0f, 10f), 0f);
        
        transform1.position = randomPosition;
        // transform2.rotation = randomRotation;
        var desiredPhoto = await CapturePhoto();
        
        
        // Debug.Log($"{randomPosition}");
        
        //desiredPhoto = new PhotoData(new CameraTransform(cameraToCapture.transform), desiredTex);
        return desiredPhoto;
    }
    
    public void SaveTextureAsPNG(Texture2D texture, string filePath)
    {
        byte[] textureBytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, textureBytes);
        Debug.Log("Texture saved to: " + filePath);
    }

    
}