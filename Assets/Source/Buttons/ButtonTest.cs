using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTest : MonoBehaviour {

    [SerializeField] private string message;

	public void OnButtonPress()
    {
        Debug.Log("Button Yeeted");
    }
}
