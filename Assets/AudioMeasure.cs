using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;
using DG.Tweening;

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

    bool mouseTap = false, startTap = false;
    private readonly float chtime = 0.1f;
    float leftime;

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
    public float energyChargeMultiple;

    private HeroControls heroControls;

    [SerializeField] TextMeshProUGUI runningChargeUpText;
    [SerializeField] TextMeshProUGUI chargedUpAmountText;
    [SerializeField] TextMeshProUGUI promptText;

    [SerializeField] List<string> promptTextList;

    private Transform cameraTranform;

    private Febucci.UI.TextAnimator textAnimator;
    private Febucci.UI.TextAnimatorPlayer textAnimatorPlayer;

    Coroutine playTextCoroutine;
    Coroutine cameraShakeCoroutine;

#if UNITY_WEBGL && !UNITY_EDITOR
        void Awake()
        {
            Microphone.Init();
            Microphone.QueryAudioInput();
        }
#endif



    void Start()
    {
        if (PlayerPrefs.GetInt("MicMode") == 0) isNoSoundMode = false;
        else isNoSoundMode = true;
        heroControls = FindObjectOfType<HeroControls>();
        cameraTranform = Camera.main.transform;

        textAnimator = promptText.gameObject.GetComponent<Febucci.UI.TextAnimator>();
        textAnimatorPlayer = promptText.gameObject.GetComponent<Febucci.UI.TextAnimatorPlayer>();

        promptText.gameObject.SetActive(false);

#if !UNITY_WEBGL
        if (!isNoSoundMode)
        {
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
        }
#endif

    }

    public void StartChargeUp()
    {
        isCharging = true;
        promptText.gameObject.SetActive(true);
        cameraTranform.DOShakePosition(10f, 0.1f, 30, 90, false, false);
        //cameraShakeCoroutine = StartCoroutine(cameraShakeHelper());
        playTextCoroutine = StartCoroutine(PlayPromptTextSequence());
    }

    public void StopCharging()
    {
        isCharging = false;

        StopCoroutine(playTextCoroutine);
        //StopCoroutine(cameraShakeCoroutine);
        cameraTranform.DOKill();

        promptText.gameObject.SetActive(false);
        //StopCoroutine(cameraShakeHelper());


    }

    private IEnumerator cameraShakeHelper()
    {
        for (int i = 0; i < 30; i++)
        {
            cameraTranform.DOShakePosition(0.2f, (0.1f + (i / 5)), Mathf.RoundToInt(100f * (0.1f + (i / 5f))), 90, false, false);
            yield return new WaitForSeconds(0.2f);

        }
    }

    private IEnumerator PlayPromptTextSequence()
    {
        if (!PlayerPrefs.HasKey("FirstPlayDone"))
        {
            PlayerPrefs.SetInt("FirstPlayDone", 1);
            for (int i = 0; i < 5; i++)
            {
                textAnimatorPlayer.ShowText(promptTextList[UnityEngine.Random.Range(0, 20)]);
                yield return new WaitUntil(() => textAnimator.allLettersShown);
                yield return new WaitForSeconds(2);
                promptText.gameObject.SetActive(false);
                promptTextContainerTransform.gameObject.SetActive(false);
                promptTextContainerTransform.localScale = new Vector3(0.5f + (chargeAmount / 100), 0.5f + (chargeAmount / 100), 0.5f + (chargeAmount / 100));
                promptTextContainerTransform.gameObject.SetActive(true);
                promptText.gameObject.SetActive(true);
            }
            StartCoroutine(PlayPromptTextSequence());
        }
        else
        {
            Debug.Log("Starting prompt text display coroutine");
            promptText.gameObject.SetActive(true);

            textAnimatorPlayer.ShowText(promptTextList[UnityEngine.Random.Range(0, promptTextList.Count)]);
            yield return new WaitUntil(() => textAnimator.allLettersShown);
            yield return new WaitForSeconds(2);
            promptText.gameObject.SetActive(false);
            promptTextContainerTransform.gameObject.SetActive(false);
            promptTextContainerTransform.localScale = new Vector3(0.5f + (chargeAmount / 100), 0.5f + (chargeAmount / 100), 0.5f + (chargeAmount / 100));
            promptTextContainerTransform.gameObject.SetActive(true);
            promptText.gameObject.SetActive(true);
            StartCoroutine(PlayPromptTextSequence());
        }

       

    }

    [SerializeField] Transform promptTextContainerTransform;
    public bool isNoSoundMode;
    public void NoMicMode()
    {
        isNoSoundMode = true;
    }

    private void Update()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Microphone.Update();

#endif

        if (isNoSoundMode)
        {

            if (chargeAmount < 0) chargeAmount = 0;

            if (!isCharging)
            {
                chargeAmount = chargeAmount - (chargeAmount * chargeDegradePercentPerFrame);
                promptText.gameObject.SetActive(false);
            }

            if (heroControls.isChargingUp && !isCharging)
            {
                StartChargeUp();
            }

            if (!heroControls.isChargingUp && isCharging)
            {
                StopCharging();
                textAnimatorPlayer.StopShowingText();
                promptText.gameObject.SetActive(false);
            }

            if (isCharging)
            {
                NoMicCharge();
                
                chargeLevelText.text = movingAverage + chargeAmount.ToString();
                if (chargeBarDamperAmount <= 0) chargeBarDamperAmount = 1;

            }

            if (chargeAmount > 100) chargeAmount = 100;
            chargeMeterFillBarImage.fillAmount = chargeAmount / 100;
            runningChargeUpText.text = "Energy Charge: " + Mathf.Round(chargeAmount).ToString() + " / 100";
        }
    }
    void FixedUpdate()
    {
        if (chargeAmount < 0) chargeAmount = 0;
        
        if (!isCharging)
        {
            chargeAmount = chargeAmount - (chargeAmount * chargeDegradePercentPerFrame);
            promptText.gameObject.SetActive(false);
        }



        if (heroControls.isChargingUp && !isCharging)
        {
            StartChargeUp();
        }
        
        if (!heroControls.isChargingUp && isCharging)
        {
            StopCharging();
            textAnimatorPlayer.StopShowingText();
            promptText.gameObject.SetActive(false);
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
#if UNITY_WEBGL && !UNITY_EDITOR
            float[] volumes = Microphone.volumes;
            RmsValue = volumes[0];
#endif
#if !UNITY_WEBGL || (UNITY_WEBGL && UNITY_EDITOR)
        if (!isNoSoundMode)
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


#endif

        //else
        //{
        //    RmsValue = 0.01f;
        //}

    }
 
    void NoMicCharge()
    {
        if (!startTap)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseTap = true;
                startTap = true;
            }
        }
        else
        {
            if (!mouseTap)
            {
                if (Input.GetMouseButtonDown(0)) { mouseTap = true; }
            }
            else
            {
                if (leftime < chtime) { leftime += 1 * Time.deltaTime; chargeAmount += 50 * Time.deltaTime; }
                else if(leftime >= chtime) chargeAmount -= 10 * Time.deltaTime;

                if (Input.GetMouseButtonDown(1)) { mouseTap = false; leftime = 0; }
            }
        }
    }
}