using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RandomSpawnerShape
{
    Box,
    Sphere,
}

namespace GPUInstancer
{
    public class BuildingSpawner : MonoBehaviour
    {
        [Header("Building Spawner")]
        [Range(0, 5000000)]
        public int objectCount = 50000;

        [Tooltip("The Inner Radius For The Prefabs To Spawn At.")]
        public float innerRadius;
        [Tooltip("The Outer Radius For The Prefabs To Spawn At.")]
        public float outerRadius;
        [Tooltip("The Minimum Height The Prefabs Should Spawn At. Leave This At 0 If Using The Sphere Spawn Shape Mode.")]
        public float minSpawnHeight;
        [Tooltip("The Maximum Height The Prefabs Should Spawn At. Leave This At 0 If Using The Sphere Spawn Shape Mode.")]
        public float maxSpawnHeight;
        [Tooltip("The Minimum Prefab Scale.")]
        public float minPrefabScale;
        [Tooltip("The Maximum Prefab Scale.")]
        public float maxPrefabScale;
        [Tooltip("The Minimum Rotation Angle For The Prefabs.")]
        public float minRotationAngle;
        [Tooltip("The Maximum Rotation Angle For The Prefabs.")]
        public float maxRotationAngle;

        [Tooltip("The Shape Of The Volume To Spawn Prefabs In.")]
        public RandomSpawnerShape spawnShape = RandomSpawnerShape.Sphere;

        [Tooltip("The List Of Objects To Spawn.")]
        public List<GPUInstancerPrefab> objects = new List<GPUInstancerPrefab>();
        public GPUInstancerPrefabManager prefabManager;
        public Transform centerTransform;

        private List<GPUInstancerPrefab> objectInstances = new List<GPUInstancerPrefab>();
        private int instantiatedCount;
        private Vector3 center;
        private Vector3 allocatedPos;
        private Quaternion allocatedRot;
        private Vector3 allocatedLocalEulerRot;
        private Vector3 allocatedLocalScale;
        private GPUInstancerPrefab allocatedGO;
        private GameObject goParent;
        private float allocatedLocalScaleFactor;

        private void Awake()
        {
            instantiatedCount = 0;
            center = centerTransform.position;
            allocatedPos = Vector3.zero;
            allocatedRot = Quaternion.identity;
            allocatedLocalEulerRot = Vector3.zero;
            allocatedLocalScale = Vector3.one;
            allocatedLocalScaleFactor = 1f;

            goParent = new GameObject("Objects");
            goParent.transform.position = center;
            goParent.transform.parent = gameObject.transform;

            objectInstances.Clear();

            for (int i = 0; i < objectCount; i++)
                objectInstances.Add(InstantiateObject());

            Debug.Log("Instantiated " + instantiatedCount + " objects.");
        }

        private void Start()
        {
            if (prefabManager != null && prefabManager.gameObject.activeSelf && prefabManager.enabled)
            {
                GPUInstancerAPI.RegisterPrefabInstanceList(prefabManager, objectInstances);
                GPUInstancerAPI.InitializeGPUInstancer(prefabManager);
            }
        }

        private GPUInstancerPrefab InstantiateObject()
        {
            // Create random position based on specified shape and range.
            if (spawnShape == RandomSpawnerShape.Box)
            {
                allocatedPos.x = Random.Range(innerRadius, outerRadius);
                allocatedPos.y = Random.Range(minSpawnHeight, maxSpawnHeight);
                allocatedPos.z = Random.Range(innerRadius, outerRadius);
            }
            else if (spawnShape == RandomSpawnerShape.Sphere)
                allocatedPos = Random.insideUnitSphere * Random.Range(innerRadius, outerRadius);

            allocatedRot = Quaternion.FromToRotation(Vector3.forward, center - allocatedPos);
            allocatedGO = Instantiate(objects[Random.Range(0, objects.Count)], allocatedPos, allocatedRot);
            allocatedGO.transform.parent = goParent.transform;

            allocatedLocalEulerRot.x = Random.Range(minRotationAngle, maxRotationAngle);
            allocatedLocalEulerRot.y = Random.Range(minRotationAngle, maxRotationAngle);
            allocatedLocalEulerRot.z = Random.Range(minRotationAngle, maxRotationAngle);
            allocatedGO.transform.localRotation = Quaternion.Euler(allocatedLocalEulerRot);

            allocatedLocalScaleFactor = Random.Range(minPrefabScale, maxPrefabScale);
            allocatedLocalScale.x = allocatedLocalScaleFactor;
            allocatedLocalScale.y = allocatedLocalScaleFactor;
            allocatedLocalScale.z = allocatedLocalScaleFactor;
            allocatedGO.transform.localScale = allocatedLocalScale;

            instantiatedCount++;

            return allocatedGO;
        }
    }
}
