using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpeechEffect : MonoBehaviour
{
    [SerializeField] MicSpeechEffect micSpeechEffect;
    [SerializeField] GCTranslateManager gcTranslateManager;
    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }

    private void RegisterEvents()
    {
        gcTranslateManager.speechController.OnVoiceLevelChanged += micSpeechEffect.OnVoiceLevelChanged;
    }
    private void UnRegisterEvents()
    {
        gcTranslateManager.speechController.OnVoiceLevelChanged -= micSpeechEffect.OnVoiceLevelChanged;
    }

}
