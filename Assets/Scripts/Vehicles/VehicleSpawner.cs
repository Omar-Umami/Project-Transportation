using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class VehicleSpawner : MonoBehaviour
{
    public List<Vehicle> carPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 2f; // Adjust this value as needed
    private ObjectPool<Vehicle> vehiclePool;

    public ObjectPool<Vehicle> VehiclePool => vehiclePool;

    public static VehicleSpawner Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        vehiclePool = new ObjectPool<Vehicle>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, true,
            defaultCapacity: 2, maxSize: 2);
        InvokeRepeating(nameof(SpawnCar), 0f, spawnInterval);
    }

    private void ActionOnDestroy(Vehicle vehicle)
    {
        Destroy(vehicle.gameObject);
    }

    private void ActionOnRelease(Vehicle vehicle)
    {
        vehicle.gameObject.SetActive(false);
    }

    private void ActionOnGet(Vehicle vehicle)
    {
        var randomIndex = Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[randomIndex];
        var transform1 = vehicle.transform;
        transform1.position = spawnPoint.position;
        transform1.rotation = spawnPoint.rotation;
        vehicle.gameObject.SetActive(true);
    }

    private Vehicle CreateFunc()
    {
        var randomIndex = Random.Range(0, spawnPoints.Length);
        var spawnPoint = spawnPoints[randomIndex];
        var newCar = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Count)], spawnPoint.position, spawnPoint.rotation);
        return newCar;
    }

    private void SpawnCar()
    {
        vehiclePool.Get();
    }
}
