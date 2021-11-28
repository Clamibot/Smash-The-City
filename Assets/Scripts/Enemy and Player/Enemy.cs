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
    public int attackCooldownTime;
    public float attackRange = 4f;
    public GameObject enemyStats;
    private float originalSpeed;
    private float timePassed;

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

        if (player != null)
            GoTowardsPoint(player.transform.position);
        else
            agent.destination = transform.position;

        originalSpeed = agent.speed;

        timePassed = 0;

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
        //If the player exists, get their location
        if (player != null)
        {
            GoTowardsPoint(player.transform.position);
        }
        
        //Based on their distance, change speed (or go around randomly if the player is N/A)
        if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance)
        {
            if (player == null)
                GoToRandomPoint();
            else
                agent.speed = originalSpeed/10;
        }
        else
            agent.speed = originalSpeed;

        //Do we attack
        if (player != null && Vector3.Distance(transform.position, player.transform.position) < attackRange)
        {
            if (anim != null)
                anim.SetTrigger("Attack");

            agent.isStopped = true;

            if (timePassed < attackCooldownTime)
                timePassed += Time.deltaTime;
            if (timePassed >= attackCooldownTime)  
                timePassed = 0;
        }
        else
            agent.isStopped = false;

        //Recharge Shields
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

    public void GetHurt(float damageValue)
    {
        rechargeTimer = 0;
        enemyStats.SetActive(true);

        //Sheild or Health damage
        if (shields > 0)
        {
            shields -= (10 + damageValue * 10) / damageResistance;

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
            health -= (10 + damageValue * 10) / damageResistance;
        }

        //Does the creature die?
        if (health <= 0)
        {
            health = 0;

            //deinstancingArea._enteredInstances.Remove(allocatedGO);
            //deinstancingSphere._enteredInstances.Remove(allocatedGO);
            //GPUInstancer.GPUInstancerAPI.RemovePrefabInstance(prefabManager, allocatedGO);

            if (anim != null)
                anim.SetTrigger("Die");
            base.Die();
        }

        UpdateBars();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            Debug.Log("Body " + gameObject.name + " : hit value " + collision.relativeVelocity.magnitude);
            GetHurt(collision.relativeVelocity.magnitude);
        }
    }

    private IEnumerator MoveAfterAttackCooldown(float cooldownTime)
    {
        agent.isStopped = true;

        float timePassed = 0;
        while(timePassed < cooldownTime)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
    }

    public void AttemptAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < attackRange)
            player.GetHurt(attackPower);
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
