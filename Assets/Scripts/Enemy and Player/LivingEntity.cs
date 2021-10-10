using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivingEntity : MonoBehaviour
{
    [Header("Living Entity")]
    public MeshRenderer shieldRenderer;
    public Slider healthBar;
    public Slider shieldBar;
    public float health = 100;
    public float shields = 100;
    public float rechargeDelay = 5;
    public float rechargeRate = 20;
    public float damageResistance = 1000;
    public float rechargeTimer = 0;
    public GameObject fracture;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        shieldBar.maxValue = shields;
        shieldBar.value = shields;
    }

    public virtual void Die()
    {
        Instantiate(fracture, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
