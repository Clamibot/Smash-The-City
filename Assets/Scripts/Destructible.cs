using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public float breakForce;
    public GameObject fracture;
    public GPUInstancer.GPUInstancerPrefabManager prefabManager;
    public GPUInstancer.GPUInstancerModificationCollider deinstancingSphere;
    public GPUInstancer.GPUInstancerModificationCollider deinstancingArea;
    private GPUInstancer.GPUInstancerPrefab allocatedGO;

    public void Start()
    {
        prefabManager = GameObject.Find("GPUI Prefab Manager").GetComponent<GPUInstancer.GPUInstancerPrefabManager>();
        deinstancingSphere = GameObject.Find("DeInstancingSphere").GetComponent<GPUInstancer.GPUInstancerModificationCollider>();
        deinstancingArea = GameObject.Find("DeInstancingArea").GetComponent<GPUInstancer.GPUInstancerModificationCollider>();
        allocatedGO = GetComponent<GPUInstancer.GPUInstancerPrefab>();
        fracture.transform.localScale = transform.localScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > breakForce)
        {
            deinstancingArea._enteredInstances.Remove(allocatedGO);
            deinstancingSphere._enteredInstances.Remove(allocatedGO);
            GPUInstancer.GPUInstancerAPI.RemovePrefabInstance(prefabManager, allocatedGO);
            Instantiate(fracture, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
