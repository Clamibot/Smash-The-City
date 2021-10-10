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
        lowShieldNotificationThresholdAbsolute = shields * lowShieldNotificationThreshold;
        structuralIntegrityCriticalNotificationThresholdAbsolute = health * structuralIntegrityCriticalNotificationThreshold;
        base.Start();
    }

    public void Update()
    {
        if (shieldBar.value < shields)
        {
            rechargeTimer += Time.deltaTime;
            if (rechargeTimer > rechargeDelay)
            {
                shieldBar.value += rechargeRate * Time.deltaTime;
                shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f + (shieldBar.value / shields) * 1.2f);
                if (shieldBar.value > lowShieldNotificationThresholdAbsolute)
                    lowShieldNotification.SetActive(false); // Reset the low shield notification
            }
        }

        shieldRenderer.material.SetColor("Color_63659391", new Color(1.0f - (shieldBar.value * 2 / 255), shieldBar.value / 255, shieldBar.value * 2 / 255));
    }

    protected void OnCollisionEnter(Collision collision)
    {
        rechargeTimer = 0;

        // If shields are empty, we take real damage
        // Else, we take shield damage and any damage the goes past the shield value is negated
        if (shieldBar.value > 0)
        {
            shieldBar.value -= collision.impulse.magnitude / damageResistance;

            if (shieldBar.value < lowShieldNotificationThresholdAbsolute)
                lowShieldNotification.SetActive(true); // Display low shield notification if shields go under the notification threshold

            if (shieldBar.value == 0)
                shieldRenderer.material.SetFloat("Vector1_AE9DFBD", -1.0f);
        }
        else
        {
            healthBar.value -= collision.impulse.magnitude / damageResistance;

            if (healthBar.value < structuralIntegrityCriticalNotificationThresholdAbsolute)
                structuralIntegrityCriticalNotification.SetActive(true); // Display structural integrity critical notification if health goes under the notification threshold
        }

        if (healthBar.value == 0)
        {
            Die();
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
