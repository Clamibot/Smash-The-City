using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    public float defaultLineLength = 5;
    public GameObject dot;
    public VRInputModule inputModule;

    private LineRenderer lineRenderer = null;

    protected void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    protected void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        //Use default or raycast distance
        PointerEventData data = inputModule.getData();
        float targetLength = (data.pointerCurrentRaycast.distance == 0) ? defaultLineLength : data.pointerCurrentRaycast.distance;

        //Do the raycast
        RaycastHit hit = CreateRaycast(targetLength);

        //Default length
        Vector3 endPosition = transform.position + transform.forward * targetLength;

        //Hit length
        if (hit.collider != null)
            endPosition = hit.point;

        //Set dot position
        dot.transform.position = endPosition;

        //Set line renderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;

        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, defaultLineLength);

        return hit;
    }
}
