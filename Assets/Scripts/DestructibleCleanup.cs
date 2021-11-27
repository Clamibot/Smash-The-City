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
        cleanupTimer += Random.Range(0.0f, 1.0f); // Having everything disappear at once is boring. This creates a cool cleanup effect.
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timePassed += Time.deltaTime;

        if (timePassed > cleanupTimer)
            Destroy(gameObject);
    }
}
