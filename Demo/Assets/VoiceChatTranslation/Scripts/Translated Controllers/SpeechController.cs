using System;
using System.Collections.Generic;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
using FrostweepGames.Plugins.GoogleCloud.Translation;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public enum TYPE_SPEECH_MESSAGE
{
    NUMBER,
    INFO,
}
public class SpeechController : MonoBehaviour
{
    [SerializeField] TYPE_SPEECH_MESSAGE typeSpeech = TYPE_SPEECH_MESSAGE.INFO;
    [SerializeField]
    private GCSpeechRecognition _speechRecognition;
    private Image _speechRecognitionState;
    private Image _voiceLevelImage;

    [SerializeField]
    private bool _voiceDetectionToggle = true;
    [SerializeField]
    private bool _recognizeDirectlyToggle = true;
    [SerializeField]
    private bool _longRunningRecognizeToggle = false;

    private AudioClip clipTest; // use for translate audio file like .mp3 file;    
    private static bool isRecording = false;
    [SerializeField]
    private string languageSpeechCode = "ja_JP";

    public static bool IsRecording
    {
        get { return isRecording; }
    }

    public Action<string> OnTranslatedCompleted;
    public Action<float> OnVoiceLevelChanged;
    public Action<RecognitionResponse> RecognizeSuccessEvent;
    public Action<string> RecognizeFailedEvent;
    public bool isForNumberControlSpeech;
    public bool isForOtherSpeech;
    // [SerializeField]
    // private Text _resultText;

    void Start()
    {
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
        _speechRecognition.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
        _speechRecognition.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
        _speechRecognition.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
        _speechRecognition.GetOperationFailedEvent += GetOperationFailedEventHandler;
        _speechRecognition.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
        _speechRecognition.ListOperationsFailedEvent += ListOperationsFailedEventHandler;
        _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
        _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;
        _speechRecognition.BeginTalkigEvent += BeginTalkigEventHandler;
        RefreshMicsButtonOnClickHandler();
        MicrophoneDevicesDropdownOnValueChangedEventHandler(0); // get the first Micro was found.
    }

    public void RegisterFinishedRecordEventHandler()
    {
        _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;
        _speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
    }

    public void UnregisterFinishedRecordEventHandler()
    {
        _speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
        _speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
    }

    public void InitVoiceLevel_UI(Image stateSpeechRecog = null, Image voiceLevel = null)
    {
        this._speechRecognitionState = stateSpeechRecog;
        this._voiceLevelImage = voiceLevel;
    }

    public void SetLanguage(string _speechCode)
    {
        this.languageSpeechCode = _speechCode;
    }

    public void OnOffRecord()
    {
        if (!isRecording)
        {
            RefreshMicsButtonOnClickHandler();
            MicrophoneDevicesDropdownOnValueChangedEventHandler(0); // get the first Micro was found.
            StartRecordButtonOnClickHandler();
        }
        else
        {
            StopRecordButtonOnClickHandler();
        }

    }

    private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
    {
        Debug.Log("Recognize Success.");
        InsertRecognitionResponseInfo(recognitionResponse);
        RecognizeSuccessEvent?.Invoke(recognitionResponse);
    }

    private void InsertRecognitionResponseInfo(RecognitionResponse recognitionResponse)
    {
        if (recognitionResponse == null || recognitionResponse.results.Length == 0)
        {
            Debug.Log("\nWords not detected.");
            return;
        }
        Debug.Log(languageSpeechCode + " speech controller: " + gameObject.name + " " + (recognitionResponse.results[0].alternatives[0].transcript));
        OnTranslatedCompleted?.Invoke(recognitionResponse.results[0].alternatives[0].transcript);
        var words = recognitionResponse.results[0].alternatives[0].words;

        if (words != null)
        {
            string times = string.Empty;

            foreach (var item in recognitionResponse.results[0].alternatives[0].words)
            {
                times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
            }

            //Debug.Log("My words\n" + times);
        }

        string other = "\nDetected alternatives: ";

        foreach (var result in recognitionResponse.results)
        {
            foreach (var alternative in result.alternatives)
            {
                if (recognitionResponse.results[0].alternatives[0] != alternative)
                {
                    other += alternative.transcript + ", ";
                }
            }
        }

        //Debug.Log("Other words " + other);
    }

