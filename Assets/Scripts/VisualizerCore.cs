using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VisualizerCore : MonoBehaviour {

	// How many samples to split the sound data into.
	private const int SAMPLE_SIZE = 1024;

	///  These three values are set by the analysis of the sound
	private float rmsValue;
	private float dbValue;
	private float pitchValue;

	private AudioSource source;
	private float[] samples; // Passed into the audiosouce methods to get data.
	private float[] spectrum; // Same.
	private float sampleRate;

	private GameObject _scaledEmpty;
	private List<IVisualizerModule> _visualizers;

    [Header("Modules")]
    public bool useSoundVisualizer = false;
    public bool useFlareVisualizer = true;
    public bool useWindVisualizer = true;

    private void Start ()
	{
		source = GetComponent<AudioSource>();
		samples = new float[SAMPLE_SIZE];
		spectrum = new float[SAMPLE_SIZE];
		sampleRate = AudioSettings.outputSampleRate;
		
		// Make an object to scale inside the Monado
		_scaledEmpty = new GameObject("ScaledEmpty");
		_scaledEmpty.transform.parent = transform;
		_scaledEmpty.AddComponent<Rotator>();

		// Build the list of visualizers to use
		_visualizers = new List<IVisualizerModule>();

        if(useFlareVisualizer)
		    _visualizers.Add(new FlareVisualizer());
        if(useWindVisualizer)
		    _visualizers.Add(new WindVisualizer());
        if(useSoundVisualizer)
		    _visualizers.Add(new SoundVisual());

		// Initialize visualizers
		foreach (IVisualizerModule visualizer in _visualizers)
		{
			GameObject empty = new GameObject(visualizer.GetType().ToString());
			empty.transform.parent = visualizer.Scale() ? _scaledEmpty.transform : transform;
			visualizer.Spawn(empty.transform);
		}
		
		ScaleToMonado();
	}

	private void Update()
	{
		AnalyzeSound();

		//Update visualizers
		foreach (IVisualizerModule visualizer in _visualizers)
			visualizer.UpdateVisuals(SAMPLE_SIZE, spectrum);
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
		int maxN = 0;

		for (int i = 0; i < SAMPLE_SIZE; i++)
		{
			if (spectrum[i] < maxV || spectrum[i] < 0.0f)
				continue;
			maxV = spectrum[i];
			maxN = i;
		}

		float freqN = maxN;
		var dL = spectrum[(maxN - 1 + SAMPLE_SIZE) % SAMPLE_SIZE] / spectrum[maxN];
		var dR = spectrum[(maxN + 1) % SAMPLE_SIZE] / spectrum[maxN];
		freqN += 0.5f * (dR * dR - dL * dL);
		pitchValue = freqN * (sampleRate / 2) / SAMPLE_SIZE;
	}

	private void ScaleToMonado()
	{
		_scaledEmpty.transform.position = new Vector3(-0.07f, -2.345f, 0);
		_scaledEmpty.transform.localScale = Vector3.one * 0.055f;
	}
}
