using System.Diagnostics;
using System.IO;
using System.Collections;
using UnityEngine;

// Get the title of the open Google Chrome tab
public class GetWindowTitle : MonoBehaviour {

    TMPro.TextMeshProUGUI text;

    // References to the two files we'll need.
    FileInfo program = new FileInfo("Assets/Scripts/ChromeTabReader.exe");
    FileInfo tabs = new FileInfo("ChromeTabs.txt");

    // Use this for initialization
    void Start () {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine("UpdateText");
    }

    // Start the process every 5 seconds instead of every frame to improve performance. 
    // Can probably do some way better optimizations but i just threw this together lol.
    private IEnumerator UpdateText()
    {
        while(true)
        {
            Process p = Process.Start(program.FullName); // Get the chrome tab with the c# app i wrote.
            yield return new WaitForSeconds(0.5f); // Wait for the program to write to the file.
            string[] openTabs = File.ReadAllLines(tabs.FullName);
            // Strip any extraneous strings.
            string currentlyPlaying = openTabs[openTabs.Length - 1].Replace(" - YouTube", "");
            currentlyPlaying = currentlyPlaying.Replace("- Audio playing", "");
            text.text = "Currently Playing - " + currentlyPlaying;
            p.Close(); // Close the program.
            // Make sure the file is wiped before the next write.
            File.WriteAllLines(tabs.FullName, new string[] { "" });
            yield return new WaitForSeconds(5f);
        }
    }
}
