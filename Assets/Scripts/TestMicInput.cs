using UnityEngine;

// Changes object's audiosource to the stereo mix
// Requires headphones to be plugged in to 3.5mm jack
// Requires Stereo Mix enabled and set to default.
[RequireComponent(typeof(AudioSource))]
public class TestMicInput : MonoBehaviour {

    private AudioSource _aud;
    public bool UseDesktopAudio;
	
	// Use this for initialization
	private void Awake () {
	    if (!UseDesktopAudio) return;

		string stereoMixName = null;
		foreach (string device in Microphone.devices) {
			print(device);
			if (device.Contains("Stereo Mix"))
			{
				stereoMixName = device;
				break;
			}
		}
		
	    _aud = GetComponent<AudioSource>();
		// If there's no Stereo Mix, stereoMixName will be null
		// WARNING: THE DEVICE CAN'T BE CHOSEN UNTIL UNITY 2018.2
	    _aud.clip = Microphone.Start(stereoMixName, true, 9999, AudioSettings.outputSampleRate);
		// Wait until the recording has started
	    while (!(Microphone.GetPosition(null) > 0)) { }
	    _aud.Play();
	}
}
