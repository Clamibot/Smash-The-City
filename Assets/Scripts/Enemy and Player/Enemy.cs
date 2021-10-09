using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    [Header("Enemy")]
    public int attackPower;
    public float attackRange = 10f;

    //NavMesh Movement
    NavMeshAgent agent;

    //Reference to the player
    private PlayerCharacter player;

    // Start is called before the first frame update
    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        player = FindObjectOfType<PlayerCharacter>();
        agent.destination = transform.position;

        base.Start();
    }

    protected void Update()
    {
        if (player != null)
            GoTowardsPoint(player.transform.position);
        else
            GoToRandomPoint();

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < attackRange)
            Attack();
    }

    private void Attack()
    {
        //TODO
    }

    #region NavMesh Methods
    public void GoTowardsPoint(Vector3 point)
    {
        Vector3 vectorToPoint = point - transform.position;  //Get direction from self to point
        Vector3 vectorFromEnemy = vectorToPoint + transform.position;   //Get that same direction but move the origin to self

        NavMeshHit hit;
        if (NavMesh.SamplePosition(vectorFromEnemy, out hit, 5, ~NavMesh.GetAreaFromName("Walkable")))   //Get the closest point on the NavMesh
            agent.destination = hit.position;  //Set the navMeshAgent's destination
    }

    public void GoToRandomPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 100; //Get a random direction
        randomDirection += transform.position;  //Add it to our position 

        NavMeshHit hit;
        //Check if there is a close point on the navmesh
        if (NavMesh.SamplePosition(randomDirection, out hit, 5, ~NavMesh.GetAreaFromName("Walkable")))
        {
            agent.destination = hit.position;   //Set agent's destination
        }
        else
            GoToRandomPoint();
    }
    #endregion

}
