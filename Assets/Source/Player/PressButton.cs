using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour {

    private void Update()
    {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2.0f))
        {
            if (hit.transform.tag == "Button")
            {
                hit.transform.gameObject.GetComponent<Button>().PressButton();
            }
        }
    }
}
