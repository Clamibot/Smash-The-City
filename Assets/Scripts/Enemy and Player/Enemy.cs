using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    [Header("Enemy")]
    public Animator anim;
    public int attackPower;
    public float attackRange = 10f;
    public GameObject enemyStats;

    [Header("GPU Stuff")]
    public GPUInstancer.GPUInstancerPrefabManager prefabManager;
    public GPUInstancer.GPUInstancerModificationCollider deinstancingSphere;
    public GPUInstancer.GPUInstancerModificationCollider deinstancingArea;
    private GPUInstancer.GPUInstancerPrefab allocatedGO;

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

        //prefabManager = GameObject.Find("GPUI Prefab Manager").GetComponent<GPUInstancer.GPUInstancerPrefabManager>();
        //deinstancingSphere = GameObject.Find("DeInstancingSphere").GetComponent<GPUInstancer.GPUInstancerModificationCollider>();
        //deinstancingArea = GameObject.Find("DeInstancingArea").GetComponent<GPUInstancer.GPUInstancerModificationCollider>();
        //allocatedGO = GetComponent<GPUInstancer.GPUInstancerPrefab>();
        //fracture.transform.localScale = transform.localScale;
        enemyStats.SetActive(false);
    }

    protected void Update()
    {
        if (player != null)
            GoTowardsPoint(player.transform.position);
        else if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance)
            GoToRandomPoint();

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            if (anim != null)
                anim.SetTrigger("Attack");
            Attack();
        }

        if (shieldBar != null && shields < maxShields)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer > rechargeDelay)
            {
                shields += rechargeRate * Time.deltaTime;
                if (shieldRenderer != null)
                    shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f + (shields / maxShields) * 1.2f);
                if (shields == maxShields)
                {
                    enemyStats.SetActive(false);
                }

                UpdateBars();
            }
        }

        if (shieldRenderer != null)
            shieldRenderer.material.SetColor("Color_63659391", new Color(1.0f - (shields * 2 / 255), shields / 255, shields * 2 / 255));
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            rechargeTimer = 0;
            enemyStats.SetActive(true);

            //Sheild or Health damage
            if (shields > 0)
            {
                shields -= collision.impulse.magnitude / damageResistance;

                if (shields <= 0)
                {
                    shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f);
                    shields = 0;
                }
            }
            else
            {
                if (anim != null)
                    anim.SetTrigger("Hurt");
                health -= collision.impulse.magnitude / damageResistance;
            }

            //Does the creature die?
            if (health <= 0)
            {
                health = 0;

                deinstancingArea._enteredInstances.Remove(allocatedGO);
                deinstancingSphere._enteredInstances.Remove(allocatedGO);
                GPUInstancer.GPUInstancerAPI.RemovePrefabInstance(prefabManager, allocatedGO);

                if (anim != null)
                    anim.SetTrigger("Die");
                base.Die();
            }

            UpdateBars();
        }
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
        Vector3 randomDirection = Random.insideUnitSphere * 10; //Get a random direction
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
