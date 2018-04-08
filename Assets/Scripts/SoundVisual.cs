using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The meat of the project. It grabs the sound data from the audio
// source that is attached. Analyzes the sound, and then uses the value
// to scale cubes that are generated in various patterns. (At this time: Circle).
[RequireComponent(typeof(AudioSource))]
public class SoundVisual : MonoBehaviour {

    // How many samples to split the sound data into.
    private const int SAMPLE_SIZE = 1024;

    // For the moment I'm just using a cube i made in blender that has
    // a pivot point on the corner so it only scales in one direction.
    public GameObject cubePrefab;

    /// <summary>
    ///  These three values are set by the analysis of the sound
    /// </summary>
    private float rmsValue;
    private float dbValue;
    private float pitchValue;

    // Clamp the cubes from stretching way too far so they look
    // somewhat comparative.
    public float maxVisualScale = 25f;

    // Multplier by which to scale all the cubes
    public float sizeModifier = 10f;

    // How quickly the cubes descend from a spike
    public float smoothSpeed = 10f;

    // Which percentage of samples to keep (most of the latter portion barely move
    // so i only use a portion for the visualization).
    public float keepPercentage = 0.1f;

    private AudioSource source;
    private float[] samples; // Passed into the audiosouce methods to get data.
    private float[] spectrum; // Same.
    private float sampleRate;

    private Transform[] visuals; // Store the transforms of the cubes.
    private float[] scales;      // Store the scales (y) of the cubes.
    public int amountOfVisuals = 64; // How many cubes to use.

    public float circleRadius = 5;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;

        //SpawnLine();
        SpawnCircle();
        SetScale(); // This is just to make the circle of cubes fit in the monado.
	}
	
	// Update is called once per frame
	void Update () {
        AnalyzeSound(); 
        UpdateVisuals(); 
	}

    // Spawn cubes to be visualized with audio in a circle 
    private void SpawnCircle()
    {
        visuals = new Transform[amountOfVisuals];
        scales = new float[amountOfVisuals];

        Vector3 center = transform.position;

        for (int i = 0; i < amountOfVisuals; i++)
        {
            float angle = i * 1.0f / amountOfVisuals;
            angle = angle * Mathf.PI * 2;

            float x = center.x + Mathf.Cos(angle) * circleRadius;
            float y = center.y + Mathf.Sin(angle) * circleRadius;

            Vector3 pos = center + new Vector3(x, y, 0);
                //GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            GameObject g = Instantiate(cubePrefab, pos, Quaternion.LookRotation(Vector3.forward, pos), transform);
            visuals[i] = g.transform;
        }
    }

    // Spawn cubes to be visualized with audio in a line
    void SpawnLine()
    {
        visuals = new Transform[amountOfVisuals];
        scales = new float[amountOfVisuals];

        for (int i = 0; i < amountOfVisuals; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;
            visuals[i] = g.transform;
            visuals[i].parent = transform;
            visuals[i].position = Vector3.right * i;
        }
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

            visuals[visualIndex].localScale = Vector3.one + Vector3.up * scales[visualIndex];
            visualIndex++;
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

    // Don't worry about this if you're not trying to set the circle
    // to fit in the Monado
    private void SetScale()
    {
        transform.position = new Vector3(-0.07f, -2.345f, 0);
        transform.localScale = Vector3.one * 0.055f;
    }
}
