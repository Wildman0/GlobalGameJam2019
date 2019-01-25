using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTool : MonoBehaviour
{
    ToolInHand toolInHand;

    private void Start()
    {
        toolInHand = gameObject.GetComponent<ToolInHand>();
    }

    void Update ()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3.0f))
        {
            if (hit.transform.tag == "Tool")
            {
                Debug.Log("Tool pickup");

                toolInHand.toolType = hit.transform.gameObject.GetComponent<Tool>().toolType;
                toolInHand.currentToolGameObject = hit.transform.gameObject;
            }
        }
    }
}
