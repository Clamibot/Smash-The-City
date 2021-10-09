using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile")]
    public int range;
    public float lifetime;

    // Start is called before the first frame update
    void Start()
    {

        Destroy(gameObject, lifetime);  //TODO May need to be changed (based on how we pause the game)
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //TODO implement moving and range
}
