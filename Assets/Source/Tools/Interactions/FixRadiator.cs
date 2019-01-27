using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRadiator : MonoBehaviour
{
    [SerializeField] GameObject radiator;

    public void OnToolInteract(ToolType toolType)
    {
        if (toolType == ToolType.Wrench)
        {
            radiator.GetComponent<Radiator>().FixRadiator();
        }
    }
}
