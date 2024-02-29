using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CameraPositions", order = 1)]
public class CameraPositionsSO : ScriptableObject
{
    [SerializeField] private List<CameraTransform> cameraTransforms;

    [SerializeField] private List<CameraTransform> forcedPositionsTransforms;

    public List<CameraTransform> CameraTransforms => cameraTransforms;

    public List<CameraTransform> ForcedPositionsTransforms => forcedPositionsTransforms;

    public List<CameraTransform> GetRandomPositions(int count)
    {
        var randomPositions = new List<CameraTransform>();
        
        if (count > cameraTransforms.Count)
        {
            Debug.LogWarning("Count is greater than the number of available positions.");
            count = cameraTransforms.Count;
        }
        
        var shuffledList = new List<CameraTransform>(cameraTransforms);
        shuffledList.Shuffle();

        var shuffledForcedList = new List<CameraTransform>(forcedPositionsTransforms);
        shuffledForcedList.Shuffle();
        
        for (var i = 0; i < count - 1; i++)
        {
            randomPositions.Add(shuffledList[i]);
        }
        
        randomPositions.Add(shuffledForcedList[0]);

        return randomPositions;
    }
} 

public static class ListExtensions
{
    private static System.Random _random = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = _random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
