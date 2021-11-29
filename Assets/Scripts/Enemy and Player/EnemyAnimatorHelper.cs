using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimatorHelper : MonoBehaviour
{
    private Enemy enemy;

    // Start is called before the first frame update
    void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }


    public void turnOnAttack()
    {
        //enemy.canHurtPlayer = true;
        //Debug.Log("attacking "+ enemy.canHurtPlayer);
    }

    public void turnOffAttack()
    {
        //enemy.canHurtPlayer = false;
        //Debug.Log("attacking " + enemy.canHurtPlayer);
    }

    public void attemptAttack()
    {
        enemy.AttemptAttack();
    }

    public void timeToShrink()
    {
        enemy.timeToShrinkAndDie = true;
    }
}
