using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DispenseWater : MonoBehaviour {

    [SerializeField] private GameObject Bottle;

    private bool IsBroken;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnButtonPress ()
    {
        if (!IsBroken)     
        {
            Instantiate(Bottle, (Bottle.transform.position + new Vector3(0, 1, 0)), Quaternion.identity);
        }
    }
          
}
