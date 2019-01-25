using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLights : MonoBehaviour {

    [SerializeField] private Light[] lights = new Light[0];

    public void OnButtonPress()
    {
        if (lights[0].intensity == 1)
        {
            for (int i = 0; i < lights.Length; i++ )
            {
                lights[i].intensity = 0;
            }
        }else
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = 1;
            }
        }

        
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
