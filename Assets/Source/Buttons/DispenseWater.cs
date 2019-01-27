using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenseWater : MonoBehaviour {

    [SerializeField] private GameObject Bottle;
    [SerializeField] private Transform dispensePoint;
    private bool isBroken;
    float lastTime = 0.0f;
    
    void OnButtonPress ()
    {
        if (!isBroken)     
        {
            Instantiate(Bottle, dispensePoint.position, Quaternion.identity);
        }
    }
    
    public void FixWaterDispencer ()
    {
        isBroken = false;
    }
}
