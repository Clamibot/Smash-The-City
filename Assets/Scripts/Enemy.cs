using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    public enum EnemyState { IdleMotionless, IdleWalking, RunningFromSomething, GoingTowardsSomething, Attacking}

    [HideInInspector] public EnemyState state;

    [Header("Enemy")]
    [SerializeField, Range(0, 10)] private float minIdleTime = 1f;
    [SerializeField, Range(0, 10)] private float maxIdleTime = 4f;


    //NavMesh Movement
    NavMeshAgent agent;
    [SerializeField] private float idleWalkDistance = 1f;

    float timeToStopGoingTowardsSomething;

    // Start is called before the first frame update
    protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        state = EnemyState.IdleWalking;
        agent.destination = transform.position;
    }

    protected void Update()
    {
        //If we aren't sitting still and have reached the target...
        if (state != EnemyState.IdleMotionless && agent.remainingDistance < agent.stoppingDistance)
        {
            if (state == EnemyState.GoingTowardsSomething)
            {
                state = EnemyState.Attacking;

            }
            else if (state == EnemyState.RunningFromSomething || state == EnemyState.IdleWalking)    //Go to motionless
            {
                BecomeIdle();
            }
        }

        if (state == EnemyState.GoingTowardsSomething && Time.time > timeToStopGoingTowardsSomething)
            BecomeIdle();
    }

    public void BecomeIdle()
    {
        agent.destination = transform.position;

        state = EnemyState.IdleMotionless;

        //Timer till go to random point (Idle for a psuedo random amount of time)
        StartCoroutine(IdleMotionlessTimer(Random.Range(minIdleTime, maxIdleTime)));
    }

    public IEnumerator IdleMotionlessTimer(float timeToSit)
    {
        float time = 0;
        while (time < timeToSit)
        {
            time += Time.deltaTime;
            yield return null;
        }

        GoToRandomPoint();
    }

    public void GoToRandomPoint()
    {
        //Only go to a random point if we aren't doing anything
        if (state == EnemyState.IdleMotionless)
        {
            Vector3 randomDirection = Random.insideUnitSphere * idleWalkDistance; //Get a random direction
            randomDirection += transform.position;  //Add it to our position 

            NavMeshHit hit;
            //Check if there is a close point on the navmesh
            if (NavMesh.SamplePosition(randomDirection, out hit, 5, ~NavMesh.GetAreaFromName("Walkable")))
            {
                state = EnemyState.IdleWalking;

                agent.destination = hit.position;   //Set agent's destination
            }
            else
                GoToRandomPoint();
        }
    }

    #region Monkey Interuptions

    public void GoTowardsPoint(Vector3 point)
    {
        if (state != EnemyState.RunningFromSomething || state != EnemyState.Attacking)
        {
            StopCoroutine("IdleMotionlessTimer");

            //If we are currently going to another source of food, and our current destination is closer, then do not go to the new point
            if (state == EnemyState.GoingTowardsSomething && agent.remainingDistance < Vector3.Distance(point, transform.position))
                return;

            timeToStopGoingTowardsSomething = Time.time + 5f;


            state = EnemyState.GoingTowardsSomething;

            Vector3 directionOfPoint = point - transform.position;  //Get direction from self to point
            Vector3 directionFromPrimate = directionOfPoint + transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(directionFromPrimate, out hit, 5, ~NavMesh.GetAreaFromName("Walkable")))   //Get the closest point on the NavMesh
                agent.destination = hit.position;  //Set the navMeshAgent's destination
        }
    }

    public void RunFromPoint(Vector3 point, float runDistance)
    {
        StopCoroutine("IdleMotionlessTimer");


        state = EnemyState.RunningFromSomething;

        Vector3 directionOfPoint = point - transform.position;  //Get direction from self to point
        Vector3 oppositeDirection = -directionOfPoint * runDistance;
        Vector3 directionFromPrimate = oppositeDirection + transform.position;

        NavMeshHit hit;

        //Check if there is a close point on the navmesh
        if (NavMesh.SamplePosition(directionFromPrimate, out hit, 5, ~NavMesh.GetAreaFromName("Walkable")))
            agent.destination = hit.position;   //Set agent's destination
        else    //Try a closer point
            RunFromPoint(point, runDistance * 0.5f);
    }

    #endregion
}
