using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Text;


// Get the title of the Google Chrome tab playing YouTube
public class GetWindowTitle : MonoBehaviour {
    
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowTextLength(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);


    private Text _text;

    private void Start () {
        _text = GetComponent<Text>();
        StartCoroutine("UpdateText");
    }
    
    private IEnumerator UpdateText()
    {
        while(true)
        {
            // Look for a window with this string as its name
            string windowName = GetWindowText(FindWindowsWithText("- YouTube - Google Chrome").FirstOrDefault());
            // Format the window title
            _text.text = "Currently Playing: " + windowName.Replace("- YouTube - Google Chrome", "");
            
            // Wait a bit so we call this code less frequently
			yield return new WaitForSeconds(2.0f);
        }
    }
    
    // Converts a handler to the window's title
    private static string GetWindowText(IntPtr hWnd)
    {
        int size = GetWindowTextLength(hWnd);
        
        if (size <= 0)
            return string.Empty;
        
        var builder = new StringBuilder(size + 1);
        GetWindowText(hWnd, builder, builder.Capacity);
        return builder.ToString();
    }

    // Finds all windows with the given text in the title
    private static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
    {
        return FindWindows((wnd, param) => GetWindowText(wnd).Contains(titleText));
    } 

    // Finds all windows with the given proc.
    // DO NOT call this directly
    private static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
    {
        List<IntPtr> windows = new List<IntPtr>();

        EnumWindows(delegate(IntPtr wnd, IntPtr param)
        {
            // only add the windows that pass the filter
            if (filter(wnd, param))
                windows.Add(wnd);

            // but return true here so that we iterate all windows
            return true;
        }, IntPtr.Zero);

        return windows;
    }
}
