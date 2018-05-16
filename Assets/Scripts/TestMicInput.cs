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
        _aud = GetComponent<AudioSource>();

        if (UseDesktopAudio)
        {
            string preferredDevice = null;
            foreach (string device in Microphone.devices)
            {
                print(device);
                if (device.Contains("CABLE Output"))
                {
                    preferredDevice = device;
                    break;
                }
                if (device.Contains("Stereo Mix"))
                {
                    preferredDevice = device;
                    break;
                }
            }
            // If there's no Stereo Mix or VB Cable, preferredDevice will be null
            _aud.clip = Microphone.Start(preferredDevice, true, 1, AudioSettings.outputSampleRate);
            // Wait until the recording has started
        }
        else
        {
            _aud.outputAudioMixerGroup = (Resources.Load("Prefabs/Mute") as 
                UnityEngine.Audio.AudioMixer).FindMatchingGroups("Master")[0];
            _aud.clip = Resources.Load("MOON - Paris") as AudioClip;
        }
        _aud.Play();
    }
}
