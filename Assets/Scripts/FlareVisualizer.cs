using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FlareVisualizer : MonoBehaviour {

    // How many samples to split the sound data into.
    private const int SAMPLE_SIZE = 1024;

    public GameObject LensFlarePrefab;

    /// <summary>
    ///  These three values are set by the analysis of the sound
    /// </summary>
    private float rmsValue;
    private float dbValue;
    private float pitchValue;

    // Clamp the brightnesses so they look somewhat comparative.
    public float maxVisualScale = 5;

    // Multplier by which to scale all the flares
    public float sizeModifier = 200;

    // How quickly the flares descend from a spike
    public float smoothSpeed = 10;

    // Which percentage of samples to keep (most of the latter portion barely move
    // so i only use a portion for the visualization).
    public float keepPercentage = 0.1f;

    private AudioSource source;
    private float[] samples;  // Passed into the audiosouce methods to get data.
    private float[] spectrum; // ^
    private float sampleRate;

    private GameObject[] visuals; // Store the flare objects
    private float[] scales;       // Store the brightness (y) of the flares.
    public int amountOfVisuals = 64; // How many flares to use.

    public float circleRadius = 7;

    
	// Use this for initialization
	private void Start () {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;

        SpawnCircle();
        SetScale(); // This is just to make the circle fit in the monado.
	}
	
	// Update is called once per frame
	private void Update () {
        AnalyzeSound(); 
        UpdateVisuals(); 
	}

    // Spawn flares in a circle 
    private void SpawnCircle()
    {
        visuals = new GameObject[amountOfVisuals];
        scales = new float[amountOfVisuals];

        Vector3 center = transform.position;

        for (int i = 0; i < amountOfVisuals; i++)
        {
			float rads = Mathf.PI * 2 * i / amountOfVisuals;
			float degs = 360.0f * i / amountOfVisuals;

            float x = center.x + Mathf.Cos(rads) * circleRadius;
            float y = center.y + Mathf.Sin(rads) * circleRadius;

			Vector3 pos = center + new Vector3(x, y, 0);
			GameObject g = Instantiate(LensFlarePrefab, pos, Quaternion.Euler(new Vector3(0, 0, degs)), transform);
            visuals[i] = g;
        }
    }

    // Analyze sound and assign pitch, db, and rms values.
    private void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);

        // Get the RMS Value
        float sum = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);

        // Get the DB Value
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        // Get Sound Spectrum
        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);

        // Find Pitch Value
        float maxV = 0;
        var maxN = 0;

        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            if (!(spectrum[i] > maxV) || !(spectrum[i] > 0.0f))
                continue;
            maxV = spectrum[i];
            maxN = i;
        }

        float freqN = maxN;
        if (maxN > 0 && maxN < SAMPLE_SIZE - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
    }

    // Fits the circle into the monado
    private void SetScale()
    {
        transform.position = new Vector3(-0.07f, -2.345f, 0);
        transform.localScale = Vector3.one * 0.055f;
    }
    
    // Set scale of cubes based on the values from analysis.
    private void UpdateVisuals()
    {
        int visualIndex = 0, spectrumIndex = 0;
        int averageSize = (int)(SAMPLE_SIZE * keepPercentage) / amountOfVisuals;

        while (visualIndex < amountOfVisuals)
        {
            int j = 0;
            float sum = 0;
            while (j < averageSize)
            {
                sum += spectrum[spectrumIndex];
                spectrumIndex++;
                j++;
            }

            float scaleY = sum / averageSize * sizeModifier;
            scales[visualIndex] -= smoothSpeed * Time.deltaTime;
            if (scales[visualIndex] < scaleY)
                scales[visualIndex] = scaleY;

            if (scales[visualIndex] > maxVisualScale)
                scales[visualIndex] = maxVisualScale;

            visuals[visualIndex].GetComponent<LensFlare>().brightness = scales[visualIndex] * 0.03f;
            visualIndex++;
        }
    }

}
