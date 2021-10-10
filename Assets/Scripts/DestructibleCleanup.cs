using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleCleanup : MonoBehaviour
{
    public float cleanupTimer;

    [SerializeField]
    private float timePassed;

    private void Start()
    {
        timePassed = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed += Time.deltaTime;

        if (timePassed > cleanupTimer)
            Destroy(gameObject);
    }
}
