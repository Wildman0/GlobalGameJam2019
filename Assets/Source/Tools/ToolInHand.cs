using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType
{
    None,
    Wrench,
    Paintbrush
}

public class ToolInHand : MonoBehaviour
{
    public ToolType toolType;
    public GameObject currentToolGameObject;

    private void Update()
    {
        if (currentToolGameObject)
            currentToolGameObject.transform.position = Camera.main.transform.position + Camera.main.transform.TransformDirection(new Vector3(0.4f, -0.1f, 1.0f));
    }
}
