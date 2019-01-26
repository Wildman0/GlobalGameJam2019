using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public GameObject objectInHand;
    ToolInHand toolInHand;

    private void Start()
    {
        toolInHand = gameObject.GetComponent<ToolInHand>();
    }

    void Update ()
    {
        if (!objectInHand)
            CheckForObjectPickup();
        else
            CheckForObjectDrop();

        CarryObject();
    }

    void CheckForObjectPickup()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3.0f))
        {
            if (hit.transform.gameObject.GetComponent<Rigidbody>() && !hit.transform.gameObject.GetComponent<Rigidbody>().isKinematic)
            {
                objectInHand = hit.transform.gameObject;
                objectInHand.GetComponent<Rigidbody>().useGravity = false;
                objectInHand.GetComponent<Rigidbody>().freezeRotation = true;

                if (hit.transform.tag == "Tool")
                {
                    toolInHand.toolType = hit.transform.gameObject.GetComponent<Tool>().toolType;
                }
            }
        }
    }

    void CheckForObjectDrop()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (objectInHand.tag == "Tool")
            {
                toolInHand.toolType = ToolType.None;
            }

            objectInHand.GetComponent<Rigidbody>().useGravity = true;
            objectInHand.GetComponent<Rigidbody>().freezeRotation = false;
            objectInHand = null;
        }
    }

    void CarryObject()
    {
        if (objectInHand)
            objectInHand.transform.position = Camera.main.transform.position + Camera.main.transform.TransformDirection(new Vector3(0.4f, -0.1f, 1.0f));
    }
}