    private void RecognizeFailedEventHandler(string error)
    {
        // _resultText.text = "Recognize Failed: " + error;
        RecognizeFailedEvent?.Invoke(error);
    }
    private void LongRunningRecognizeFailedEventHandler(string error)
    {
        // _resultText.text = "Long Running Recognize Failed: " + error;
    }
    private void LongRunningRecognizeSuccessEventHandler(Operation operation)
    {
        if (operation.error != null || !string.IsNullOrEmpty(operation.error.message))
            return;

        //_resultText.text = "Long Running Recognize Success.\n Operation name: " + operation.name;

        if (operation != null && operation.response != null && operation.response.results.Length > 0)
        {
            // _resultText.text = "Long Running Recognize Success.";
            //_resultText.text += "\n" + operation.response.results[0].alternatives[0].transcript;
            //chatManager.SetContent(chatManager.SelfId, operation.response.results[0].alternatives[0].transcript);
            string other = "\nDetected alternatives:\n";

            foreach (var result in operation.response.results)
            {
                foreach (var alternative in result.alternatives)
                {
                    if (operation.response.results[0].alternatives[0] != alternative)
                    {
                        other += alternative.transcript + ", ";
                    }
                }
            }

            //_resultText.text += other;
            //chatManager.SetContent(chatManager.SelfId, other);
        }
        else
        {
            //_resultText.text = "Long Running Recognize Success. Words not detected.";
            //chatManager.SetContent("1", "...");
        }
    }
    private void GetOperationSuccessEventHandler(Operation operation)
    {
        //_resultText.text = "Get Operation Success.\n";
        //_resultText.text += "name: " + operation.name + "; done: " + operation.done;

        if (operation.done && (operation.error == null || string.IsNullOrEmpty(operation.error.message)))
        {
            // InsertRecognitionResponseInfo(operation.response);
        }
    }
    private void GetOperationFailedEventHandler(string error)
    {
        // _resultText.text = "Get Operation Failed: " + error;
    }
    private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
    {
        // _resultText.text = "List Operations Success.\n";

        if (operationsResponse.operations != null)
        {
            // _resultText.text += "Operations:\n";

            foreach (var item in operationsResponse.operations)
            {
                //  _resultText.text += "name: " + item.name + "; done: " + item.done + "\n";
            }
        }
    }
    private void ListOperationsFailedEventHandler(string error)
    {
        //_resultText.text = "List Operations Failed: " + error;
    }
    private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
    {
        if (!_voiceDetectionToggle && !isRecording)
        {
            // _speechRecognitionState.color = Color.green;
        }
        if (clip == null || !_recognizeDirectlyToggle)
            return;
        RecognitionConfig config = RecognitionConfig.GetDefault();
        config.languageCode = languageSpeechCode.ToString();
        config.speechContexts = new SpeechContext[]
        {
            new SpeechContext()
            {
           // phrases = new string[1]{ typeSpeech.ToString() }
            }
        };
        config.audioChannelCount = clip.channels;
        // configure other parameters of the config if need

        GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
        {
            audio = new RecognitionAudioContent()
            {
                content = raw.ToBase64()
            },
            config = config
        };

        if (_longRunningRecognizeToggle)
        {
            _speechRecognition.LongRunningRecognize(recognitionRequest);
        }
        else
        {
            _speechRecognition.Recognize(recognitionRequest);
        }
    }

