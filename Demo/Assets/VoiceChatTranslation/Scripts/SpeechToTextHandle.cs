using FrostweepGames.Plugins.GoogleCloud.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpeechToTextHandle : MonoBehaviour
{
    [SerializeField] Text outputText;
    public SpeechController speechController;
    private TranslationController translationController;
    private void Awake()
    {
        translationController = GCTranslateManager.Instance.translationController;
    }

    private void OnEnable()
    {
        StartCoroutine(RegisterEvents());
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }

    private IEnumerator RegisterEvents()
    {
        yield return new WaitForSeconds(1);
        speechController.RegisterFinishedRecordEventHandler();
        speechController.StartRecordButtonOnClickHandler();
        speechController.OnTranslatedCompleted += OnSpeechToTextCompleted;
        translationController.OnTranslated += TranslationController_OnTranslated;
    }

    public void TranslationController_OnTranslated(string mgs)
    {
        Debug.Log("TranslationController_OnTranslated " + mgs);
    }

    private void UnRegisterEvents()
    {
        speechController.OnTranslatedCompleted -= OnSpeechToTextCompleted;
        speechController.StopRecordButtonOnClickHandler();
    }


    private void OnSpeechToTextCompleted(string mgs)
    {
        Debug.Log("speech to text output: " + mgs);
        if (outputText)
            outputText.text = mgs;
    }
}
