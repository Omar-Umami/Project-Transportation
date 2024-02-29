using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class TextureComparer 
{ 
    public static float CompareTextures(Texture2D texture1, Texture2D texture2)
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
        else 
        {
            intersection += Random.Range(0.1f, 0.2f);
        }

        intersection = Mathf.Min(intersection, 100);
        
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
