using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    //[Header("Projectile")]
    //public int range;
    //public float lifetime;
    private float timeAlive = 0;
    private Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale;
        //Destroy(gameObject, lifetime);  //TODO May need to be changed (based on how we pause the game)
        Destroy(gameObject, 3.01f);
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        transform.localScale = Vector3.Lerp(startScale, Vector3.zero, timeAlive / 3);
    }

    //TODO implement moving and range
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Enemy")
            Destroy(gameObject);
    }
}
