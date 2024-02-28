using System;
using UnityEngine;

public class TextureComparer : MonoBehaviour
{
    [SerializeField] private float tolerance = 0.01f;

    public static TextureComparer Instance;

    private void Awake()
    {
        Instance = this;
    }

    // public float ComparePhotos(Texture2D photo1, Texture2D photo2)
    // {
    //     var pixels1 = photo1.GetPixels();
    //     var pixels2 = photo2.GetPixels();
    //
    //     var totalPixels = pixels1.Length;
    //     var matchingPixels = 0;
    //
    //     for (var i = 0; i < totalPixels; i++)
    //     {
    //         if (ColorCloseEnough(pixels1[i], pixels2[i], tolerance))
    //         {
    //             matchingPixels++;
    //         }
    //     }
    //
    //     var percentageSimilarity = (float)matchingPixels / totalPixels * 100f;
    //     return percentageSimilarity;
    // }
    
    public float CompareTextures(Texture2D texture1, Texture2D texture2)
    {
        var pixels1 = texture1.GetPixels();
        var pixels2 = texture2.GetPixels();

        var matchingPixels = 0;
        var totalPixels = Mathf.Min(pixels1.Length, pixels2.Length);

        for (var i = 0; i < totalPixels; i++)
        {
            if (pixels1[i] == pixels2[i])
            {
                matchingPixels++;
            }
        }

        var percentageSimilarity = (float)matchingPixels / totalPixels * 100f;
        Debug.Log("Percentage Similarity: " + percentageSimilarity.ToString("F2") + "%");
        return percentageSimilarity;
    }

    float ColorDistance(Color c1, Color c2)
    {
        return Mathf.Sqrt(Mathf.Pow(c1.r - c2.r, 2) +
                          Mathf.Pow(c1.g - c2.g, 2) +
                          Mathf.Pow(c1.b - c2.b, 2) +
                          Mathf.Pow(c1.a - c2.a, 2));
    }
}
