using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] public AudioClip clip;

	public void PressButton()
    {
        gameObject.SendMessage("OnButtonPress");
    }
}