    private void FinishedRecordEventHandlerEng(AudioClip clip, float[] raw)
    {
        if (languageSpeechCode == "en_US") return;
        if (!_voiceDetectionToggle && !isRecording)
        {
            // _speechRecognitionState.color = Color.green;
        }
        if (clip == null || !_recognizeDirectlyToggle)
            return;

        RecognitionConfig config = RecognitionConfig.GetDefault();
        config.languageCode = "en_US";
        config.speechContexts = new SpeechContext[]
        {
            new SpeechContext()
            {
           // phrases = new string[1]{ typeSpeech.ToString() }
            }
        };
        config.audioChannelCount = clip.channels;
        // configure other parameters of the config if need

        GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
        {
            audio = new RecognitionAudioContent()
            {
                content = raw.ToBase64()
            },
            config = config
        };

        if (_longRunningRecognizeToggle)
        {
            _speechRecognition.LongRunningRecognize(recognitionRequest);
        }
        else
        {
            _speechRecognition.Recognize(recognitionRequest);
        }
    }

    private void StartedRecordEventHandler()
    {
        DebugRecognizeState(Color.red);
        Debug.Log("Start Recording...");
    }
    private void RecordFailedEventHandler()
    {
        isRecording = false;
    }
    private void BeginTalkigEventHandler()
    {
        DebugRecognizeState(Color.yellow);

    }
    private void EndTalkigEventHandler(AudioClip clip, float[] raw)
    {
        DebugRecognizeState(Color.red);
        FinishedRecordEventHandler(clip, raw);
    }

    [ContextMenu("StartRecordButtonOnClickHandler")]
    public void StartRecordButtonOnClickHandler()
    {
        Debug.Log("Start recording....");
        StopRecordButtonOnClickHandler();
        RefreshMicsButtonOnClickHandler();
        MicrophoneDevicesDropdownOnValueChangedEventHandler(0); // get the first Micro was found.
        isRecording = true;
        DebugRecognizeState(Color.red);
        _speechRecognition.StartRecord(_voiceDetectionToggle, clipTest);
    }

    public void StopRecordButtonOnClickHandler()
    {
        isRecording = false;
        DebugRecognizeState(Color.white);
        _speechRecognition.StopRecord();
    }
    private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
    {
        if (!_speechRecognition.HasConnectedMicrophoneDevices())
            return;
        _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
    }
    private void RefreshMicsButtonOnClickHandler()
    {
        _speechRecognition.RequestMicrophonePermission(null);
    }
    private void OnDestroy()
    {
        _speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
        _speechRecognition.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
        _speechRecognition.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
        _speechRecognition.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
        _speechRecognition.GetOperationFailedEvent -= GetOperationFailedEventHandler;
        _speechRecognition.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
        _speechRecognition.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

        _speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
        _speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
        _speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

        _speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
    }

    private void Update()
    {
        // if (_voiceLevelImage == null) return;
        if (_speechRecognition.IsRecording)
        {
            if (_speechRecognition.GetMaxFrame() > 0)
            {
                float max = (float)_speechRecognition.configs[_speechRecognition.currentConfigIndex].voiceDetectionThreshold;
                float current = _speechRecognition.GetLastFrame() / max;

                if (current >= 1f)
                {
                    OnVoiceLevelChanged?.Invoke(Mathf.Clamp(current / 2f, 0, 1f));
                    //_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 1f), 30 * Time.deltaTime);
                }
                else
                {
                    OnVoiceLevelChanged?.Invoke(Mathf.Clamp(current, 0.02f, 1));
                    // _voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current, 0, 1), 30 * Time.deltaTime);
                }
                // _voiceLevelImage.color = current >= 1f ? Color.green : Color.red;
            }
        }
        else
        {
            OnVoiceLevelChanged?.Invoke(0);
            //_voiceLevelImage.fillAmount = 0f;
        }
    }

    private void DebugRecognizeState(Color color)
    {
        if (_speechRecognitionState != null)
            _speechRecognitionState.color = color;
    }
}