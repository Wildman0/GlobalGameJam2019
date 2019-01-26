using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTool : MonoBehaviour
{
    ToolInHand toolInHand;
    PickupObject pickupObject;

    private void Start()
    {
        pickupObject = gameObject.GetComponent<PickupObject>();
        toolInHand = gameObject.GetComponent<ToolInHand>();
    }

    void Update ()
    {
        if (toolInHand.toolType == ToolType.None)
        {
            TryPickingUpTool();
        }
        else
        {
            TryDroppingTool();
        }
    }

    void TryPickingUpTool()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3.0f))
        {
            if (hit.transform.tag == "Tool")
            {
                Debug.Log("Tool pickup");

                toolInHand.toolType = hit.transform.gameObject.GetComponent<Tool>().toolType;
                pickupObject.objectInHand = hit.transform.gameObject;
                pickupObject.objectInHand.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }

    void TryDroppingTool()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            toolInHand.toolType = ToolType.None;
            pickupObject.objectInHand.GetComponent<Rigidbody>().useGravity = true;
            pickupObject.objectInHand = null;
        }
    }
}
