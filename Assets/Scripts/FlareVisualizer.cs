using UnityEngine;
using Object = UnityEngine.Object;

public class FlareVisualizer : IVisualizerModule
{

    public float MaxVisualScale = 0.25f;  // Clamp the brightnesses
    public float SizeModifier = 2.5f;     // Multplier by which to scale all the flares
    public float SmoothSpeed = 0.25f;     // How quickly the flares descend from a spike
    public int NumberOfFlares = 64;       // How many flares to use

    public float CircleRadius = 7;  // Used for scaling

    private GameObject _lensFlarePrefab;
    private GameObject[] _visuals; // Store the flare objects
    private float[] _scales;       // Store the brightnesses of the flares

    public bool Scale()
    {
        return true;
    }
    
    // Use this for initialization
    public void Spawn (Transform transform)
    {
        _lensFlarePrefab = (GameObject) Resources.Load("Prefabs/LensFlareObject");
        
        _visuals = new GameObject[NumberOfFlares];
        _scales = new float[NumberOfFlares];

        Vector3 center = transform.position;

        // Spawn flare objects
        for (int i = 0; i < NumberOfFlares; i++)
        {
            float rads = Mathf.PI * 2 * i / NumberOfFlares;
            float degs = 360.0f * i / NumberOfFlares;

            float x = center.x + Mathf.Cos(rads) * CircleRadius;
            float y = center.y + Mathf.Sin(rads) * CircleRadius;

            Vector3 pos = center + new Vector3(x, y, -1);
            GameObject g = Object.Instantiate(_lensFlarePrefab, pos, Quaternion.Euler(new Vector3(0, 0, degs)), transform);
            _visuals[i] = g;
        }
    }
    
    // Set brightnesses of flares based on the values from analysis.
    public void UpdateVisuals(int sampleSize, float[] spectrum)
    {
        // Build a logarithmic spectrum
        float[] scaledSpectrum = new float[NumberOfFlares];
        float b = Mathf.Pow(sampleSize, 1f / NumberOfFlares);
        float bPow = 1 / b;
        for (int i = 0; i < NumberOfFlares; i++)
        {
            float prevBPow = bPow;
            bPow *= b;
            for (int j = (int) prevBPow; j < bPow - 1; j++)
                scaledSpectrum[i] += spectrum[j];
        }
        
        // Set brightnesses of flares
        for (int i = 0; i < NumberOfFlares; i++)
        {
            // Smooth falling and sharp rising.
            float thisY = scaledSpectrum[i] * SizeModifier;
            _scales[i] -= SmoothSpeed * Time.deltaTime;
            _scales[i] = Mathf.Clamp(_scales[i], thisY, MaxVisualScale);

            _visuals[i].GetComponent<LensFlare>().brightness = _scales[i];
        }
    }

}
