using UnityEngine;

// The meat of the project. It grabs the sound data from the audio
// source that is attached. Analyzes the sound, and then uses the value
// to scale bars that are generated in various patterns. (At this time: Circle).
public class SoundVisual : IVisualizerModule
{
    private GameObject _barPrefab;

    // Which percentage of samples to keep (most of the latter portion barely move
    // so i only use a portion for the visualization).
    public float keepPercentage = 0.1f;
    
    // Clamp the bars from stretching way too far so they look
    // somewhat comparative.
    public float maxVisualScale = 5;

    // Multplier by which to scale all the bars
    public float sizeModifier = 175;

    // How quickly the bars descend from a spike
    public float smoothSpeed = 10;
    
    private Transform[] visuals; // Store the transforms of the bars.
    private float[] scales;      // Store the scales (y) of the bars.
    public int amountOfVisuals = 64; // How many bars to use.

    public float circleRadius = 5;

    public bool Scale()
    {
        return true;
    }

    // Use this for initialization
    public void Spawn(Transform transform)
    {
        _barPrefab = (GameObject) Resources.Load("Prefabs/Bar");
        
        visuals = new Transform[amountOfVisuals];
        scales = new float[amountOfVisuals];

        Vector3 center = transform.position;

        // Spawn bars
        for (int i = 0; i < amountOfVisuals; i++)
        {
			float rads = Mathf.PI * 2 * i / amountOfVisuals;
			float degs = 360.0f * i / amountOfVisuals;

            float x = center.x + Mathf.Cos(rads) * circleRadius;
            float y = center.y + Mathf.Sin(rads) * circleRadius;

			Vector3 pos = center + new Vector3(x, y, 0);
			GameObject g = Object.Instantiate(_barPrefab, pos, Quaternion.Euler(new Vector3(0, 0, degs)), transform);
            visuals[i] = g.transform;
        }
    }

    // Set scale of bars based on the values from analysis.
    public void UpdateVisuals(int sampleSize, float[] spectrum)
    {
        int spectrumIndex = 0;
        int averageSize = (int)(sampleSize * keepPercentage / amountOfVisuals);

        for (int visualIndex = 0; visualIndex < amountOfVisuals; visualIndex++)
        {
            float sum = 0;
            for (int j = 0; j < averageSize; j++)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
            }

            float scaleY = sum / averageSize * sizeModifier;
            scales[visualIndex] -= smoothSpeed * Time.deltaTime;
            scales[visualIndex] = Mathf.Clamp(scales[visualIndex], scaleY, maxVisualScale);

			visuals[visualIndex].localScale = Vector3.one + Vector3.right * scales[visualIndex];
        }
    }
}
