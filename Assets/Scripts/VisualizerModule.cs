using UnityEngine;
public interface IVisualizerModule
{
    // Called before start. True: Set inside Monado; False: Use whole screen
    bool Scale();
    
    // Called once on start.
    void Spawn(Transform transform);
    
    // Called evry frame
    void UpdateVisuals(int sampleSize, float[] spectrum);
}