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

        if (toolInHand.toolType == ToolType.Wrench)
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2.0f))
            {
                if (hit.transform.tag == "ToolInteractable")
                {
                    hit.transform.gameObject.SendMessage("OnToolInteract", toolInHand.toolType);
                }
            }
        }
        else if (toolInHand.toolType == ToolType.Paintbrush)
        {
            if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2.0f))
            {
                if (hit.transform.tag == "Paintable")
                {
                    hit.transform.gameObject.GetComponent<MeshRenderer>().material.color = toolInHand.GetPaintbrushColor();
                    Debug.Log("Paint");
                }
                else if (hit.transform.tag == "PaintBucket")
                {
                    toolInHand.SetPaintbrushColor(hit.transform.gameObject.GetComponent<PaintBucket>().color);
                    Debug.Log("SetPaint");
                }
            }
        }
    }
}
