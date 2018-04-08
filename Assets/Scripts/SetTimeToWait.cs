using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Allow people to define the minutes for the timer
public class SetTimeToWait : MonoBehaviour {

    public void OnStringChanged(System.String value) {
        PlayerPrefs.SetInt("Time", int.Parse(value));
    }

    public void LoadNextLevel()
    {
        AsyncOperation loading = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main");
    }
}
