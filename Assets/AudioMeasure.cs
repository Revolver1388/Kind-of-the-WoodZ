using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;
using System.IO;

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
    public float movingAverage;

    public int movingAverageLength = 10;
    public bool isCharging;
    public float chargeAmount;
    public float chargeDegradePercentPerFrame;
    public int chargeBarDamperAmount = 5;
    public int energyChargeMultiple;

    [SerializeField] TextMeshProUGUI runningChargeUpText;
    [SerializeField] TextMeshProUGUI chargedUpAmountText;
    [SerializeField] TextMeshProUGUI promptText;


#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
        }
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        void Update()
        {
            Microphone.Update();
        }
#endif

    void Start()
    {
        promptText.gameObject.SetActive(false);

#if !UNITY_WEBGL

        micAudioSource = GetComponent<AudioSource>();

        micAudioSource.clip = Microphone.Start(null, true, 1000, 44100);

        micAudioSource.loop = true;
        micAudioSource.mute = false;
        micAudioSource.volume = 1;
        while (!(Microphone.GetPosition(null) > 0)) { }
        micAudioSource.Play();

        _samples = new float[QSamples];
        _spectrum = new float[QSamples];
        _fSample = AudioSettings.outputSampleRate;
#endif

    }

    public void StartChargeUp()
    {
        isCharging = true;
        promptText.gameObject.SetActive(true);
    }

    public void StopCharging()
    {
        isCharging = false;
        chargedUpAmountText.text = chargeAmount.ToString();
        promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (chargeAmount < 0) chargeAmount = 0;
        
        if (!isCharging)
        {
            chargeAmount = chargeAmount - (chargeAmount * chargeDegradePercentPerFrame);

        }


        if (Input.GetKeyDown(KeyCode.Space) && !isCharging)
        {
            StartChargeUp();
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            StopCharging();
        }



        if (isCharging)
        {
            count++;

            AnalyzeSound();

            CalculateMovingAverage();

            chargeLevelText.text = movingAverage.ToString();

            if (chargeBarDamperAmount <= 0) chargeBarDamperAmount = 1;

            chargeAmount += movingAverage;
        }

        if (chargeAmount > 100) chargeAmount = 100;
        chargeMeterFillBarImage.fillAmount = chargeAmount / 100;
        runningChargeUpText.text = "Energy Charge: " + Mathf.Round(chargeAmount).ToString() + " / 100";


    }

    private void CalculateMovingAverage()
    {
        if (count > movingAverageLength)
        {
            movingAverage = movingAverage + (RmsValue - movingAverage) / (movingAverageLength + 1);
        }
        else
        {
            movingAverage += RmsValue;
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
        RmsValue = energyChargeMultiple * Mathf.Sqrt(sum / QSamples); // rms = square root of average
        DbValue = energyChargeMultiple * Mathf.Log10(RmsValue / RefValue); // calculate dB
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