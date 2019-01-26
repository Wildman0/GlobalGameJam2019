using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSleep : MonoBehaviour {

    [SerializeField] private TimeProgression TimeKeep;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnButtonPress ()
    {
        TimeKeep.AdvanceMinutes(480);
    }
}
