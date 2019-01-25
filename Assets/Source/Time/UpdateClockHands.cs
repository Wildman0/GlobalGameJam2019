using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateClockHands : MonoBehaviour {

    [SerializeField] private TimeProgression TimeKeep;
    [SerializeField] private GameObject HourHand;
    [SerializeField] private GameObject MinuteHand;

    private 
        
	void Start ()
    {
		
	}
	
	void Update ()
    {
        UpdateMinute();
    }

    void UpdateHour (int Minutes)
    {
        HourHand.transform.Rotate((30*(Minutes / 60)), 0, 0);
    }

    void UpdateMinute ()
    {
        int Minutes = TimeKeep.GetMinutes();

        MinuteHand.transform.Rotate((30*(Minutes % 60)), 0, 0);

        UpdateHour(Minutes);
    }
}


