using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : LivingEntity
{
    [Header("Player Character")]
    public int maxShield;
    [HideInInspector] public int currentShield;

    // Start is called before the first frame update
    protected override void Start()
    {
        currentShield = maxShield;

        base.Start();
    }


    #region Living Entity Overrides
    public override void TakeDamage(int damageTaken)
    {
        //If sheilds are empty, we take real damage
        //Else, we take shield damage and any damage the goes past the sheild value is negated
        if (currentShield < 1)
            base.TakeDamage(damageTaken);
        else
            currentShield = Mathf.Max(0, currentShield - damageTaken);
    }

    public override void Die()
    {
        //TODO Game stuff

        base.Die();
    }
    #endregion
}
