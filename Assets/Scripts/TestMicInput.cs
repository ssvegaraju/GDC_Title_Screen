using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Changes object's audiosource to the stereo mix
// Requires headphones to be plugged in to 3.5mm jack
// Requires Stereo Mix enabled and set to default.
[RequireComponent(typeof(AudioSource))]
public class TestMicInput : MonoBehaviour {

    private AudioSource aud;
    public bool useDesktopAudio = false;
	// Use this for initialization
	void Awake () {
        if (useDesktopAudio)
        {
            aud = GetComponent<AudioSource>();
            aud.clip = Microphone.Start(null, true, 10, AudioSettings.outputSampleRate);
            aud.loop = true; // Set the audClip to loop
            aud.mute = true; // Mute the sound, we don't want the player to hear it
            while (!(Microphone.GetPosition(null) > 0)) { } // Wait until the recording has started
            aud.Play(); // Play the aud source!
            aud.mute = false;
        }
    }
}
