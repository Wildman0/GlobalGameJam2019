using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeProgression : MonoBehaviour {
    private int Minutes;
    private int UpdateMinutes = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate ()
    {
        UpdateMinutes++;
        if (UpdateMinutes == 50)
        {
            if (Minutes == 1440)
            {
                Minutes = 0;
                UpdateMinutes = 0;
            }
            else
            {
                Minutes += 2;
                UpdateMinutes = 0;
            }
        }
    }
    
    public int GetMinutes ()
    {
        return Minutes;
    }

    public void SetMinutes (int NewMinutes)
    {
        Minutes = NewMinutes;
    }

    public void AdvanceMinutes(int AddedTime)
    {
        Minutes += AddedTime;
        
        if (Minutes > 1439)
        {
            Minutes -= 1440;
        }
    }
}
