using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressButton : MonoBehaviour {

    private void Update()
    {
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E) && Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.0f))
        {
            if (hit.transform.tag == "Button")
            {
                hit.transform.gameObject.GetComponent<Button>().PressButton
            }
        }
    }
}
