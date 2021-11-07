using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : LivingEntity
{
    [Header("Player Character")]
    [Range(0.1f, 0.3f)]
    public float lowShieldNotificationThreshold = 0.3f;
    [Range(0.1f, 0.3f)]
    public float structuralIntegrityCriticalNotificationThreshold = 0.3f;
    public GameObject lowShieldNotification;
    public GameObject structuralIntegrityCriticalNotification;
    public GameObject deathNotification;
    public GameObject pauseNotification;
    public GameObject pauseMenu;
    public Text menuText;
    private float lowShieldNotificationThresholdAbsolute;
    private float structuralIntegrityCriticalNotificationThresholdAbsolute;

    // Start is called before the first frame update
    protected override void Start()
    {
        lowShieldNotificationThresholdAbsolute = maxShields * lowShieldNotificationThreshold;
        structuralIntegrityCriticalNotificationThresholdAbsolute = maxHealth * structuralIntegrityCriticalNotificationThreshold;
        base.Start();
    }

    public void Update()
    {
        if (shields < maxShields)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer > rechargeDelay)
            {
                shields += rechargeRate * Time.deltaTime;
                if (shieldRenderer != null)
                    shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f + (shields / maxShields) * 1.2f);
                if (shields > lowShieldNotificationThresholdAbsolute)
                    lowShieldNotification.SetActive(false); // Reset the low shield notification

                UpdateBars();
            }
        }

        if(shieldRenderer != null)
            shieldRenderer.material.SetColor("Color_63659391", new Color(1.0f - (shields * 2 / 255), shields / 255, shields * 2 / 255));
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            rechargeTimer = 0;

            // If shields are empty, we take real damage
            // Else, we take shield damage and any damage the goes past the shield value is negated
            if (shields > 0)
            {
                shields -= collision.impulse.magnitude / damageResistance;

                if (shields < lowShieldNotificationThresholdAbsolute)
                    lowShieldNotification.SetActive(true); // Display low shield notification if shields go under the notification threshold

                if (shields <= 0)
                {
                    shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f);
                    shields = 0;
                }
            }
            else
            {
                health -= collision.impulse.magnitude / damageResistance;

                if (health < structuralIntegrityCriticalNotificationThresholdAbsolute)
                    structuralIntegrityCriticalNotification.SetActive(true); // Display structural integrity critical notification if health goes under the notification threshold
            }

            if (health <= 0)
            {
                health = 0;
                Die();
            }

            UpdateBars();
        }
    }

    #region Living Entity Overrides
    public override void Die()
    {
        pauseMenu.SetActive(true);
        pauseNotification.SetActive(false);
        deathNotification.SetActive(true);
        menuText.text = "Pick An Option";
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        base.Die();
    }
    #endregion
}
