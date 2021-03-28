using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class AudioMeasure : MonoBehaviour
{
    public float RmsValue;
    public float DbValue;
    public float PitchValue;

    private const int QSamples = 1024;
    private const float RefValue = 0.1f;
    private const float Threshold = 0.02f;

    float[] _samples;
    private float[] _spectrum;
    private float _fSample;

    private AudioSource micAudioSource;

    public Image chargeMeterFillBarImage;
    public TextMeshProUGUI chargeLevelText;

    private int count;
    private float movingAverage;

    public int movingAverageLength = 10;

    private float audioChargeMeterLevel;

    public int chargeBarDamperAmount = 5;

    void Start()
    {
        micAudioSource = GetComponent<AudioSource>();
        micAudioSource.clip = Microphone.Start(null, true, 100, 44100);
        micAudioSource.loop = true;
        micAudioSource.mute = false;
        while (!(Microphone.GetPosition(null) > 0)) { }
        micAudioSource.Play();

        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;

    }
     

    void Update()
    {
        count++;

        AnalyzeSound();

        CalculateMovingAverage();

        chargeLevelText.text = movingAverage.ToString();

        if (chargeBarDamperAmount <= 0) chargeBarDamperAmount = 1;

        chargeMeterFillBarImage.fillAmount = movingAverage / chargeBarDamperAmount;

    }

    private void CalculateMovingAverage()
    {
        if (count > movingAverageLength)
        {
            movingAverage = movingAverage + (DbValue - movingAverage) / (movingAverageLength + 1);
        }
        else
        {
            movingAverage += DbValue;
        }
        if (count == movingAverageLength)
        {
            movingAverage = movingAverage / count;
        }
    }

    void AnalyzeSound()
    {
        GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
        int i;
        float sum = 0;
        for (i = 0; i < QSamples; i++)
        {
            sum += _samples[i] * _samples[i]; // sum squared samples
        }
        RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
        if (DbValue < 0) DbValue = 0; // clamp it to 0dB min
                                            // get sound spectrum
        micAudioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0;
        var maxN = 0;
        for (i = 0; i < QSamples; i++)
        { // find max 
            if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                continue;

            maxV = _spectrum[i];
            maxN = i; // maxN is the index of max
        }
        float freqN = maxN; // pass the index to a float variable
        if (maxN > 0 && maxN < QSamples - 1)
        { // interpolate index using neighbours
            var dL = _spectrum[maxN - 1] / _spectrum[maxN];
            var dR = _spectrum[maxN + 1] / _spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency
    }
}