using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using DG.Tweening;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;

public class CityTraffic : MonoBehaviour
{
    [System.Serializable]
    public class TrafficVehicle
    {
        public Transform vehicleTransform;
        public GameObject vehicleObject;
        public float moveSpeed;
    }

    public List<TrafficVehicle> trafficVehiclePrefabs;
    public List<PathDefinition> pathDefinitions;
    public float spawnInterval = 3f;

    void Start()
    {
       // StartCoroutine(InitializeTraffic());
        StartCoroutine(SpawnVehiclesContinuously());

    }

    IEnumerator InitializeTraffic()
    {
        foreach (var path in pathDefinitions)
        {
            yield return new WaitForSeconds(1.5f);
            TrySpawnVehicleOnPath(path);
        }
    }
    void TrySpawnVehicleOnPath(PathDefinition path)
    {
        if (path.ActiveVehicleCount() >= path.maxActiveVehicles)
            return;

        TrafficVehicle randomVehiclePrefab = trafficVehiclePrefabs[Random.Range(0, trafficVehiclePrefabs.Count)];
        List<Vector3> route = path.GetRoute();

        if (route.Count == 0)
            return;

        GameObject spawnedVehicle = LeanPool.Spawn(randomVehiclePrefab.vehicleObject, route[0], Quaternion.LookRotation(route[1] - route[0]));
        Transform spawnedTransform = spawnedVehicle.transform;

       

        TrafficVehicle newTrafficVehicle = new TrafficVehicle
        {
            vehicleObject = spawnedVehicle,
            vehicleTransform = spawnedTransform,
            moveSpeed = randomVehiclePrefab.moveSpeed
        };

        path.AddVehicle(newTrafficVehicle);


        float travelTime = route.Count / newTrafficVehicle.moveSpeed;

        Tween tween = spawnedTransform
            .DOPath(route.ToArray(), travelTime, PathType.CatmullRom, PathMode.Full3D)
            .SetEase(Ease.InOutSine)
            .SetLookAt(0.01f, true)
            .SetAutoKill(false)
            .OnComplete(() =>
            {
                LeanPool.Despawn(spawnedVehicle);
                path.RemoveVehicle(newTrafficVehicle);
                TrySpawnVehicleOnPath(path);
            });

        TrafficObject trafficObject = spawnedVehicle.GetComponent<TrafficObject>();
        if (trafficObject != null)
        {
            trafficObject.movementTween = tween;
        }
    }

    IEnumerator SpawnVehiclesContinuously()
    {
        while (true)
        {
            //yield return new WaitForSeconds(spawnInterval);

            /*
            float randomSpawn = spawnInterval * Random.Range(0.7f, 1.3f);
            yield return new WaitForSeconds(randomSpawn);
            */
            yield return WaitWithSpawnLogic(spawnInterval);

            foreach (var path in pathDefinitions)
            {
                TrySpawnVehicleOnPath(path);
            }
        }
    }
    int spawnCallCount = 0;

    IEnumerator WaitWithSpawnLogic(float spawnInterval)
    {
        spawnCallCount++;

        float waitTime = spawnCallCount switch
        {
            1 => 1f,
            2 => 1f,
            3 => spawnInterval,
            _ => spawnInterval * Random.Range(0.7f, 1.3f)
        };

        if (spawnCallCount >= 4)
            spawnCallCount = 0;

        yield return new WaitForSeconds(waitTime);
    }
}

[System.Serializable]
public class PathDefinition 
{
    public Transform pathParent;
    public int maxActiveVehicles = 3;

    private List<Vector3> routePoints = new List<Vector3>();
    private List<CityTraffic.TrafficVehicle> activeVehicles = new List<CityTraffic.TrafficVehicle>();


    public void CollectRoute()
    {
        routePoints.Clear();
        for (int i = 0; i < pathParent.childCount; i++)
        {
            routePoints.Add(pathParent.GetChild(i).position);
        }
    }

    public List<Vector3> GetRoute()
    {
        if (routePoints == null || routePoints.Count == 0)
        {
            CollectRoute();
        }
        return routePoints;
    }

    public void AddVehicle(CityTraffic.TrafficVehicle vehicle)
    {
        if (!activeVehicles.Contains(vehicle))
        {
            activeVehicles.Add(vehicle);
        }
    }

    public void RemoveVehicle(CityTraffic.TrafficVehicle vehicle)
    {
        if (activeVehicles.Contains(vehicle))
        {
            activeVehicles.Remove(vehicle);
        }
    }

    public int ActiveVehicleCount()
    {
        return activeVehicles.Count;
    }
}