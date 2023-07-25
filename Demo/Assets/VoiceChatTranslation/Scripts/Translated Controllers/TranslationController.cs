using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace FrostweepGames.Plugins.GoogleCloud.Translation
{
    public class TranslationController : MonoBehaviour
    {   
        [SerializeField]
        private GCTranslation _gcTranslation;
        string textInput;
        string textOutput;

        public string TextInput { get => textInput; set => textInput = value; }
        public string TextOutput { get => textOutput; set => textOutput = value; }
        public UnityEvent translateSuccess;
        public event Action<string> OnTranslated;
        public event Action<string> OnTranslateFailed;
        private void Start()
        {
            _gcTranslation = GCTranslation.Instance;
            _gcTranslation.TranslateFailedEvent += _gcTranslation_TranslateFailedEvent;
            _gcTranslation.TranslateSuccessEvent += TranslateSuccessEventHandler;
            _gcTranslation.DetectLanguageSuccessEvent += DetectLanguageSuccessEventHandler;
            _gcTranslation.GetLanguagesSuccessEvent += GetLanguagesSuccessEventHandler;

            _gcTranslation.TranslateFailedEvent += TranslateFailedEventHandler;
            _gcTranslation.DetectLanguageFailedEvent += DetectLanguageFailedEventHandler;
            _gcTranslation.GetLanguagesFailedEvent += GetLanguagesFailedEventHandler;

            _gcTranslation.ContentOutOfLengthEvent += ContentOutOfLengthEventHandler;
        }

        private void _gcTranslation_TranslateFailedEvent(string obj)
        {
            OnTranslateFailed?.Invoke(obj);
        }

        private void OnDestroy()
        {
            _gcTranslation.TranslateSuccessEvent -= TranslateSuccessEventHandler;
            _gcTranslation.DetectLanguageSuccessEvent -= DetectLanguageSuccessEventHandler;
            _gcTranslation.GetLanguagesSuccessEvent -= GetLanguagesSuccessEventHandler;

            _gcTranslation.TranslateFailedEvent -= TranslateFailedEventHandler;
            _gcTranslation.DetectLanguageFailedEvent -= DetectLanguageFailedEventHandler;
            _gcTranslation.GetLanguagesFailedEvent -= GetLanguagesFailedEventHandler;

            _gcTranslation.ContentOutOfLengthEvent -= ContentOutOfLengthEventHandler;
        }


        public void Translate(string inputLangCode, string outputLangCode, string content)
        {
            _gcTranslation.Translate(new TranslationRequest()
            {
                q = content,
                source = inputLangCode,
                target = outputLangCode,
                format = "text",
                model = "nmt"
            });

        }

        private void DetectLanguageButtonOnclickHandler()
        {
            if (string.IsNullOrEmpty(TextInput))
                return;

            ResetState(false);

            _gcTranslation.DetectLanguage(new DetectLanguageRequest()
            {
                q = TextInput
            });
        }

        private void GetLanguagesButtonOnclickHandler()
        {
            if (string.IsNullOrEmpty(TextInput))
                return;

            ResetState(false);

            _gcTranslation.GetLanguages(new LanguagesRequest()
            {
                target = TextInput,
                model = "nmt"
            });
        }


        private void ResetState(bool status)
        {
            TextOutput = "";
        }

        // handlers
        public void TranslateSuccessEventHandler(TranslationResponse value)
        {
            ResetState(true);

            foreach (var translation in value.data.translations)
                textOutput += translation.translatedText;
            translateSuccess.Invoke();
            OnTranslated?.Invoke(textOutput);
        }

        private void DetectLanguageSuccessEventHandler(DetectLanguageResponse value)
        {
            ResetState(true);

            foreach (var detection in value.data.detections)
            {
                foreach (var item in detection)
                    TextOutput += "language: " + item.language + "\n" +
                                                         "isReliable: " + item.isReliable + "\n" +
                                                         "confidence: " + item.confidence + "\n---------\n";
            }
            Debug.Log(textOutput);
        }

        private void GetLanguagesSuccessEventHandler(LanguagesResponse value)
        {
            ResetState(true);

            foreach(var language in value.data.languages)
            {
                TextOutput += "name: " + language.name + "\n" +
                                                     "language: " + language.language + "\n---------\n";
            }
            Debug.Log(textOutput);
        }

        private void TranslateFailedEventHandler(string value)
        {
            ResetState(true);

            TextOutput = value;
            Debug.Log("Failed: " + value);
        }

        private void DetectLanguageFailedEventHandler(string value)
        {
            ResetState(true);

            TextOutput = value;
        }

        private void GetLanguagesFailedEventHandler(string value)
        {
            ResetState(true);

            TextOutput = value;
        }

        private void ContentOutOfLengthEventHandler()
        {
            ResetState(true);

            TextOutput = "Content Out Of Length";
        }
    }
}