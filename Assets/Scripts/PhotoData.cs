using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhotoData
{
    [SerializeField] private CameraTransform cameraTransform;
    private Texture2D savedTexture;

    public CameraTransform CameraTransform => cameraTransform;

    public Texture2D SavedTexture
    {
        get => savedTexture;
        set => savedTexture = value;
    }

    public PhotoData(CameraTransform cameraTransform, Texture2D savedTexture)
    {
        this.cameraTransform = cameraTransform;
        this.savedTexture = savedTexture;
    }
    
}

public interface IPhotoComparer
{
    public void ComparePhotos(PhotoData photo1, PhotoData photo2);
}

[Serializable]
public class CameraTransform
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;

    public Vector3 Position => position;

    public Quaternion Rotation => rotation;
    

    
    public CameraTransform(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}
