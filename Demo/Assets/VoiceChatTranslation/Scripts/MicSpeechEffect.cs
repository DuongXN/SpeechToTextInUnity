using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


    public class MicSpeechEffect : MonoBehaviour
    {
        [SerializeField] private Image imgMicStatus;
        [SerializeField] private Sprite micOff, micOn;
        [SerializeField] Image voiceLevelImg;
        public void UpdateStateMic(bool isMicOn)
        {
            imgMicStatus.sprite = isMicOn ? micOn : micOff;
        }

        public void OnVoiceLevelChanged(float level)
        {
            voiceLevelImg.fillAmount = Mathf.Lerp(voiceLevelImg.fillAmount, Mathf.Clamp(level * 2, 0, 1), 30 * Time.deltaTime);
        }
    }
