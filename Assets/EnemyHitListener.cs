using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitListener : MonoBehaviour
{
    public Enemy enemy;

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            Debug.Log("Hit Listener " + enemy.gameObject.name + " : hit value " + collision.relativeVelocity.magnitude);
            enemy.GetHurt(collision.relativeVelocity.magnitude);
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Attack")
        {
            Debug.Log("Hit Listener " + enemy.gameObject.name + " : hit value " + 10);
            enemy.GetHurt(10);
        }
    }
}
