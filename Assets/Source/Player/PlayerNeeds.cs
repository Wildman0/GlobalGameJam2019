using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNeeds : MonoBehaviour {

    [SerializeField] private TimeProgression TimeKeep;
    private int CurrectTime, PrevTime;
    private int Hunger, Thirst, Sleep;
    private int TickCounter;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
       
	}

    void FixedUpdate ()
    {
        CurrectTime = TimeKeep.GetMinutes();

        if (PrevTime != CurrectTime)
        {
            TickCounter++;
            if (TickCounter == 1) // TickCounter determines how many time updates must pass before status update
            {
                Hunger -= 1;
                Thirst -= 1;
                Sleep -= 1;

                TickCounter = 0;
            }
        }

        PrevTime = CurrectTime;
    }
}
