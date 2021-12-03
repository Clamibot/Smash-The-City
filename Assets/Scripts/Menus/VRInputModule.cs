using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class VRInputModule : BaseInputModule
{
    public Camera camera;
    public SteamVR_Input_Sources targetSource;
    public SteamVR_Action_Boolean clickAction;

    private GameObject curPointedObject = null;
    private PointerEventData pData = null;

    protected override void Awake()
    {
        base.Awake();

        pData = new PointerEventData(eventSystem);
    }

    public override void Process()
    {
        //Reset data, set camera
        pData.Reset();
        pData.position = new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2);

        //Raycast
        eventSystem.RaycastAll(pData, m_RaycastResultCache);
        pData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        curPointedObject = pData.pointerCurrentRaycast.gameObject;

        //Clear the raycast
        m_RaycastResultCache.Clear();

        //Hover
        HandlePointerExitAndEnter(pData, curPointedObject);

        //Press
        if (clickAction.GetStateDown(targetSource))
            ProcessPress(pData);

        //Release
        if (clickAction.GetStateUp(targetSource))
            ProcessRelease(pData);

    }

    public PointerEventData getData()
    {
        return pData;
    }

    private void ProcessPress(PointerEventData data)
    {
        //Set raycast
        data.pointerPressRaycast = data.pointerCurrentRaycast;

        //Check for object hit, get the down handler, call
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(curPointedObject, data, ExecuteEvents.pointerDownHandler);

        //If no down handler, try and get clock handler
        if (newPointerPress == null)
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(curPointedObject);

        //Set data
        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = curPointedObject;

    }

    private void ProcessRelease(PointerEventData data)
    {
        //Exectue pointer up
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

        //Check for click handler
        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(curPointedObject);

        //Check if actual
        if(data.pointerPress == pointerUpHandler)
        {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        //Clear selected gameobject
        eventSystem.SetSelectedGameObject(null);

        //Reset data
        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;

    }
}
