using System;
using System.Collections.Generic;
using System.Linq;
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
        
        var histogram1 = ComputeHistogram(pixels1);
        var histogram2 = ComputeHistogram(pixels2);
        
        var intersection = ComputeHistogramIntersection(histogram1, histogram2);
        
        var scalingFactor = 1f;
        if (intersection < 0.6f)
        {
            scalingFactor = Mathf.Lerp(1f, 0.3f, intersection / 0.6f);
        }
        
        var scaledIntersection = intersection * scalingFactor;

        return scaledIntersection;
    }


    private static float[] ComputeHistogram(IEnumerable<Color> pixels)
    {
        var histogram = new float[256];

        foreach (var color in pixels)
        {
            var luminance = (color.r + color.g + color.b + color.a) / 4f;
            histogram[(int)(luminance * 255)]++;
        }

        return histogram;
    }

    private static float ComputeHistogramIntersection(IReadOnlyList<float> histogram1, IReadOnlyList<float> histogram2)
    {
        var intersection = histogram1.Select((t, i) => Mathf.Min(t, histogram2[i])).Sum();
        
        intersection /= Mathf.Min(histogram1.Sum(), histogram2.Sum());

        return intersection;
    }

    

    // float ColorDistance(Color c1, Color c2)
    // {
    //     return Mathf.Sqrt(Mathf.Pow(c1.r - c2.r, 2) +
    //                       Mathf.Pow(c1.g - c2.g, 2) +
    //                       Mathf.Pow(c1.b - c2.b, 2) +
    //                       Mathf.Pow(c1.a - c2.a, 2));
    // }
}
