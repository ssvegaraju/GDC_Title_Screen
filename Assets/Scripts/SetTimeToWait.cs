using UnityEngine;


// Allow people to define the minutes for the timer
public class SetTimeToWait : MonoBehaviour {

    public void OnStringChanged(System.String value) {
        PlayerPrefs.SetInt("Time", int.Parse(value));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Main");
    }
}
