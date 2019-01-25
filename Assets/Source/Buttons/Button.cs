using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
	public void PressButton()
    {
        gameObject.SendMessage("OnButtonPress");
    }
}
