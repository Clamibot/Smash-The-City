using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableFragment : MonoBehaviour
{
    private Valve.VR.InteractionSystem.Interactable interactableController;

    private void Start()
    {
        interactableController = GetComponent<Valve.VR.InteractionSystem.Interactable>();
        interactableController.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6) // Enables interaction when the player hand is touching the object fragment
            interactableController.enabled = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == 6) // Disables interaction when the player hand isn't touching the object fragment to save performance
            interactableController.enabled = false;
    }
}
