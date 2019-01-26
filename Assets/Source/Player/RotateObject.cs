using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    PickupObject pickupObject;

    private void Start()
    {
        pickupObject = gameObject.GetComponent<PickupObject>();
    }

    private void Update()
    {
        if (pickupObject.objectInHand && Input.GetMouseButton(1))
        {
            SetObjectRotation();
        }
    }

    void SetObjectRotation()
    {
        Vector3 rotationVector = new Vector3(BoolToFloat(Input.GetKey(KeyCode.W)) - BoolToFloat(Input.GetKey(KeyCode.S)),
                                             0,
                                             BoolToFloat(Input.GetKey(KeyCode.A)) - BoolToFloat(Input.GetKey(KeyCode.D)));
        
        pickupObject.objectInHand.transform.Rotate(rotationVector * Time.deltaTime * 150);
    }

    float BoolToFloat(bool b)
    {
        if (b)
            return 1.0f;
        else
            return 0.0f;
    }
}
