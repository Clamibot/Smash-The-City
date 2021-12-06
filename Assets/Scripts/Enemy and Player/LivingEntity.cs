using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivingEntity : MonoBehaviour
{
    [Header("Living Entity")]
    public MeshRenderer shieldRenderer;
    public Image healthBar;
    public Image shieldBar;
    public float maxHealth = 100;
    [HideInInspector] public float health;
    public float maxShields = 100;
    [HideInInspector] public float shields;
    public float rechargeDelay = 5;
    public float rechargeRate = 20;
    public float damageResistance = 1000;
    public float rechargeTimer = 0;
    public GameObject fracture;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        health = maxHealth;
        shields = maxShields;

        if(healthBar != null)
            healthBar.fillAmount = 1;
        if (shieldBar != null)
            shieldBar.fillAmount = 1;
    }

    public virtual void Die()
    {
        //Instantiate(fracture, transform.position, transform.rotation);
        //Destroy(gameObject);
    }

    protected void UpdateBars()
    {
        healthBar.fillAmount = health / maxHealth;
        shieldBar.fillAmount = shields / maxShields;
    }
}
