using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInteractionTest : MonoBehaviour {

	public void OnToolInteract(ToolType toolType)
    {
        Debug.Log(toolType);
    }
}
