using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public GameObject objectInHand;
    [SerializeField] private AudioClip Pickup;

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
                gameObject.GetComponent<AudioSource>().PlayOneShot(Pickup);

                objectInHand.gameObject.layer = 8;

                foreach (Transform child in objectInHand.transform)
                {
                    child.gameObject.layer = 8;
                }

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
            objectInHand.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            objectInHand.gameObject.layer = 0;

            foreach (Transform child in objectInHand.transform)
            {
                child.gameObject.layer = 0;
            }

            objectInHand = null;
        }
    }

    void CarryObject()
    {
        if (objectInHand)
            objectInHand.transform.position = Camera.main.transform.position + Camera.main.transform.TransformDirection(new Vector3(0.2f, -0.1f, 1.0f));
    }
}
