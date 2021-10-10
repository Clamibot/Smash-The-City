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
    public GameObject enemyStats;
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

        prefabManager = GameObject.Find("GPUI Prefab Manager").GetComponent<GPUInstancer.GPUInstancerPrefabManager>();
        deinstancingSphere = GameObject.Find("DeInstancingSphere").GetComponent<GPUInstancer.GPUInstancerModificationCollider>();
        deinstancingArea = GameObject.Find("DeInstancingArea").GetComponent<GPUInstancer.GPUInstancerModificationCollider>();
        allocatedGO = GetComponent<GPUInstancer.GPUInstancerPrefab>();
        fracture.transform.localScale = transform.localScale;
        enemyStats.SetActive(false);
        enabled = false;
    }

    protected void Update()
    {
        if (player != null)
            GoTowardsPoint(player.transform.position);
        else
            GoToRandomPoint();

        if (player != null && Vector3.Distance(transform.position, player.transform.position) < attackRange)
            Attack();

        if (shieldBar.value < shields)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer > rechargeDelay)
            {
                shieldBar.value += rechargeRate * Time.deltaTime;
                shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f + (shieldBar.value / shields) * 1.2f);
                if (shieldBar.value == shields)
                {
                    enemyStats.SetActive(false);
                    enabled = false;
                }
            }
        }

        shieldRenderer.material.SetColor("Color_63659391", new Color(1.0f - (shieldBar.value * 2 / 255), shieldBar.value / 255, shieldBar.value * 2 / 255));
    }

    protected void OnCollisionEnter(Collision collision)
    {
        rechargeTimer = 0;
        enemyStats.SetActive(true);
        enabled = true;

        if (shieldBar.value > 0)
        {
            shieldBar.value -= collision.impulse.magnitude / damageResistance;

            if (shieldBar.value == 0)
                shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f);
        }
        else
        {
            healthBar.value -= collision.impulse.magnitude / damageResistance;
        }

        if (healthBar.value == 0)
        {
            deinstancingArea._enteredInstances.Remove(allocatedGO);
            deinstancingSphere._enteredInstances.Remove(allocatedGO);
            GPUInstancer.GPUInstancerAPI.RemovePrefabInstance(prefabManager, allocatedGO);
            base.Die();
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
