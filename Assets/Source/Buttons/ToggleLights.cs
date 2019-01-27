using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLights : MonoBehaviour {

    [SerializeField] private Light[] lights = new Light[0];
    [SerializeField] private float intensity = 0.6f;

    public void OnButtonPress()
    {
        if (lights[0].intensity != 0)
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = 0;
            }
        }
        else
        {
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].intensity = intensity;
            }
        }
    }
}
