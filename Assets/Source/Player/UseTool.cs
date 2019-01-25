using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseTool : MonoBehaviour
{
    ToolInHand toolInHand;

    private void Start()
    {
        toolInHand = gameObject.GetComponent<ToolInHand>();
    }

    void Update ()
    {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2.0f))
        {
            if (hit.transform.tag == "ToolInteractable")
            {
                hit.transform.gameObject.SendMessage("OnToolInteract", toolInHand.toolType);
            }
        }
    }
}
