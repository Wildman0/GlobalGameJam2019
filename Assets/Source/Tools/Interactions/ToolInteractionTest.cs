using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolInteractionTest : MonoBehaviour {

    [SerializeField] ToolType toolType = ToolType.Wrench;

	public void OnToolInteract(ToolType toolType)
    {
        if (toolType == this.toolType)
        {
            Debug.Log("Correct tool (Wrench)!");
        }
    }
}
