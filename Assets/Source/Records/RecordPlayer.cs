using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    [SerializeField] GameObject recordAttachPoint;

    private void Start()
    {
        recordAttachPoint = transform.Find("DiscOrigin").gameObject;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Record")
        {
            AttachRecord(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Record")
        {
            collision.gameObject.GetComponent<Record>().StopRecord();
        }
    }

    void AttachRecord(GameObject go)
    {
        go.transform.parent = recordAttachPoint.transform;
        go.GetComponent<Record>().PlayRecord();
        Debug.Log("ye");
    }
}
