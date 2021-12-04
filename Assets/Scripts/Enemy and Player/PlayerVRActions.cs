using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerVRActions : MonoBehaviour
{
    [Header("Player Movement Stats")]
    public float gravity = 30;
    public float sensitivity = 0.1f;
    public float maxSpeed = 3;
    public float rotateIncrement = 30;
    public float launchForce = 1000;
    
    [Header("VR Actions")]
    public SteamVR_Action_Boolean turnLeft = null;
    public SteamVR_Action_Boolean turnRight = null;
    public SteamVR_Action_Boolean movePressed = null;
    public SteamVR_Action_Vector2 moveValue = null;
    public SteamVR_Action_Boolean switchWeaponForward = null;
    public SteamVR_Action_Boolean switchWeaponBackward = null;
    public SteamVR_Action_Boolean launchDebris = null;
    public Valve.VR.InteractionSystem.Hand leftHand;
    public Valve.VR.InteractionSystem.Hand rightHand;

    private float speed = 0;

    private CharacterController charController;

    private const int NUM_HAND_TYPES = 3;
    private int currentHandLeft = 0;
    private int currentHandRight = 0;

    [Header("Player Objects")]
    public Transform cameraRig;
    public Transform head;
    public GameObject[] leftHands, rightHands;
    public GameObject photonPrefab;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    protected void Start()
    {
        //cameraRig = SteamVR_Render.Top().origin;
        //head = SteamVR_Render.Top().head;
        

        SwitchHand(leftHands, currentHandLeft);
        SwitchHand(rightHands, currentHandRight);
    }

    // Update is called once per frame
    protected void Update()
    {
        //Movement
        HandleLocalSpace();
        CalculateMovement();
        SnapRotation();

        //Weapon stuff
        HandleHandSwitching();
        HandleWeapons();
    }

    #region Movement

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
        if (moveValue.GetAxis(SteamVR_Input_Sources.LeftHand).magnitude == 0)
        {
            speed = 0;
        }

        //Add, clamp
        speed += moveValue.GetAxis(SteamVR_Input_Sources.LeftHand).magnitude * sensitivity;
        speed = Mathf.Clamp(speed, -maxSpeed * 0.5f, maxSpeed);

        //Direction & Gravity
        movement += orientation * (speed * Vector3.forward);
        movement.y -= gravity * Time.deltaTime;

        //Apply
        //Log(movement.x + " : " + moveValue.axis.magnitude + "\n" + rotatePress.state);
        charController.Move(movement * Time.deltaTime);
    }

    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(moveValue.GetAxis(SteamVR_Input_Sources.LeftHand).x, moveValue.GetAxis(SteamVR_Input_Sources.LeftHand).y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationEuler = new Vector3(0, head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    private void SnapRotation()
    {
        float snapValue = 0;

        if (turnLeft.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = -Mathf.Abs(rotateIncrement);

        else if (turnRight.GetStateDown(SteamVR_Input_Sources.RightHand))
            snapValue = Mathf.Abs(rotateIncrement);

        transform.RotateAround(head.position, Vector3.up, snapValue);
    }
    #endregion

    #region Hand and Weapons
    private void HandleHandSwitching()
    {
        //TODO Don't allow switching while holding something?

        //Left Hand
        if (leftHand.currentAttachedObject == null)
        {
            if (switchWeaponForward.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                currentHandLeft = (currentHandLeft + 1 == NUM_HAND_TYPES) ? 0 : currentHandLeft + 1;

                SwitchHand(leftHands, currentHandLeft);
            }
            else if (switchWeaponBackward.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                currentHandLeft = (currentHandLeft - 1 < 0) ? NUM_HAND_TYPES - 1 : currentHandLeft - 1;

                SwitchHand(leftHands, currentHandLeft);
            }
        }

        //Right Hand
        if (rightHand.currentAttachedObject == null)
        {
            if (switchWeaponForward.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                currentHandRight = (currentHandRight + 1 == NUM_HAND_TYPES) ? 0 : currentHandRight + 1;

                SwitchHand(rightHands, currentHandRight);
            }
            else if (switchWeaponBackward.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                currentHandRight = (currentHandRight - 1 < 0) ? NUM_HAND_TYPES - 1 : currentHandRight - 1;

                SwitchHand(rightHands, currentHandRight);
            }
        }
    }

    private void SwitchHand(GameObject[] hands, int switchTo)
    {
        if(leftHands[0] == null)
            leftHands[0] = GameObject.FindGameObjectWithTag("LeftHandModel");
        if (rightHands[0] == null)
            rightHands[0] = GameObject.FindGameObjectWithTag("RightHandModel");

        foreach (GameObject t in hands)
            if(t != null)
                t.SetActive(false);

        if (hands[switchTo] != null)
        {
            hands[switchTo].SetActive(true);
            
            if(switchTo != 0)
            {
                StopAllCoroutines();
                StartCoroutine(followHands(hands[switchTo], hands[0]));
            }    
        }
    }

    private void HandleWeapons()
    {
        //Left Hand
        if (launchDebris.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            if (currentHandLeft == 1 && leftHand.currentAttachedObject != null)
                ShootDebris(leftHand.currentAttachedObject, leftHand, leftHands[currentHandLeft]);
            else if (currentHandLeft == 2)
                ShootPhoton(leftHands[currentHandLeft]);
        }

        //Right Hand
        if (launchDebris.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            if (currentHandRight == 1 && rightHand.currentAttachedObject != null)
                ShootDebris(rightHand.currentAttachedObject, rightHand, rightHands[currentHandRight]);
            else if (currentHandRight == 2)
                ShootPhoton(rightHands[currentHandRight]);
        }
    }

    private void ShootDebris(GameObject debris, Valve.VR.InteractionSystem.Hand hand, GameObject handModel)
    {
        //Launch when trigger is pressed and if there is something that we are holding
        Rigidbody debrisBody = debris.GetComponent<Rigidbody>();

        if(debrisBody != null)
        {
            hand.DetachObject(debris);
            debrisBody.AddForce(launchForce * handModel.transform.forward, ForceMode.Impulse);
        }
        else
        {
            Debug.LogError("Couldn't get Throwable component!");
        }
    }

    private void ShootPhoton(GameObject handModel)
    {
        Rigidbody photon = Instantiate(photonPrefab, handModel.transform.position, Quaternion.identity).GetComponent<Rigidbody>();

        photon.transform.localScale = transform.localScale * 0.1f;

        photon.AddForce(launchForce * 0.1f * handModel.transform.forward, ForceMode.Impulse);

        Destroy(photon.gameObject, 3f);
    }

    private IEnumerator followHands(GameObject handModel, GameObject actualHand)
    {
        while(true)
        {
            handModel.transform.position = actualHand.transform.position;
            yield return null;
        }
    }
    #endregion
}
