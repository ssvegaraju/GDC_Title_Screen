using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Countdown the time to the meeting.
public class Countdown : MonoBehaviour {

    // define private variables to work with time
    float timeLeft;
    private int minutes;
    private int seconds;

    public TMPro.TextMeshProUGUI text;

	// Use this for initialization
	void Start () {
        timeLeft = 60 * PlayerPrefs.GetInt("Time", 1); // Grab defined time
        text = GetComponent<TMPro.TextMeshProUGUI>();
	}
	
	// Update is called once per frame
	void Update () {
        if (timeLeft > 0)
        {
            minutes = Mathf.FloorToInt(timeLeft / 60);
            seconds = Mathf.RoundToInt(timeLeft) % 60;
            if (seconds > 59)
                seconds = 59;

            timeLeft -= Time.deltaTime;

            string min = minutes.ToString();
            if (minutes < 10)
                min = "0" + min;
            string sec = seconds.ToString();
            if (seconds < 10)
                sec = "0" + sec;

            text.text = "Meeting will begin in: " + min + ":" + sec;
        }
        else
        {
            text.text = "Meeting will begin momentarily";
        }
	}
}
