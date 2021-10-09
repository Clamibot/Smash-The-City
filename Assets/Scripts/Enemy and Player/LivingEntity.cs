using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [Header("Living Entity")]
    public int maxHealth;
    [HideInInspector] public int currentHealth;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(int damageTaken)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageTaken);

        if (currentHealth < 1)
            Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
