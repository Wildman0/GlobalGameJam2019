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

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.0f))
        {
            if (hit.transform.tag == "Tool")
            {
                toolInHand.toolType = hit.transform.gameObject.GetComponent<Tool>().toolType;
            }
        }
    }
}
