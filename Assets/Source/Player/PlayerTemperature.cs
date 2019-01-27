using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class PlayerTemperature : MonoBehaviour
{
    public static PlayerTemperature getInstance;

    public int temperature = 5;
    public int increasePerRadiator = 5;

    private void Start()
    {
        getInstance = this;
    }
}
