using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Temperature : MonoBehaviour
{
    Image temperature;

    private void Start()
    {
        temperature = gameObject.GetComponent<Image>();
    }

    private void Update()
    {
        temperature.fillAmount = PlayerTemperature.getInstance.temperature / 20.0f;
    }
}
