using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerVRActions : MonoBehaviour
{
    public float sensitivity = 0.1f;
    public float maxSpeed = 1.0f;

    public SteamVR_Action_Boolean movePressed = null;
    public SteamVR_Action_Vector2 moveValue = null;

    private float speed = 0;
    private Vector3 direction = Vector3.zero;

    private CharacterController charController;
    private Transform cameraRig;
    private Transform head;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        cameraRig = SteamVR_Render.Top().origin;
        head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleHead();
        HandleLocalSpace();
        CalculateMovement();
    }

    private void HandleHead()
    {
        //Store current rotation
        Vector3 oldPos = cameraRig.position;
        Quaternion oldRot = cameraRig.rotation;

        //Rotate
        transform.eulerAngles = new Vector3(0, head.rotation.eulerAngles.y, 0);

        //Restore
        cameraRig.position = oldPos;
        cameraRig.rotation = oldRot;
    }

    private void CalculateMovement()
    {
        //Figure out movement orientation
        Vector3 orientationEuler = new Vector3(0, transform.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        //If not moving
        if (movePressed.GetStateUp(SteamVR_Input_Sources.Any))
        {
            speed = 0;
            direction = Vector3.zero;
        }

        //If we are supposed to be moving
        if (movePressed.state)
        {
            //Add, clamp
            speed += moveValue.axis.magnitude * sensitivity;
            speed = Mathf.Clamp(speed, -maxSpeed * 0.5f, maxSpeed);

            //direction
            direction = new Vector3(moveValue.axis.normalized.x, 0, moveValue.axis.normalized.y);
            movement += orientation * (speed * direction) * Time.deltaTime;
        }

        //Apply
        charController.Move(movement);
    }

    private void HandleLocalSpace()
    {
        //Get head in local space
        float headHeight = Mathf.Clamp(head.localPosition.y, 1, 2);
        charController.height = headHeight;

        //Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = charController.height * 0.5f;
        newCenter.y += charController.skinWidth;

        //Move capsule in local space
        newCenter.x = head.localPosition.x;
        newCenter.z = head.localPosition.z;

        //Rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        //Apply
        charController.center = newCenter;
    }
}
