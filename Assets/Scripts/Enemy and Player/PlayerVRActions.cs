using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerVRActions : MonoBehaviour
{
    public float gravity = 30;
    public float sensitivity = 0.1f;
    public float maxSpeed = 1.0f;
    public float rotateIncrement = 90;

    public SteamVR_Action_Boolean rotatePress = null;
    public SteamVR_Action_Boolean movePressed = null;
    public SteamVR_Action_Vector2 moveValue = null;

    private float speed = 0;
    private Vector3 direction = Vector3.zero;

    private CharacterController charController;
    public Transform cameraRig;
    public Transform head;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        //cameraRig = SteamVR_Render.Top().origin;
        //head = SteamVR_Render.Top().head;
    }

    // Update is called once per frame
    private void Update()
    {
        //HandleHead()
        HandleLocalSpace();
        CalculateMovement();
        SnapRotation();
    }

    /*private void HandleHead()
    {
        //Store current rotation
        Vector3 oldPos = cameraRig.position;
        Quaternion oldRot = cameraRig.rotation;

        //Rotate
        transform.eulerAngles = new Vector3(0, head.rotation.eulerAngles.y, 0);

        //Restore
        cameraRig.position = oldPos;
        cameraRig.rotation = oldRot;
    }*/

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

        //Apply
        charController.center = newCenter;
    }

    private void CalculateMovement()
    {
        //Figure out movement orientation
        Quaternion orientation = CalculateOrientation();
        Vector3 movement = Vector3.zero;

        //If not moving
        if (moveValue.axis.magnitude == 0)
        {
            speed = 0;
            direction = Vector3.zero;
        }

        //Add, clamp
        speed += moveValue.axis.magnitude * sensitivity;
        speed = Mathf.Clamp(speed, -maxSpeed * 0.5f, maxSpeed);

        //direction & gravity
        direction = new Vector3(moveValue.axis.normalized.x, 0, moveValue.axis.normalized.y);
        movement += orientation * (speed * direction);
        movement.y -= gravity * Time.deltaTime;

        //Apply
        Debug.Log(movement.x + " : " + moveValue.axis.magnitude + "\n" + rotatePress.state);
        charController.Move(movement * Time.deltaTime);
    }

    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.axis.x, moveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    private void SnapRotation()
    {
        float snapValue = 0;

        if (rotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
            snapValue = -Mathf.Abs(rotateIncrement);

        if (rotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = Mathf.Abs(rotateIncrement);

        transform.RotateAround(head.position, Vector3.up, snapValue);
    }
}
