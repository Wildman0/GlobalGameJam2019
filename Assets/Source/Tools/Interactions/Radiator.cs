using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radiator : MonoBehaviour
{
    bool isFixed;
    [SerializeField] AudioClip fixSound;

    public void FixRadiator()
    {
        if (!isFixed)
        { 
            transform.Find("Area Light").GetComponent<Light>().intensity = 3.0f;
            PlayerTemperature.getInstance.temperature += PlayerTemperature.getInstance.increasePerRadiator;
            isFixed = true;

            Camera.main.transform.root.gameObject.GetComponent<AudioSource>().PlayOneShot(fixSound);

            Debug.Log(PlayerTemperature.getInstance.temperature);
        }
    }
}
